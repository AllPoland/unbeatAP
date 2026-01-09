using System;
using System.Linq;
using Arcade.UI;
using TMPro;
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

    private ArchipelagoManager Manager => ArchipelagoManager.Instance;


    public void HandleConnectionError(FailConnectionReason reason)
    {
        connectScreen.CancelAndShowError(reason);
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
    }


    private void InitArcadeUIDisconnected(Transform root)
    {
        Transform screenArea = root.GetChild(1);
        Transform mainScreens = screenArea.GetChild(1);
        Transform mainMenu = mainScreens.GetChild(1);

        GameObject connectObject = PrefabInitializer.LoadAndInstantiatePrefab(ArchipelagoConnectionScreen, ArchipelagoManager.APUIBundle, mainMenu);
        connectScreen = connectObject.GetComponent<APConnectionScreen>();

        connectScreen.Init();
        connectScreen.SetConnectionInfo(Plugin.GetConnectionInfo());
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
        Client.OnFailConnect += HandleConnectionError;
    }
}