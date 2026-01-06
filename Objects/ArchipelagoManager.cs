using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UNBEATAP.Objects;

public class ArchipelagoManager : MonoBehaviour
{
    public static ArchipelagoManager Instance { get; private set; }

    public bool IsArcadeMenu { get; private set; }

    public UIManager UIManager;

    public event Action<Scene> OnSceneLoaded;


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

        UIManager = gameObject.AddComponent<UIManager>();

        SceneManager.activeSceneChanged += UpdateScene;
    }
}