using HarmonyLib;

namespace ModList.Patcher;

[HarmonyPatch(typeof(GameLoad))]
public static class GameLoadPatch
{
    [HarmonyPostfix, HarmonyPatch("LoadMainGameData")]
    public static void LoadMainGameData_Postfix()
    {
        ModManager.Init();
    }
}