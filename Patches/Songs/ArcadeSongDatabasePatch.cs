using Arcade.UI.SongSelect;
using HarmonyLib;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(ArcadeSongDatabase))]
public class ArcadeSongDatabasePatch
{
    [HarmonyPatch("LoadDatabase")]
    [HarmonyPrefix]
    static void LoadCustomsPrefix(ref bool ____loadCustomSongs)
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        ____loadCustomSongs = false;
    }
}