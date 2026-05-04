using Arcade.UI.SongSelect;
using HarmonyLib;
using UBUI.Archipelago;
using UNBEATAP.Objects;

namespace UNBEATAP.Helpers;

[HarmonyPatch(typeof(ArcadeSongListView))]
public class ArcadeSongListViewPatch
{
    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    static bool UpdatePrefix()
    {
        if(!Plugin.Client.Connected)
        {
            return true;
        }

        APConsole console = ArchipelagoManager.Instance.UIManager.console;
        if(!console)
        {
            return true;
        }

        // Disable song list scroll while the console is hovered
        return !console.hovered;
    }
}