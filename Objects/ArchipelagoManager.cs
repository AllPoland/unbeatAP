using System;
using System.Collections;
using System.Threading.Tasks;
using UBUI.Colors;
using UBUI.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UNBEATAP.Objects;

public class ArchipelagoManager : MonoBehaviour
{
    public static ArchipelagoManager Instance { get; private set; }

    public bool IsArcadeMenu { get; private set; }

    public AssetBundle UIBundle;
    public AssetBundle APUIBundle;

    public UIManager UIManager;
    public ColorManager ColorManager;

    public event Action<Scene> OnSceneLoaded;

    private bool connecting;


    private IEnumerator ConnectCoroutine()
    {
        connecting = true;

        Plugin.SetupNewClient();

        using Task connectTask = Plugin.Client.ConnectAndGetData();
        yield return new WaitUntil(() => connectTask.IsCompleted);

        connecting = false;
    }


    public void CreateClientAndConnect()
    {
        if(Plugin.Client.Connected)
        {
            Plugin.Logger.LogWarning("Tried to connect while already conneced!");
            return;
        }

        if(connecting)
        {
            Plugin.Logger.LogWarning($"Tried to connect while already connecting!");
            return;
        }

        StartCoroutine(ConnectCoroutine());
    }


    public void LoadAssetBundles()
    {
        Plugin.Logger.LogInfo("Loading UI.");

        try
        {
            // Load all dependency assets
            UIBundle = AssetBundle.LoadFromFile(Plugin.UiResourcesBundlePath);
            UIBundle.LoadAllAssets();
            UIBundle.Unload(false);

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


    private void UpdateScene(Scene current, Scene next)
    {
        IsArcadeMenu = next.name == JeffBezosController.arcadeMenuScene;
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
        DontDestroyOnLoad(gameObject);

        LoadAssetBundles();

        ColorManager = gameObject.AddComponent<ColorManager>();
        UIManager = gameObject.AddComponent<UIManager>();

        SceneManager.activeSceneChanged += UpdateScene;
    }
}