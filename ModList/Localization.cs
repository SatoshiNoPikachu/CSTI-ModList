using ModCore.Services;

namespace ModList;

/// <summary>
/// 本地化
/// </summary>
public static class Localization
{
    public const string KeyPrefix = $"{Plugin.PluginName}_";

    public static string CurrentLanguage => LocalizationService.CurrentLanguage;

    public static LocalizedString NoName => new()
    {
        DefaultText = "无名称",
        LocalizationKey = $"{KeyPrefix}NoName"
    };

    public static LocalizedString Enabled => new()
    {
        DefaultText = "已启用",
        LocalizationKey = $"{KeyPrefix}Enabled"
    };

    public static LocalizedString NotEnabled => new()
    {
        DefaultText = "未启用",
        LocalizationKey = $"{KeyPrefix}NotEnabled"
    };

    public static LocalizedString ModAuthor => new()
    {
        DefaultText = "作者",
        LocalizationKey = $"{KeyPrefix}ModAuthor"
    };

    public static LocalizedString ModTypeDLL = new()
    {
        DefaultText = "DLL",
        LocalizationKey = $"{KeyPrefix}ModTypeDLL"
    };

    public static LocalizedString ModTypeMe = new()
    {
        DefaultText = "ME",
        LocalizationKey = $"{KeyPrefix}ModTypeME"
    };
}