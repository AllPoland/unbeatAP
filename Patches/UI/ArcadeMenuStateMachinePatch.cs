using System;
using System.Collections.Generic;
using Arcade.UI.AnimationSystem;
using Arcade.UI.MenuStates;
using HarmonyLib;
using UNBEATAP.Objects;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(ArcadeMenuStateMachine))]
public class ArcadeMenuStateMachinePatch
{
    [HarmonyPatch("PlayTransition", [typeof(EArcadeMenuStates), typeof(EArcadeMenuStates), typeof(bool), typeof(Action)])]
    [HarmonyPostfix]
    static void PlayTransitionPostfix(EArcadeMenuStates entryState, EArcadeMenuStates exitState)
    {
        // Notify custom UI elements of the menu transition
        ArchipelagoManager.Instance.UIManager.PlayTransition(entryState, exitState);
    }
}