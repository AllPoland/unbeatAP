using System.Collections.Generic;
using Arcade.Progression;
using HarmonyLib;
using UnityEngine;

namespace UNBEATAP.Patches;

public class RewardsNotificationPatch
{
    [HarmonyPatch(typeof(ArcadeRewardsNotification), "Fill")]
    [HarmonyPostfix]
    static void FillPatch(List<MainProgressionContainer.Unlock> unlocks, ref RectTransform ___maskTransform)
    {
        if(Plugin.Client.Connected)
        {
            int maxsize = Screen.height - 300;
            float calc = 350 + 50 * unlocks.Count;
            if((int)calc > maxsize)
            {
                calc = maxsize;
            }

            ___maskTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (float)(calc));
        }
    }
}