using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ModList;

/// <summary>
/// 本地化
/// </summary>
public static class Localization
{
    public const string KeyPrefix = $"{Plugin.PluginName}_";

    public static string CurrentLanguage { get; private set; }

    public static void LoadLanguage()
    {
        var manager = LocalizationManager.Instance;
        if (manager is null) return;

        var currentLanguage = LocalizationManager.CurrentLanguage;
        if (currentLanguage < 0 || currentLanguage >= manager.Languages.Length) return;

        var path = GetFilePath(manager.Languages[currentLanguage]);
        CurrentLanguage = GetLanguageName(path);
        if (!File.Exists(path)) return;

        var text = File.ReadAllText(path);
        var localizationDict = Parse(text);

        if (localizationDict is null) return;
        var texts = LocalizationManager.CurrentTexts;
        var regex = new Regex(@"\\n");
        foreach (var (key, value) in localizationDict)
        {
            if (value.Count < 1) continue;
            texts[$"{KeyPrefix}{key}"] = regex.Replace(value[0], "\n");
        }
    }

    private static string GetFilePath(LanguageSetting setting)
    {
        var fileName = Path.GetFileName(setting.FilePath);
        if (fileName == "") fileName = "En.csv";
        return Path.Combine(Plugin.PluginPath, "Localization", fileName);
    }

    private static Dictionary<string, List<string>> Parse(string text)
    {
        try
        {
            return CSVParser.LoadFromString(text);
        }
        catch (Exception e)
        {
            Plugin.Log.LogError(e);
            return null;
        }
    }

    private static string GetLanguageName(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }

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