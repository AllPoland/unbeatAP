using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.SceneManagement;

namespace UNBEATAP.Objects;

public class UIManager : MonoBehaviour
{
    private const string disconnectText = "<mspace=11>//<mspace=17> </mspace><cspace=0.35em>disconnect.";

    private ArchipelagoManager Manager => ArchipelagoManager.Instance;


    private void InitArcadeUIConnected(Transform root)
    {
        Transform screenArea = root.GetChild(1);
        Transform mainScreens = screenArea.GetChild(1);
        Transform mainMenu = mainScreens.GetChild(1);

        Transform buttons = mainMenu.GetChild(2);
        Transform storyButtonContainer = buttons.GetChild(2);

        Transform textIdle = storyButtonContainer.GetChild(0).GetChild(0).GetChild(0);
        Transform textActive = textIdle.GetChild(0).GetChild(0);

        textIdle.gameObject.GetComponent<GameObjectLocalizer>().enabled = false;
        textActive.gameObject.GetComponent<GameObjectLocalizer>().enabled = false;

        textIdle.gameObject.GetComponent<TextMeshProUGUI>().SetText(disconnectText);
        textActive.gameObject.GetComponent<TextMeshProUGUI>().SetText(disconnectText);

        Plugin.Logger.LogInfo(textIdle.gameObject.name);
    }


    private void InitArcadeUI(Transform root)
    {
        if(Plugin.Client.Connected)
        {
            InitArcadeUIConnected(root);
        }
    }


    private void HandleSceneLoaded(Scene newScene)
    {
        if(Manager.IsArcadeMenu)
        {
            GameObject[] roots = newScene.GetRootGameObjects();
            GameObject arcadeRoot = roots.FirstOrDefault(x => x.name == "New Arcade Menu");
            if(!arcadeRoot)
            {
                Plugin.Logger.LogError("Unable to find the GameObject 'New Arcade Menu'!");
                return;
            }

            try
            {
                InitArcadeUI(arcadeRoot.transform);
            }
            catch(Exception e)
            {
                Plugin.Logger.LogError($"Failed to initialize arcade scene with error: {e.Message}\n    {e.StackTrace}");
            }
        }
    }


    private void Awake()
    {
        Manager.OnSceneLoaded += HandleSceneLoaded;
    }
}