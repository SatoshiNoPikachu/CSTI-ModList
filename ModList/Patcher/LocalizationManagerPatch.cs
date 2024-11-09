using HarmonyLib;

namespace ModList.Patcher;

[HarmonyPatch(typeof(LocalizationManager))]
public static class LocalizationManagerPatch
{
    [HarmonyPostfix, HarmonyPatch("LoadLanguage")]
    public static void LoadLanguage_Postfix()
    {
        Localization.LoadLanguage();
    }
}