using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    GameObject hovered;

    public GameObject crosshair;
    public GameObject pausePanel;
    CameraRotation cameraRotationScript;

    public GameObject settingsPanel;

    SaveManager saveManager;
    LoadManager loadManager;

    bool canPause = true;

    bool inventoryOpenLastFrame;

    GameObject player;
    Player playerScript;
    GameObject ship;

    GameObject fader;

    public Toggle muteMusicToggle;
    public Slider sensSlider;
    public Text sensText;
    public Toggle showTimerToggle;

    public GameObject timerObject;

    GameObject shipBlueprintPanel;
    GameObject shipBlueprintLeft;
    GameObject shipBlueprintMiddle;
    GameObject shipBlueprintRight;
    Text shipBlueprintText;

    GameObject speedrunBox;
    Text speedrunName;

    string processedDigSpots;

    GameObject debugLog;
    GameObject debugTab;

    Music musicScript;

    GameObject hatchPos;

    public BBoolean.BetterBoolean hasControl;

    Sprite[] loadingScreens;
    public Sprite loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5;

    IEnumerator pauseCooldown()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        canPause = true;
    }

    IEnumerator changeScene(int sceneNumber)
    {
        saveManager.currentLoadingScreen = loadingScreens[Random.Range(1, 5)];
        fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 0.2f).setIgnoreTimeScale(true);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 1, 0.2f).setIgnoreTimeScale(true);
        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(sceneNumber, LoadSceneMode.Single);
    }

    IEnumerator loadingScreen()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1;

        playerScript.stopWatch.Start();

        BBoolean.changeBoolean(hasControl, "Loading", false);

        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 0, 0.2f);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 0, 0.2f);
    }

    void Start()
    {
        // betterBoolean that controls if the player can look and move around \\

        hasControl = new BBoolean.BetterBoolean();

        string[] names = new string[] { "Paused", "Inventory", "Loading" };
        bool[] booleans = new bool[] { false, false, true };

        hasControl.names = names;
        hasControl.booleans = booleans;

        // Initialize loading screens \\

        loadingScreens = new Sprite[]
        {
            loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5
        };


        StartCoroutine(loadingScreen());

        // Get player and playerScript \\

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();

        // Get the ship object \\

        foreach(GameObject _object in GameObject.FindGameObjectsWithTag("Ship"))
            {
            if (_object.name == "MainShipCollision")
            {
                ship = _object;
                hatchPos = ship.GetComponent<Airship>().airshipObject.transform.Find("HatchPos").gameObject;
            }
        }

        // Grab a bunch of references \\

        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();
        musicScript = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<Music>();
        cameraRotationScript = Camera.main.GetComponent<CameraRotation>();

        shipBlueprintPanel = transform.Find("ShipBlueprint").gameObject;
        shipBlueprintLeft = shipBlueprintPanel.transform.Find("Left").gameObject;
        shipBlueprintMiddle = shipBlueprintPanel.transform.Find("Middle").gameObject;
        shipBlueprintRight = shipBlueprintPanel.transform.Find("Right").gameObject;
        shipBlueprintText = shipBlueprintPanel.transform.Find("Text").GetComponent<Text>();

        // Fade in the scene \\

        fader = GameObject.FindGameObjectWithTag("Fader");
        fader.GetComponent<Image>().enabled = true;
        fader.transform.Find("Text").GetComponent<Text>().enabled = true;

        fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        // Load settings \\

        SaveManager.SettingsSaveData data = saveManager.getSettingsData();
        muteMusicToggle.isOn = data.muteMusic;
        showTimerToggle.isOn = data.showTimer;
        sensSlider.value = data.sensitivity;
        sensText.text = data.sensitivity.ToString();

        // Debug mode stuff \\

        debugLog = transform.Find("Log").gameObject;
        debugTab = transform.Find("Debug").gameObject;

        if (data._01000100_01100101_01100010_01110101_01100111 == "01000110 01100001 01101100 01110011 01100101")
        {
            debugLog.SetActive(true);
            debugTab.SetActive(true);
        }

        // Put the cool text box at the top left if in a speedrun or the test scene \\

        SaveManager.SaveData sData = saveManager.getSaveData(loadManager.saveSlot);

        speedrunBox = transform.Find("Speedrun").gameObject;
        speedrunName = speedrunBox.GetComponentInChildren<Text>();

        if (loadManager.saveSlot != "1" && loadManager.saveSlot != "2" && loadManager.saveSlot != "3")
        {
            speedrunBox.transform.localPosition = new Vector3(-303.4f, 180.3f, 0);
            speedrunName.text = $"{loadManager.saveSlot} {sData.saveName}";
        }
    }

    void Update()
    {
        // F1 Toggle UI \\

        if (Input.GetKeyDown(KeyCode.F1))
        {
            gameObject.GetComponent<Canvas>().enabled = !gameObject.GetComponent<Canvas>().enabled;
            player.transform.Find("Main Camera/ItemCam").gameObject.SetActive(!player.transform.Find("Main Camera/ItemCam").gameObject.activeSelf);
        }

        // Pause Menu \\

        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            BBoolean.swapBoolean(hasControl, "Paused");
            canPause = false;
            StartCoroutine(pauseCooldown());
            checkPauseStuff();
        }

        // Check if inventory is open \\

        if (playerScript.inventoryOpen)
            BBoolean.changeBoolean(hasControl, "Inventory", true);
        else
            BBoolean.changeBoolean(hasControl, "Inventory", false);

        if (BBoolean.checkBoolean(hasControl, "Inventory") != inventoryOpenLastFrame)
            checkPauseStuff();

        inventoryOpenLastFrame = playerScript.inventoryOpen;


        // If in inventory, paused, or loading, then disable camera movement \\

        if (BBoolean.NAND(hasControl))
            cameraRotationScript.enabled = true;
        else
            cameraRotationScript.enabled = false;
    }

    void checkPauseStuff()
    {
        // If paused \\
        if (BBoolean.checkBoolean(hasControl, "Paused"))
        {
            Cursor.lockState = CursorLockMode.None;
            crosshair.SetActive(false);
            Cursor.visible = true;
            LeanTween.moveLocalY(pausePanel, 0, 0.5f).setIgnoreTimeScale(true);
            LeanTween.moveLocalX(settingsPanel, 300, 0.5f).setIgnoreTimeScale(true);
        }
        // If in inventory \\
        else if (BBoolean.checkBoolean(hasControl, "Inventory"))
        {
            crosshair.SetActive(false);
        }
        // If not paused or in inventory \\
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            crosshair.SetActive(true);
            Cursor.visible = false;
            LeanTween.moveLocalY(pausePanel, 350, 0.5f);
            LeanTween.moveLocalX(settingsPanel, 500, 0.5f);
        }
    }

    public void mouseEntered(BaseEventData data)
    {
        PointerEventData newCastedData = data as PointerEventData;
        hovered = newCastedData.pointerCurrentRaycast.gameObject;
        hovered.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
    }

    public void mouseExited(BaseEventData data)
    {
        hovered.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        hovered = null;
    }

    public void mouseClicked(BaseEventData data)
    {
        if (hovered != null)
        {
            if (hovered.name == "Resume" && canPause)
            {
                BBoolean.changeBoolean(hasControl, "Paused", false);
                canPause = false;
                StartCoroutine(pauseCooldown());
                checkPauseStuff();
            }
            if (hovered.name == "Save")
            {
                SaveGame();
            }
            if (hovered.name == "Quit")
            {
                StartCoroutine(changeScene(0));
            }
        }
    }

    void SaveGame()
    {
        var playerS = playerScript;
        SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);

        if (SceneManager.GetActiveScene().name == "World")
        {
            GameObject[] digSpots = GameObject.FindGameObjectsWithTag("Dig");
            string[] spotStrings = new string[digSpots.Length];
            foreach (GameObject spot in digSpots)
            {
                string spotDug = spot.name.Split(' ')[0];
                int spotNumber = int.Parse(spot.name.Split(' ')[2]);
                spotStrings[spotNumber] = spotDug;
            }

            foreach (string s in spotStrings)
            {
                processedDigSpots = (processedDigSpots + " " + s);
            }

            processedDigSpots = processedDigSpots.Substring(1);

            saveManager.CreateSaveData(loadManager.saveSlot, data.saveName,
                                        "World", player.transform.position, player.transform.eulerAngles.y,
                                        Camera.main.transform.eulerAngles.x,ship.transform.position,
                                        ship.transform.eulerAngles, playerS.itemEquipped,
                                        playerS.leftUnlocked, playerS.topUnlocked, playerS.rightUnlocked,
                                        (playerS.stopWatch.ElapsedMilliseconds+ playerS.addedTime).ToString(),
                                        playerS.shipBlueprintAmount, processedDigSpots, hatchPos.transform.position,
                                        hatchPos.transform.eulerAngles.y,
                                        Camera.main.transform.GetComponent<CameraRotation>().cameraValue,
                                        playerS.leftUnlocked2, playerS.topUnlocked2, playerS.rightUnlocked2);
        }
        else
        {
            saveManager.CreateSaveData(loadManager.saveSlot, data.saveName,
                                        SceneManager.GetActiveScene().name,
                                        player.transform.position, player.transform.eulerAngles.y,
                                        Camera.main.transform.eulerAngles.x,
                                        new Vector3(data.shipX, data.shipY, data.shipZ),
                                        new Vector3(data.shipRotX, data.shipRotY, data.shipRotZ),
                                        playerS.itemEquipped, playerS.leftUnlocked, playerS.topUnlocked, playerS.rightUnlocked,
                                        (playerS.stopWatch.ElapsedMilliseconds + playerS.addedTime).ToString(),
                                        playerS.shipBlueprintAmount, data.digSpots, new Vector3(data.hatchX,
                                        data.hatchY, data.hatchZ), data.hatchR,
                                        Camera.main.transform.GetComponent<CameraRotation>().cameraValue,
                                        playerS.leftUnlocked2, playerS.topUnlocked2, playerS.rightUnlocked2);
        }

        processedDigSpots = "";
    }

    public void MusicToggleChanged(bool value)
    {
        muteMusicToggle.isOn = value;

        SaveManager.SettingsSaveData data = saveManager.getSettingsData();

        if (saveManager != null)
            saveManager.CreateSettingsData(muteMusicToggle.isOn, showTimerToggle.isOn, float.Parse(sensText.text),
                                       data._01000100_01100101_01100010_01110101_01100111);

        musicScript.checkToggle();
    }

    public void TimerToggleChanged(bool value)
    {
        if (value == true)
        {
            showTimerToggle.isOn = true;
            LeanTween.moveLocalY(timerObject, 208, 0.2f);
        }
        else
        {
            showTimerToggle.isOn = false;
            LeanTween.moveLocalY(timerObject, 250, 0.2f);
        }

        SaveManager.SettingsSaveData data = saveManager.getSettingsData();

        if (saveManager != null)
            saveManager.CreateSettingsData(muteMusicToggle.isOn, showTimerToggle.isOn, float.Parse(sensText.text),
                                       data._01000100_01100101_01100010_01110101_01100111);
    }

    public void SensitivitySliderChanged(float value)
    {
        value = Round(value, 1);
        sensText.text = value.ToString();
        PlayerPrefs.SetFloat("Sens", value);
        if (Camera.main.transform.GetComponent<CameraRotation>())
            Camera.main.transform.GetComponent<CameraRotation>().sensitivity = value;

        SaveManager.SettingsSaveData data = saveManager.getSettingsData();

        if (saveManager != null)
            saveManager.CreateSettingsData(muteMusicToggle.isOn, showTimerToggle.isOn, float.Parse(sensText.text),
                                       data._01000100_01100101_01100010_01110101_01100111);
    }
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    public IEnumerator collectShipBlueprint(int value)
    {
        shipBlueprintText.text = $"{value}/3 Blueprint Pieces Collected";

        if (value == 3)
        {
            shipBlueprintLeft.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            shipBlueprintMiddle.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

            LeanTween.moveLocalX(shipBlueprintPanel, -296, 1);

            yield return new WaitForSecondsRealtime(1);

            LeanTween.alpha(shipBlueprintRight.GetComponent<RectTransform>(), 1, 1).setEaseInBounce();

            yield return new WaitForSecondsRealtime(3);

            LeanTween.moveLocalX(shipBlueprintPanel, -514, 1);
        }
        else if (value == 2)
        {
            shipBlueprintLeft.GetComponent<Image>().color = new Color32(255, 255, 255, 255);

            LeanTween.moveLocalX(shipBlueprintPanel, -296, 1);

            yield return new WaitForSecondsRealtime(1);

            LeanTween.alpha(shipBlueprintMiddle.GetComponent<RectTransform>(), 1, 1).setEaseInBounce();

            yield return new WaitForSecondsRealtime(3);

            LeanTween.moveLocalX(shipBlueprintPanel, -514, 1);
        }
        else
        {
            LeanTween.moveLocalX(shipBlueprintPanel, -296, 1);

            yield return new WaitForSecondsRealtime(1);

            LeanTween.alpha(shipBlueprintLeft.GetComponent<RectTransform>(), 1, 1).setEaseInBounce();

            yield return new WaitForSecondsRealtime(3);

            LeanTween.moveLocalX(shipBlueprintPanel, -514, 1);
        }
    }
}
