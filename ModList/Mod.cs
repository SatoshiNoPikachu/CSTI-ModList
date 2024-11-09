using UnityEngine;

namespace ModList;

/// <summary>
/// 模组
/// </summary>
/// <param name="meta">模组元数据</param>
public class Mod(ModMeta meta)
{
    /// <summary>
    /// 元数据
    /// </summary>
    public ModMeta Meta => meta;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; } = meta.GetName();

    /// <summary>
    /// 描述
    /// </summary>
    public string Desc { get; } = meta.GetDesc();

    /// <summary>
    /// 作者
    /// </summary>
    public string Author { get; } = meta.Author;

    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; } = meta.Version;

    /// <summary>
    /// 图标
    /// </summary>
    public Sprite Icon { get; } = meta.SpriteIcon;

    /// <summary>
    /// 是否已启用
    /// </summary>
    public bool Enabled { get; } = meta.GetEnabled();

    /// <summary>
    /// 是否是ME模组
    /// </summary>
    public bool IsMe { get; private set; } = meta.IsMeMod;

    /// <summary>
    /// 是否是DLL模组
    /// </summary>
    public bool IsDLL { get; private set; } = meta.IsDLLMod;
}