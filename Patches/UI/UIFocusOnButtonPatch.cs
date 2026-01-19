using HarmonyLib;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(UIFocusOnButton))]
public class UIFocusOnButtonPatch
{
    [HarmonyPatch("OnButtonPressed")]
    [HarmonyPrefix]
    static bool OnButtonPressedPrefix()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if(selected && selected.GetComponent<TMP_InputField>())
        {
            // An input field is selected, so ignore all keybinds
            return false;
        }

        return true;
    }
}