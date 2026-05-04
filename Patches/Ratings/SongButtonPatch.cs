using Arcade.UI.SongSelect;
using HarmonyLib;
using TMPro;

namespace UNBEATAP.Helpers;

[HarmonyPatch(typeof(SongButton))]
public class SongButtonPatch
{
    [HarmonyPatch("SetSongData")]
    [HarmonyPostfix]
    static void SetSongDataPostfix(SongButton __instance, int songIndex)
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        Traverse traverse = new Traverse(__instance);
        TextMeshProUGUI songClearRank = traverse.Field("songClearRank").GetValue<TextMeshProUGUI>();

        ArcadeSongDatabase.BeatmapItem songAtIndex = ArcadeSongDatabase.Instance.GetSongAtIndex(songIndex);
        if(songAtIndex == null)
        {
            songClearRank.gameObject.SetActive(false);
            return;
        }

        HighScoreItem scoreItem = ArcadeSongDatabase.Instance.HighScores.GetScoreItem(songAtIndex.Path + HighScoreList.GetModifiersLeaderboard(StorableBeatmapOptions.GetModifierMask()));
        float acc = scoreItem.accuracy * 100f;

        int level = songAtIndex.Beatmap.metadata.tagData.Level;
        float expectedAcc = HighScoreHandler.GetExpectedAcc(level);

        songClearRank.gameObject.SetActive(true);
        if(!scoreItem.cleared)
        {
            // Normally, songClearRank is disabled in this case, but we still wanna show target acc
            songClearRank.text = $"<size=70%>(00.00% / {expectedAcc:00.00}%)</size>";
            return;
        }

        songClearRank.text += $" <size=70%>({acc:00.00}% / {expectedAcc:00.00}%)</size>";
    }
}