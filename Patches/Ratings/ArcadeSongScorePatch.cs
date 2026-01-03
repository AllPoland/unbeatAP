using Arcade.UI.SongSelect;
using HarmonyLib;
using TMPro;
using UNBEATAP.Helpers;

namespace UNBEATAP.Patches;

[HarmonyPatch(typeof(ArcadeSongScore))]
public class ArcadeSongScorePatch
{
    [HarmonyPatch("OnSelectedSongChanged")]
    [HarmonyPostfix]
    static void OnSelectedSongChangedPostfix(ArcadeSongScore __instance, ArcadeSongDatabase.BeatmapItem beatmapItem)
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        if(beatmapItem == null)
        {
            return;
        }

        Traverse traverse = new Traverse(__instance);
        string difficulty = traverse.Field("difficulty").GetValue<string>();
        if(!ArcadeSongDatabase.Instance.SongDatabase.ContainsKey(beatmapItem.Song.name + "/" + difficulty))
        {
            return;
        }

        ArcadeSongDatabase.BeatmapItem _song = traverse.Field("_song").GetValue<ArcadeSongDatabase.BeatmapItem>();
        if(!_song.Unlocked)
        {
            return;
        }

        string song = beatmapItem.Song.name + "/" + difficulty + "\\Classic";
        float expectedAcc = HighScoreHandler.GetExpectedAcc(_song.Beatmap.metadata.tagData.Level);

        float earnedAcc = 0f;
        if(HighScoreHandler.HighScores._highScores.TryGetValue(song, out HighScoreItem highScore))
        {
            earnedAcc = highScore.accuracy * 100f;
        }
        
        TextMeshProUGUI songDifficulty = traverse.Field("songDifficulty").GetValue<TextMeshProUGUI>();
        songDifficulty.text += $"<line-height=65%><br><size=70%>({earnedAcc:00.00}% / {expectedAcc:00.00}%)</size></line-height>";
    }
}