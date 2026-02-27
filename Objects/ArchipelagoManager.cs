using System;
using System.Collections;
using System.Threading.Tasks;
using UBUI.Colors;
using UBUI.Serialization;
using UNBEATAP.Helpers;
using UNBEATAP.Traps;
using UnityEngine;
using UnityEngine.SceneManagement;
using Arcade.UI.SongSelect;

namespace UNBEATAP.Objects;

public class ArchipelagoManager : MonoBehaviour
{
    public static ArchipelagoManager Instance { get; private set; }

    public bool IsArcadeMenu { get; private set; }

    public static AssetBundle APUIBundle;

    public UIManager UIManager;
    public ColorManager ColorManager;

    public event Action<Scene> OnSceneLoaded;

    public bool Connecting { get; private set; }
    public bool SavingHighScores { get; private set; }

    private const float songListRefreshTime = 0.5f;
    private float songListDirtyTime;


    private IEnumerator ConnectCoroutine()
    {
        Connecting = true;

        Plugin.SetupNewClient();

        if(Plugin.Client.Session != null)
        {
            using Task connectTask = Task.Run(Plugin.Client.ConnectAndGetData);
            yield return new WaitUntil(() => connectTask.IsCompleted);
        }

        Connecting = false;
    }


    private IEnumerator SaveHighScoresCoroutine()
    {
        SavingHighScores = true;

        using Task saveTask = Task.Run(HighScoreSaver.SaveHighScores);
        yield return new WaitUntil(() => saveTask.IsCompleted);

        SavingHighScores = false;
    }


    public void CreateClientAndConnect()
    {
        if(Plugin.Client.Connected)
        {
            Plugin.Logger.LogWarning("Tried to connect while already conneced!");
            return;
        }

        if(Connecting)
        {
            Plugin.Logger.LogWarning($"Tried to connect while already connecting!");
            return;
        }

        StartCoroutine(ConnectCoroutine());
    }


    public void SaveHighScores()
    {
        if(SavingHighScores)
        {
            Plugin.Logger.LogWarning("Tried to save high scores while already saving!");
            return;
        }

        StartCoroutine(SaveHighScoresCoroutine());
    }


    public void SetSongListDirty()
    {
        if(!IsArcadeMenu)
        {
            // Song list gets refreshed when going to arcade menu anyway
            return;
        }

        songListDirtyTime = songListRefreshTime;
    }


    public static void LoadAssetBundles()
    {
        if(APUIBundle)
        {
            return;
        }

        Plugin.Logger.LogInfo("Loading UI.");

        try
        {
            // Load all dependency assets
            AssetBundle uiBundle = AssetBundle.LoadFromFile(Plugin.UiResourcesBundlePath);
            uiBundle.LoadAllAssets();

            // For some reason, the font *only* works if you load the asset, *then* unload the bundle
            // No idea why this is the case, but this means we need to put it in a separate bundle
            // because for any other asset, unloading the bundle also makes the asset inaccessible
            AssetBundle fontBundle = AssetBundle.LoadFromFile(Plugin.FontResourcesBundlePath);
            fontBundle.LoadAllAssets();
            fontBundle.Unload(false);

            APUIBundle = AssetBundle.LoadFromFile(Plugin.ApUiBundlePath);

            // These manifest files let us restore custom components on prefabs
            PrefabInitializer.AddComponentManifest(Plugin.UiResourcesBundlePath);
            PrefabInitializer.AddComponentManifest(Plugin.ApUiBundlePath);
        }
        catch(Exception e)
        {
            Plugin.Logger.LogError($"Failed to load AssetBundles with error: {e.Message}\n    {e.StackTrace}");
        }
    }


    public void UpdateScene(Scene current, Scene next)
    {
        IsArcadeMenu = next.name == JeffBezosController.arcadeMenuScene;
        if(IsArcadeMenu && Muted.IsMuted)
        {
            // Force unmute the music when we go back to the menu
            Muted.UnMute();
        }

        songListDirtyTime = 0f;
        OnSceneLoaded?.Invoke(next);
    }


    private void Awake()
    {
        if(Instance && Instance != this)
        {
            enabled = false;
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        ColorManager = gameObject.AddComponent<ColorManager>();
        UIManager = gameObject.AddComponent<UIManager>();
    }


    private void OnDestroy()
    {
        Plugin.Logger.LogWarning("ArchipelagoManager was destroyed!");
    }


    private void Update()
    {
        // Only show notification if no items in queue
        if(Plugin.Client.Connected && IsArcadeMenu && !(Plugin.Client.Session?.Items.Any() ?? false))
        {
            // Notifications must be shown in main unity thread, otherwise it causes a game crash
            NotificationHelper.ShowNotification();

            // Refresh song list after a bit of time has passed without getting new songs
            if(songListDirtyTime >= 0f && ArcadeSongDatabase.Instance)
            {
                songListDirtyTime -= Time.deltaTime;
                if(songListDirtyTime <= 0f)
                {
                    ArcadeSongDatabase.Instance.LoadDatabase();
                    ArcadeSongDatabase.Instance.RefreshSongList();
                }
            }
        }
    }
}