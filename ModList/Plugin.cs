﻿using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ModList;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
internal class Plugin : BaseUnityPlugin
{
    public const string PluginGuid = "Pikachu.CSTI.ModList";
    public const string PluginName = "ModList";
    public const string PluginVersion = "1.2.0";

    public static Plugin Instance = null!;
    public static ManualLogSource Log = null!;
    private static readonly Harmony Harmony = new(PluginGuid);

    public static string PluginPath => Path.GetDirectoryName(Instance.Info.Location);

    private void Awake()
    {
        Instance = this;
        Log = Logger;
        Harmony.PatchAll();
        Log.LogInfo($"Plugin {PluginName} is loaded!");
    }
}