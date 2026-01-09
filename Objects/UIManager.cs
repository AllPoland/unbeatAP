using System;
using System.Collections.Generic;
using System.Linq;
using Arcade.UI;
using Arcade.UI.AnimationSystem;
using Arcade.UI.MenuStates;
using HarmonyLib;
using TMPro;
using UBUI.Animation;
using UBUI.Archipelago;
using UBUI.Serialization;
using UNBEATAP.AP;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.SceneManagement;

namespace UNBEATAP.Objects;

public class UIManager : MonoBehaviour
{
    private const string disconnectText = "<mspace=11>//<mspace=17> </mspace><cspace=0.35em>disconnect.";
    private const string ArchipelagoConnectionScreen = "ArchipelagoConnectionScreen.prefab";

    private APConnectionScreen connectScreen;
    private Dictionary<(EArcadeMenuStates, EArcadeMenuStates), Dictionary<string, float>> transitionDelays = new Dictionary<(EArcadeMenuStates, EArcadeMenuStates), Dictionary<string, float>>();

    private ArchipelagoManager Manager => ArchipelagoManager.Instance;


    public void HandleConnectionError(FailConnectionReason reason)
    {
        connectScreen.CancelAndShowError(reason);
    }


    public void PlayTransition(EArcadeMenuStates oldState, EArcadeMenuStates newState)
    {
        if(!transitionDelays.TryGetValue((oldState, newState), out Dictionary<string, float> delays))
        {
            delays = new Dictionary<string, float>();
        }

        UIStateManager.SetState((UIState)oldState, (UIState)newState, delays);
    }


    private void GetTransitionDelays(Transform menuRoot)
    {
        Transform transitionRoot = menuRoot.GetChild(2);
        foreach(UITransition transition in transitionRoot.GetComponentsInChildren<UITransition>())
        {
            // Transition objects are named by their exit and enter states
            string[] states = transition.name.Split("-");
            if(states.Contains("UserProfile") || states.Contains("UserLeaderboard"))
            {
                // This one isn't real and it doesn't count and it can't hurt me
                return;
            }

            if(states.Length < 2)
            {
                Plugin.Logger.LogWarning($"Invalid UI states: {transition.name}");
                continue;
            }

            if(!Enum.TryParse(states[0], true, out EArcadeMenuStates exitState))
            {
                Plugin.Logger.LogWarning($"Unknown UI states: {transition.name}");
                continue;
            }

            if(!Enum.TryParse(states[1], true, out EArcadeMenuStates enterState))
            {
                Plugin.Logger.LogWarning($"Unknown UI states: {transition.name}");
                continue;
            }

            Traverse traverse = new Traverse(transition);
            List<UITransition.TransitionStep> steps = traverse.Field("animationTriggers").GetValue<List<UITransition.TransitionStep>>();
            float speed = traverse.Field("speed").GetValue<float>();

            Dictionary<string, float> delays = new Dictionary<string, float>();
            foreach(UITransition.TransitionStep step in steps)
            {
                delays[step.animation.name] = step.delay / speed;
            }

            transitionDelays[(exitState, enterState)] = delays;
        }
    }


    private void SetConnectionInfoAndConnect()
    {
        APConnectionInfo info = connectScreen.GetConnectionInfo();

        if(!int.TryParse(info.port, out int port))
        {
            port = 58008;
        }
        Plugin.SetConnectionInfo(info.ip, port, info.slot, info.pass);
        Manager.CreateClientAndConnect();
    }


    private void CreateConnectScreen(Transform mainMenu)
    {
        GameObject connectObject = PrefabInitializer.LoadAndInstantiatePrefab(ArchipelagoConnectionScreen, ArchipelagoManager.APUIBundle, mainMenu);
        connectScreen = connectObject.GetComponent<APConnectionScreen>();

        connectScreen.Init();
        connectScreen.SetConnectionInfo(Plugin.GetConnectionInfo());
    }


    private void InitArcadeUIConnected(Transform root)
    {
        Transform screenArea = root.GetChild(1);
        Transform mainScreens = screenArea.GetChild(1);
        Transform mainMenu = mainScreens.GetChild(1);

        Transform buttons = mainMenu.GetChild(2);
        Transform storyButtonContainer = buttons.GetChild(2);

        Transform textIdle = storyButtonContainer.GetChild(0).GetChild(0).GetChild(0);
        Transform textActive = textIdle.GetChild(0).GetChild(0);

        // Disable localizers so they don't overwrite our replacement text
        textIdle.gameObject.GetComponent<GameObjectLocalizer>().enabled = false;
        textActive.gameObject.GetComponent<GameObjectLocalizer>().enabled = false;

        textIdle.gameObject.GetComponent<TextMeshProUGUI>().SetText(disconnectText);
        textActive.gameObject.GetComponent<TextMeshProUGUI>().SetText(disconnectText);

        CreateConnectScreen(mainMenu);
        connectScreen.SetConnected();
        connectScreen.OnConnect.AddListener(() => Plugin.Client.DisconnectAndClose());
    }


    private void InitArcadeUIDisconnected(Transform root)
    {
        Transform screenArea = root.GetChild(1);
        Transform mainScreens = screenArea.GetChild(1);
        Transform mainMenu = mainScreens.GetChild(1);

        CreateConnectScreen(mainMenu);
        connectScreen.OnConnect.AddListener(SetConnectionInfoAndConnect);
    }


    private void InitArcadeUI(Transform root)
    {
        string paletteName = UIColorPaletteUpdater.SelectedPalette;
        if(MenuPaletteIndex.CachedDefaultIndex.TryGetPalette(paletteName, out MenuPaletteIndex.Palette palette))
        {
            Color[] colors = palette.palette?.colors;
            if(colors == null)
            {
                Manager.ColorManager.ResetColors();
            }
            else Manager.ColorManager.SetColors(colors);
        }
        else Manager.ColorManager.ResetColors();

        if(Plugin.Client.Connected)
        {
            InitArcadeUIConnected(root);
        }
        else InitArcadeUIDisconnected(root);
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
                Transform rootTransform = arcadeRoot.transform;
                GetTransitionDelays(rootTransform);
                InitArcadeUI(rootTransform);
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
        Client.OnFailConnect += HandleConnectionError;
    }
}