using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UNBEATAP.Helpers;

public static class HighScoreSaver
{
    public const string HighScoresKey = "highscores";
    public const string LatestScoreKey = "latestScore";

    public static readonly string[] NoteJudgements =
    [
        "Critical",
        "Perfect",
        "Great",
        "Good",
        "Ok",
        "Barely",
        "Miss"
    ];


    public static byte[] SerializeHighScore(HighScoreItem item)
    {
        item.OnBeforeSerialize();

        using MemoryStream stream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(stream);

        writer.Write(item.song);
        writer.Write(item.score);
        writer.Write(item.accuracy);
        writer.Write(item.maxCombo);
        writer.Write(item.cleared);
        writer.Write((int)item.modifierMask);
        writer.Write(item.level);

        Dictionary<string, int> notes = item._notes;
        // Note counts are always ordered in descending accuracy
        foreach(int count in notes.Values)
        {
            writer.Write(count);
        }

        return stream.ToArray();
    }


    public static HighScoreItem DeserializeHighScore(byte[] highscore)
    {
        using MemoryStream byteStream = new MemoryStream(highscore);
        using BinaryReader reader = new BinaryReader(byteStream);

        string song = reader.ReadString();
        int score = reader.ReadInt32();
        float accuracy = reader.ReadSingle();
        int maxCombo = reader.ReadInt32();
        bool cleared = reader.ReadBoolean();
        int modifiers = reader.ReadInt32();
        int level = reader.ReadInt32();

        Dictionary<string, int> noteCounts = new Dictionary<string, int>();
        for(int i = 0; i < NoteJudgements.Length; i++)
        {
            string key = NoteJudgements[i];
            noteCounts[key] = reader.ReadInt32();
        }

        return new HighScoreItem(
            song,
            score,
            accuracy,
            maxCombo,
            cleared,
            noteCounts,
            (Modifiers)modifiers,
            level
        );
    }


    public static string SerializeHighScores(HighScoreList list)
    {
        list.OnBeforeSerialize();

        List<byte[]> scores = new List<byte[]>(list.highScores.Count);
        foreach(HighScoreItem highScore in list.highScores)
        {
            byte[] newScore = SerializeHighScore(highScore);
            if(newScore != null)
            {
                scores.Add(newScore);
            }
        }

        SerializedHighscoreList newList = new SerializedHighscoreList
        {
            scores = scores.ToArray()
        };

        return JsonConvert.SerializeObject(newList);
    }


    public static HighScoreList DeserializeHighScores(string serialized)
    {
        SerializedHighscoreList list = JsonConvert.DeserializeObject<SerializedHighscoreList>(serialized);

        List<HighScoreItem> highscores = new List<HighScoreItem>();
        foreach(byte[] serializedScore in list.scores)
        {
            highscores.Add(DeserializeHighScore(serializedScore));
        }

        HighScoreList newList = new HighScoreList();
        newList.highScores = highscores;
        newList.OnAfterDeserialize();

        return newList;
    }


    public static void SaveHighScores()
    {
        if(!Plugin.Client.Connected)
        {
            return;
        }

        try
        {
            string serializedScores = SerializeHighScores(HighScoreHandler.HighScores);
            Plugin.Logger.LogInfo($"Saving high scores with size: {(float)serializedScores.Length / 1000}kb");
            Plugin.Client.Session.DataStorage[Scope.Slot, HighScoresKey] = serializedScores;
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to save highscores with error: {e.Message}, {e.StackTrace}");
        }
    }


    public static async Task LoadHighScores()
    {
        if(Plugin.Client?.Session?.DataStorage == null)
        {
            Plugin.Logger.LogError("Tried to load high scores without a valid session!");
            HighScoreHandler.HighScores = new HighScoreList();
            HighScoreHandler.ResetSavedRating();
            return;
        }

        try
        {
            string serializedScores = await Plugin.Client.Session.DataStorage[Scope.Slot, HighScoresKey].GetAsync<string>();
            if(string.IsNullOrEmpty(serializedScores))
            {
                Plugin.Logger.LogInfo($"No saved high scores yet, creating new.");
                HighScoreHandler.HighScores = new HighScoreList();
                HighScoreHandler.ResetSavedRating();
                return;
            }

            HighScoreList highScores = DeserializeHighScores(serializedScores);

            HighScoreHandler.HighScores = highScores;
            HighScoreHandler.ResetSavedRating();
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to load high scores with error: {e.Message}, {e.StackTrace}");
        }
    }


    public static void SetLatestScore(HighScoreItem score)
    {
        try
        {
            byte[] bytes = SerializeHighScore(score);
            string serialized = JsonConvert.SerializeObject(bytes);
            Plugin.Client.Session.DataStorage[Scope.Slot, LatestScoreKey] = serialized;
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to save latest score with error: {e.Message}, {e.StackTrace}");
        }
    }


    private static void AddNewScore(JToken scoreToken)
    {
        try
        {
            if(scoreToken == null)
            {
                return;
            }

            string json = scoreToken.ToObject<string>();
            if(string.IsNullOrEmpty(json))
            {
                return;
            }

            byte[] bytes = JsonConvert.DeserializeObject<byte[]>(json);
            HighScoreItem score = DeserializeHighScore(bytes);

            if(HighScoreHandler.ReplaceHighScoreCustom(score))
            {
                Plugin.Logger.LogInfo($"Received new high score for {score.song}");
            }
        }
        catch(Exception e)
        {
            Plugin.Logger.LogWarning($"Failed to deserialize latest score with error: {e.Message}, {e.StackTrace}");
        }
    }


    public static void OnLatestScoreUpdated(JToken originalValue, JToken newValue, Dictionary<string, JToken> additionalArguments)
    {
        AddNewScore(originalValue);
        AddNewScore(newValue);
    }


    [Serializable]
    private class SerializedHighscoreList
    {
        public byte[][] scores;
    }
}