﻿using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Bootstrap;
using LitJson;

namespace ModList;

/// <summary>
/// 模组管理器
/// </summary>
public class ModManager
{
    /// <summary>
    /// ModLoader是否已安装
    /// </summary>
    public static bool ModLoaderInstalled { get; private set; }

    /// <summary>
    /// ModLoader是否有效
    /// </summary>
    public static bool ModLoaderValid { get; private set; }

    /// <summary>
    /// 模组元数据数组
    /// </summary>
    public static ModMeta[] ModMetas { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    internal static void Init()
    {
        ModMetas = new ModManager().GetMetas();
    }

    /// <summary>
    /// plugins目录
    /// </summary>
    private readonly DirectoryInfo _dirPlugins = new(Paths.PluginPath);

    /// <summary>
    /// plugins目录下目录数组
    /// </summary>
    private readonly DirectoryInfo[] _modDirs = new DirectoryInfo(Paths.PluginPath).GetDirectories();

    /// <summary>
    /// 插件信息字典
    /// </summary>
    private readonly Dictionary<string, PluginInfo> _pluginInfos = Chainloader.PluginInfos;

    /// <summary>
    /// ME模组目录集合
    /// </summary>
    private readonly HashSet<string> _meDirs = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// ME模组元数据字典
    /// </summary>
    private readonly Dictionary<string, ModMeta> _meMetas = [];

    /// <summary>
    /// DLL模组元数据字典
    /// </summary>
    private readonly Dictionary<string, ModMeta> _dllMetas = [];

    /// <summary>
    /// 模组元数据列表
    /// </summary>
    private readonly List<ModMeta> _allMetas = [];

    private ModManager()
    {
    }
    
    /// <summary>
    /// 获取元数据
    /// </summary>
    /// <returns>元数据数组</returns>
    private ModMeta[] GetMetas()
    {
        CheckModLoader();

        if (ModLoaderInstalled) LoadMeMetas();
        if (ModLoaderValid) LoadModPacks();

        LoadPluginMetas();
        RemoveInvalidMetas();

        return _allMetas.ToArray();
    }

    /// <summary>
    /// 检查ModLoader安装情况
    /// </summary>
    private void CheckModLoader()
    {
        ModLoaderInstalled = _pluginInfos.TryGetValue("Dop.plugin.CSTI.ModLoader", out var info);
        if (!ModLoaderInstalled) return;
        ModLoaderValid = info!.Metadata.Version >= new Version(2, 0, 8);
    }

    /// <summary>
    /// 加载ME模组元数据
    /// </summary>
    private void LoadMeMetas()
    {
        foreach (var modDir in _modDirs)
        {
            var pathInfo = Path.Combine(modDir.FullName, "ModInfo.json");
            if (!File.Exists(pathInfo)) continue;

            var (name, author) = GetModInfoNameAuthor(pathInfo);
            if (string.IsNullOrWhiteSpace(name) || _meMetas.ContainsKey(name)) continue;

            var pathMeta = Path.Combine(modDir.FullName, "ModMeta.json");
            if (!File.Exists(pathMeta)) continue;

            var meta = LoadMeta(pathMeta);
            if (meta is null) continue;

            _meDirs.Add(modDir.FullName);

            meta.TrySetAuthor(author);

            RegisterMeMeta(name, meta);
            RegisterDllMeta(meta);
        }
    }

    /// <summary>
    /// 加载模组包
    /// </summary>
    private void LoadModPacks()
    {
        var packs = ModLoader.ModLoader.ModPacks;

        foreach (var (name, meta) in _meMetas)
        {
            if (packs.ContainsKey(name)) continue;

            meta.MeName = null;
            Plugin.Log.LogWarning($"ModLoader not loaded mod '{name}'.");
        }

        foreach (var (name, pack) in packs)
        {
            if (!_meMetas.TryGetValue(name, out var meta))
            {
                meta = new ModMeta();
                RegisterMeMeta(name, meta);
            }

            meta.MeEnable = pack.EnableEntry;
            meta.TrySetVersion(pack.ModInfo.Version);
        }
    }

    /// <summary>
    /// 加载DLL模组元数据
    /// </summary>
    private void LoadPluginMetas()
    {
        foreach (var (guid, info) in _pluginInfos)
        {
            if (!_dllMetas.TryGetValue(guid, out var meta))
            {
                var dir = new DirectoryInfo(Path.GetDirectoryName(info.Location)!);
                var pathMeta = Path.Combine(dir.FullName, "ModMeta.json");

                if (IsSamePath(_dirPlugins, dir) || _meDirs.Contains(dir.FullName) || !File.Exists(pathMeta))
                {
                    meta = new ModMeta
                    {
                        PluginGuid = [guid]
                    };
                }
                else
                {
                    meta = LoadMeta(pathMeta) ?? new ModMeta
                    {
                        PluginGuid = [guid]
                    };
                }

                RegisterDllMeta(meta);
                _allMetas.Add(meta);
            }

            meta.AddPluginName(guid, info.Metadata.Name);
            meta.TrySetVersion(info.Metadata.Version.ToString());
        }
    }

    /// <summary>
    /// 移除无效的模组元数据
    /// </summary>
    private void RemoveInvalidMetas()
    {
        _allMetas.RemoveAll(meta => !meta.CheckValid());
    }

    /// <summary>
    /// 注册ME模组元数据
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="meta">元数据</param>
    private void RegisterMeMeta(string name, ModMeta meta)
    {
        meta.MeName = name;
        _meMetas[name] = meta;
        _allMetas.Add(meta);
    }

    /// <summary>
    /// 注册DLL模组元数据
    /// </summary>
    /// <param name="meta">元数据</param>
    private void RegisterDllMeta(ModMeta meta)
    {
        if (meta.PluginGuid is null) return;

        foreach (var guid in meta.PluginGuid)
        {
            if (string.IsNullOrWhiteSpace(guid)) continue;
            if (_dllMetas.ContainsKey(guid))
            {
                Plugin.Log.LogWarning($"Duplicate registered plugin GUID: '{guid}'.");
                continue;
            }

            _dllMetas[guid] = meta;
        }
    }

    /// <summary>
    /// 加载元数据
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns>元数据</returns>
    private static ModMeta LoadMeta(string path)
    {
        try
        {
            var meta = JsonMapper.ToObject<ModMeta>(File.ReadAllText(path));
            meta.LoadIcon(path);
            return meta;
        }
        catch (Exception)
        {
            Plugin.Log.LogWarning($"Failed to load ModMeta from '{Path.GetDirectoryName(path)}'.");
            return null;
        }
    }

    /// <summary>
    /// 获取ME模组信息的名称和作者
    /// </summary>
    /// <param name="path">ME模组信息路径</param>
    /// <returns>名称和作者</returns>
    private static (string, string) GetModInfoNameAuthor(string path)
    {
        try
        {
            var info = JsonMapper.ToObject(File.ReadAllText(path));
            return ((string)info["Name"], (string)info["Author"]);
        }
        catch (Exception)
        {
            Plugin.Log.LogWarning($"Failed to load ModInfo from '{Path.GetDirectoryName(path)}'.");
            return (null, null);
        }
    }

    /// <summary>
    /// 是否是相同路径
    /// </summary>
    /// <param name="dir1">路径1</param>
    /// <param name="dir2">路径2</param>
    /// <returns>是否相同</returns>
    private static bool IsSamePath(FileSystemInfo dir1, FileSystemInfo dir2)
    {
        return dir1.FullName.Equals(dir2.FullName, StringComparison.OrdinalIgnoreCase);
    }
}