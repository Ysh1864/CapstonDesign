using System.Collections.Generic;


public static class FunctionalData
{
    // key: "æ¿¿Ã∏ß_±‚πÕID" / value: »∞º∫»≠ ø©∫Œ
    private static Dictionary<string, bool> activatedGimmicks = new Dictionary<string, bool>();

    public static void SetActivated(string sceneKey, bool value)
    {
        activatedGimmicks[sceneKey] = value;
    }

    public static bool IsActivated(string sceneKey)
    {
        return activatedGimmicks.TryGetValue(sceneKey, out bool value) && value;
    }

    public static void Clear()
    {
        activatedGimmicks.Clear();
    }
}
