using System;
using System.Collections.Generic;
using System.Linq;
using Rhythm;
using UNBEATAP.AP;
using UNBEATAP.Ratings;

namespace UNBEATAP.Helpers;

public static class HighScoreHandler
{
    public const float ExpectedFailThreshold = 55f;

    public static HighScoreList HighScores = new HighScoreList();

    private static float? playerRating;


    public static void ResetSavedRating()
    {
        playerRating = null;
    }


    public static float GetOverallPlayerRating()
    {
        if(playerRating.HasValue)
        {
            return playerRating.Value;
        }

        if(HighScores == null || HighScores._highScores == null || HighScores._highScores.Count == 0)
        {
            return 0f;
        }

        Dictionary<string, float> songRatings = new Dictionary<string, float>();

        // This method is also responsible for tracking some other stats
        int totalScore = 0;
        foreach(HighScoreItem score in HighScores._highScores.Values.ToList())
        {
            if(!score.cleared)
            {
                continue;
            }

            if(score.IsCustom())
            {
                continue;
            }

            totalScore += score.score;
            if(score.leaderboard.Contains("HalfTime"))
            {
                continue;
            }

            if(score.level == 0)
            {
                // Check the source beatmap level if it exists
                BeatmapParser.ParseBeatmap(BeatmapIndex.CachedDefaultIndex, score.GetSongPath(), out BeatmapInfo _, out Beatmap beatmap, out string _, BeatmapParserEngine.SectionTypes.General | BeatmapParserEngine.SectionTypes.Metadata);
                if(beatmap == null)
                {
                    continue;
                }
                score.level = beatmap.metadata.tagData.Level;
            }

            int missCount = 0;
            score._notes.TryGetValue("Miss", out missCount);
            float rating = CustomRatingCalculator.GetCustomRatingFromPlay(score.level, score.accuracy, missCount <= 0, !score.cleared);

            if(songRatings.ContainsKey(score.song) && songRatings[score.song] < rating)
            {
                songRatings[score.song] = rating;
            }
            else
            {
                songRatings.Add(score.song, rating);
            }
        }

        PlayerStats.Update("combinedHighScore", totalScore);

        List<Tuple<string, float>> orderedRatings = songRatings.Select(x => new Tuple<string, float>(x.Key, x.Value)).ToList();
        orderedRatings = orderedRatings.OrderByDescending(x => x.Item2).ToList();

        List<Tuple<string, float>> adjustedRatings = new List<Tuple<string, float>>();

        // Adjust the overall rating to always be on a scale from 0 to 100
        float targetRating = Plugin.Client.SlotData.TargetRating;

        playerRating = 0f;
        for(int i = 0; i < orderedRatings.Count; i++)
        {
            Tuple<string, float> rating = orderedRatings[i];
            float adjustedRating = CustomRatingCalculator.GetScoreContribution(rating.Item2, i);

            adjustedRating /= targetRating;
            adjustedRating *= 100f;

            playerRating += adjustedRating;
            adjustedRatings.Add(new Tuple<string, float>(rating.Item1, adjustedRating));
        }

        // Updates the list of songs on the player profile
        PlayerStats.Update("songRatings", adjustedRatings);
        FileStorage.SaveStats();

        Plugin.Client.HandleRatingUpdate(playerRating.Value);
        return playerRating.Value;
    }


    public static float GetExpectedAcc(int level)
    {
        SlotData slotData = Plugin.Client.SlotData;
        float expectedAcc = StarCalculator.GetExpectedAccCurve(
            slotData.SkillRating,
            level,
            slotData.AccCurveCutoff,
            slotData.AccCurveBias,
            slotData.AllowPfc
        );

        if(expectedAcc < ExpectedFailThreshold)
        {
            return 0f;
        }
        else return expectedAcc;
    }
}