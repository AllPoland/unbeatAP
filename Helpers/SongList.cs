using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace UNBEATAP.Helpers
{
    public class SongList
    {
        // You can use SongList 'songList = SongList.GetInstance()' to add and get songs from the list
        // If using the ArcadeSongView patch, this will hide every song except for what is in the list.
        private static SongList _instance = new SongList();
        public static SongList GetInstance()
        {
            return _instance;
        }
        private List<String> songs = new List<String>();
        public void AddSong(String song)
        {
            songs.Add(song.ToLower());
        }

        public List<String> GetSongs()
        {
            return songs;
        }
        
        // May not be needed, depending on how we handle getting score data after song completion
        public string SongNameToUnlockName(string songName)
        {
            songName = songName.ToLower();
            // This expects the mod to be in UNBEATABLE/BepInEx/plugins/unbeatAP, and the Assets folder with songs.json to be next to it.
            var name = File.ReadAllText("BepInEx/plugins/unbeatAP/Assets/songs.json");
            var data = JObject.Parse(name);
            var unlockName = data[songName].ToString();
            return unlockName;
        }
    }
}