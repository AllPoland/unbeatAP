using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace UNBEATAP.Helpers;

public class SaveHandler : MonoBehaviour
{
    private Task saveTask;


    public static void Init()
    {
        GameObject newObject = new GameObject("AP Save Handler");
        newObject.AddComponent<SaveHandler>();
        DontDestroyOnLoad(newObject);
    }


    private async Task SaveData()
    {
        const string extension = ".apsave";

        try
        {
            List<string> items = Plugin.Client.ReceivedItems;

            SaveData newData = new SaveData
            {
                items = items.ToArray()
            };

            string seed = Plugin.Client.Session.RoomState.Seed;
            string fileName = seed + extension;
            string savePath = Path.Combine(Plugin.SaveFolder, fileName);

            if(!Directory.Exists(Plugin.SaveFolder))
            {
                Directory.CreateDirectory(Plugin.SaveFolder);
            }

            string json = JsonConvert.SerializeObject(newData);
            await File.WriteAllTextAsync(savePath, json);
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to save data with error: {e.Message}, {e.StackTrace}");
        }
    }


    private void Update()
    {
        // Keeping track of our save task avoids accidental overwrites
        if(saveTask == null)
        {
            bool isDirty = Plugin.Client.ItemsDirty;
            if(isDirty)
            {
                Plugin.Logger.LogInfo("Saving data.");
                // Run on a thread pool to make sure we don't hitch the game
                saveTask = Task.Run(SaveData);
            }
        }
        else if(saveTask.IsCompleted)
        {
            Plugin.Client.ItemsDirty = false;

            saveTask.Dispose();
            saveTask = null;
        }
    }


    private void OnDisable()
    {
        Plugin.Logger.LogError("The AP save handler has stopped.\n    Any items collected from this point on may be lost.");
    }
}


[Serializable]
public class SaveData
{
    public string[] items;
}