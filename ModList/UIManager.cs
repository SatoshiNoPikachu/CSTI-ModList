using System.Collections.Generic;
using UnityEngine;

namespace ModList;

public static class UIManager
{
    private static readonly Dictionary<string, Object> Prefabs = [];

    public static bool RegisterPrefab(string uid, GameObject go)
    {
        if (!go || Prefabs.ContainsKey(uid)) return false;

        Object.DontDestroyOnLoad(go);
        Prefabs[uid] = go;
        return true;
    }

    public static bool RegisterPrefab(string uid, Component comp)
    {
        if (!comp || Prefabs.ContainsKey(uid)) return false;

        Object.DontDestroyOnLoad(comp);
        Prefabs[uid] = comp;
        return true;
    }

    public static Object GetPrefab(string uid)
    {
        return Prefabs.TryGetValue(uid, out var obj) ? obj : null;
    }

    public static T GetPrefab<T>(string uid) where T : Component
    {
        return GetPrefab(uid) as T;
    }

    public static GameObject GetPrefabAsGameObject(string uid)
    {
        return GetPrefab(uid) switch
        {
            GameObject go => go,
            Component comp => comp.gameObject,
            _ => null
        };
    }
}