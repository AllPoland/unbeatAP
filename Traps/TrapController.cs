using System;
using System.Collections.Generic;

namespace UNBEATAP.Traps;

public static class TrapController
{
    public const string TrapSuffix = " Trap";

    public static readonly Dictionary<string, Action> TrapMethods = new Dictionary<string, Action>
    {
        {"Silence", Muted.Timer.Activate},
        {"Rainbow", Rainbow.Timer.Activate},
        {"Zoom", ScrollSpeed.ActivateZoom},
        {"Crawl", ScrollSpeed.ActivateCrawl},
        {"Stealth", Stealth.Timer.Activate}
    };


    public static void ActivateTrap(string itemName)
    {
        string trapName = itemName.Replace(TrapSuffix, "");

        if(!TrapMethods.TryGetValue(trapName, out Action trapMethod))
        {
            Plugin.Logger.LogWarning($"Unable to handle trap: {itemName}");
            return;
        }

        Plugin.Logger.LogInfo($"Activated trap: {itemName}");
        trapMethod?.Invoke();
    }


    public static void UpdateTraps(float deltaTime)
    {
        Muted.Timer.Update(deltaTime);
        Rainbow.Timer.Update(deltaTime);
        ScrollSpeed.Update(deltaTime);
        Stealth.Timer.Update(deltaTime);
    }


    public static void DeactivateTraps()
    {
        Muted.Deactivate();
        Rainbow.Timer.Deactivate();
        ScrollSpeed.Deactivate();
        Stealth.Timer.Deactivate();
    }
}