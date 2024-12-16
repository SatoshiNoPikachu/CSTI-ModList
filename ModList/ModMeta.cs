using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx.Configuration;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ModList;

/// <summary>
/// 模组元数据
/// </summary>
[Serializable]
public class ModMeta
{
    /// <summary>
    /// 本地化名称
    /// </summary>
    public Dictionary<string, string> Name;

    /// <summary>
    /// 本地化描述
    /// </summary>
    public Dictionary<string, string> Description;

    /// <summary>
    /// 作者
    /// </summary>
    public string Author;

    /// <summary>
    /// 版本
    /// </summary>
    public string Version;

    /// <summary>
    /// 图标路径
    /// </summary>
    public string Icon;

    /// <summary>
    /// 插件GUID
    /// </summary>
    public string[] PluginGuid;

    /// <summary>
    /// ME名称
    /// </summary>
    internal string MeName;

    /// <summary>
    /// ME启用配置项
    /// </summary>
    internal ConfigEntry<bool> MeEnable;

    /// <summary>
    /// 插件名称
    /// </summary>
    internal Dictionary<string, string> PluginName;

    /// <summary>
    /// 图标
    /// </summary>
    internal Sprite SpriteIcon;

    /// <summary>
    /// 模组特质
    /// </summary>
    internal CharacterPerk[] Perks;

    /// <summary>
    /// 是否是ME模组
    /// </summary>
    internal bool IsMeMod => !string.IsNullOrWhiteSpace(MeName);

    /// <summary>
    /// 是否是DLL模组
    /// </summary>
    internal bool IsDLLMod => PluginName is { Count: > 0 };

    /// <summary>
    /// 检查是否有效
    /// </summary>
    /// <returns></returns>
    public bool CheckValid()
    {
        return IsMeMod || IsDLLMod;
    }

    /// <summary>
    /// 设置作者，仅当作者为无效值时生效
    /// </summary>
    /// <param name="author">作者</param>
    public void TrySetAuthor(string author)
    {
        if (string.IsNullOrWhiteSpace(Author) && !string.IsNullOrWhiteSpace(author)) Author = author;
    }

    /// <summary>
    /// 设置版本，仅当作者为无效值时生效
    /// </summary>
    /// <param name="version"></param>
    public void TrySetVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(Version) && !string.IsNullOrWhiteSpace(version)) Version = version;
    }

    /// <summary>
    /// 加载图标
    /// </summary>
    /// <param name="path">路径</param>
    public void LoadIcon(string path)
    {
        if (string.IsNullOrWhiteSpace(Icon)) return;

        var pathDir = Directory.Exists(path) ? path : Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(path)) return;

        var pathIcon = Path.Combine(pathDir!, Icon);
        if (!File.Exists(pathIcon)) return;

        var bytes = File.ReadAllBytes(pathIcon);
        var tex = new Texture2D(0, 0)
        {
            name = "ModIcon"
        };
        if (!tex.LoadImage(bytes))
        {
            Object.Destroy(tex);
            Plugin.Log.LogWarning($"Image format not supported: '{pathIcon}'");
            return;
        }

        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        sprite.name = tex.name;

        SpriteIcon = sprite;
    }

    /// <summary>
    /// 添加插件名称
    /// </summary>
    /// <param name="guid">插件GUID</param>
    /// <param name="name">名称</param>
    public void AddPluginName(string guid, string name)
    {
        PluginName ??= [];
        PluginName[guid] = name;
    }

    /// <summary>
    /// 获取名称
    /// </summary>
    /// <returns>名称</returns>
    public string GetName()
    {
        if (Name is { Count: > 0 })
        {
            return Name.TryGetValue(Localization.CurrentLanguage, out var name) ? name : Name.First().Value;
        }

        if (MeName is not null) return MeName;

        if (PluginName?.Count is null or 0) return "";
        var (guid, pluginName) = PluginName.FirstOrDefault();
        return string.IsNullOrWhiteSpace(pluginName) ? guid : pluginName;
    }

    /// <summary>
    /// 获取描述
    /// </summary>
    /// <returns></returns>
    public string GetDesc()
    {
        if (Description is null or { Count: 0 }) return "";

        return Description.TryGetValue(Localization.CurrentLanguage, out var desc) ? desc : Description.First().Value;
    }

    /// <summary>
    /// 获取是否启用
    /// </summary>
    /// <returns></returns>
    public bool GetEnabled()
    {
        return MeEnable?.Value is null or true;
    }
}