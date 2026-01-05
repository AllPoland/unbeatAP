using HarmonyLib;
using UNBEATAP.Traps;
using UnityEngine;

namespace UNBEATAP.Patches;

public class RainbowTrapPatch
{
    [HarmonyPatch(typeof(StorableBeatmapOptions), "normalNoteColor", MethodType.Getter)]
    [HarmonyPrefix]
    static bool NormalColorPatch(ref Color __result)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if(!Rainbow.GetRainbow())
        {
            return true;
        }

        __result = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
        return false;
    }


    [HarmonyPatch(typeof(StorableBeatmapOptions), "multiHitNotesColor", MethodType.Getter)]
    [HarmonyPrefix]
    static bool MultiHitNotesColorPatch(ref Color __result)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if(!Rainbow.GetRainbow())
        {
            return true;
        }

        __result = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
        return false;
    }


    [HarmonyPatch(typeof(StorableBeatmapOptions), "laneSwapNoteColor", MethodType.Getter)]
    [HarmonyPrefix]
    static bool LaneSwapNoteColorPatch(ref Color __result)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if(!Rainbow.GetRainbow())
        {
            return true;
        }

        __result = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
        return false;
    }


    [HarmonyPatch(typeof(StorableBeatmapOptions), "dodgeNoteColor", MethodType.Getter)]
    [HarmonyPrefix]
    static bool DodgeNoteColorPatch(ref Color __result)
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        if(!Rainbow.GetRainbow())
        {
            return true;
        }

        __result = new Color(Random.Range(0F,1F), Random.Range(0, 1F), Random.Range(0, 1F));
        return false;
    }
}