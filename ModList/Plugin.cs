using BepInEx;
using HarmonyLib;
using ModCore;
using ModCore.Services;

namespace ModList;

[BepInDependency("Pikachu.CSTI.ModCore")]
[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
internal class Plugin : BaseUnityPlugin<Plugin>
{
    public const string PluginGuid = "Pikachu.CSTI.ModList";
    public const string PluginName = "ModList";
    public const string PluginVersion = "2.0.0";

    private static readonly Harmony Harmony = new(PluginGuid);

    protected override void Awake()
    {
        base.Awake();
        Harmony.PatchAll();

        LocalizationService.RegisterPath(PluginPath, Localization.KeyPrefix);

        Log.LogMessage($"Plugin {PluginName} is loaded!");
    }
}