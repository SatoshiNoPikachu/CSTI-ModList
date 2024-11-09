using HarmonyLib;

namespace ModList.Patcher;

[HarmonyPatch(typeof(MainMenu))]
public static class MainMenuPatch
{
    [HarmonyPrefix, HarmonyPatch("Awake")]
    public static void Awake_Prefix(MainMenu __instance)
    {
        ModListUGUI.Create(__instance);
    }
}