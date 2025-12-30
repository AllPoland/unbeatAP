using Rhythm;
namespace UNBEATAP.Helpers;

public class RhythmManager
{
    RhythmController controller = RhythmController.Instance;
    RhythmConsts constants = RhythmConsts.Instance;

    // None of this has been implemented properly yet, but all of this does work
    
    // Missing doubletime and halftime
    public void GetMods()
    {
        bool assist = controller.assistMode;
        bool critical = controller.useCritical;
        bool nofail = controller.noFail;
    }

    public void GetScoreInfo()
    {
        bool fc = controller.song.missCount == 0;
        bool cleared = !controller.song.gameOver;
        string score = controller.score.totalScoreText;
        float acc = controller.song.accuracy;
        RhythmConsts.ResultGrade rank = RhythmConsts.GetGrade(acc, fc, cleared);
        string rank2 = rank.grade.ToString();
    }

    public void GetBeatmapInfo()
    {
        string maptitle = controller.beatmap.metadata.title;
        float songlength = controller.beatmap.metadata.tagData.SongLength;
        int songRating = controller.beatmap.metadata.tagData.Level;
    }


}