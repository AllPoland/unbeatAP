using UnityEngine;

namespace UNBEATAP.Ratings;

public static class CustomRatingCalculator
{
    public const float DifficultyPower = 2f;
    public const float ScoreFalloffBase = 0.965f;


    public static float GetCustomRatingFromPlay(float level, float acc, bool fc, bool fail)
    {
        float adjustedLevel = Mathf.Pow(level, DifficultyPower);
        return StarCalculator.GetRatingFromPlay(adjustedLevel, acc, fc, fail);
    }


    public static float GetScoreContribution(float scoreRating, int scoreIdx)
    {
        return Mathf.Pow(ScoreFalloffBase, scoreIdx) * scoreRating;
    }
}
