using UnityEngine;

namespace UNBEATAP.Ratings;

public static class StarCalculator
{
    public const int RatingDivisor = 5625;
    public const float AccuracyPower = 1.12f;

    private static readonly RatingRange[] RatingRanges =
    [
        new RatingRange
        {
            min = 10,
            max = Mathf.Pow(15, AccuracyPower) + 10,
            gmod = 10
        },
        new RatingRange
        {
            min = Mathf.Pow(15, AccuracyPower) + 12,
            max = Mathf.Pow(25, AccuracyPower) + 12,
            gmod = 12
        },
        new RatingRange
        {
            min = Mathf.Pow(25, AccuracyPower) + 15,
            max = Mathf.Pow(35, AccuracyPower) + 15,
            gmod = 15
        },
        new RatingRange
        {
            min = Mathf.Pow(35, AccuracyPower) + 20,
            max = Mathf.Pow(40, AccuracyPower) + 20,
            gmod = 20
        },
        new RatingRange
        {
            min = Mathf.Pow(40, AccuracyPower) + 25,
            max = Mathf.Infinity,
            gmod = 25
        }
    ];


    public static float GetAccPower(float acc)
    {
        if(acc <= 50f)
        {
            return 0f;
        }

        return Mathf.Pow(acc -50f, AccuracyPower);
    }


    public static int GetGradeBonus(float acc, bool fc, bool fail)
    {
        if(fail)
        {
            return 0;
        }

        if(fc)
        {
            acc += 1f;
        }

        if(acc > 90f)
        {
            return 25;
        }
        else if(acc > 85)
        {
            return 20;
        }
        else if(acc > 75)
        {
            return 15;
        }
        else if(acc > 65)
        {
            return 12;
        }
        else return 10;
    }


    public static float GetRatingFromPlay(float level, float acc, bool fc, bool fail)
    {
        float accPower = GetAccPower(acc);
        float gradeBonus = GetGradeBonus(acc, fc, fail);

        float rating = level * (accPower + gradeBonus);
        return rating / RatingDivisor;
    }


    public static float GetExpectedAcc(float skillRating, int level)
    {
        // Port of the calculations from the apworld, refer to that repo for more detailed comments
        float requiredRatingFromSongs = RatingDivisor * (skillRating - 2f) / 25f;
        float requiredScoreRating = requiredRatingFromSongs / level;

        float requiredAccRating = 0;
        foreach(RatingRange grade in RatingRanges)
        {
            if(grade.min >= requiredScoreRating)
            {
                requiredAccRating = grade.min - grade.gmod;
                break;
            }

            if(grade.max >= requiredScoreRating)
            {
                requiredAccRating = requiredScoreRating - grade.gmod;
                break;
            }
        }

        return Mathf.Pow(requiredAccRating, 1f / AccuracyPower) + 50f;
    }


    public static float GetExpectedAccCurve(float skillRating, int level, float curveCutoff, float bias, bool allowPfc)
    {
        float rawAcc = GetExpectedAcc(skillRating, level) / 100;

        if(rawAcc < curveCutoff)
        {
            return rawAcc * 100;
        }

        float maxAcc = allowPfc ? 1f : 0.98f;
        float accEpsilon = 1f - maxAcc;

        float curveRange = 1f - curveCutoff - accEpsilon;

        float exponent = -bias * (rawAcc - curveCutoff);
        float curvedAcc = maxAcc - (curveRange * Mathf.Exp(exponent));
        return curvedAcc * 100;
    }


    private struct RatingRange
    {
        public float min;
        public float max;
        public float gmod;
    }
}
