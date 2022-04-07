using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Menu : MonoBehaviour
{
    public GameObject hovered = null;

    string currentPanel = "playQuit";

    string[] debugCode;
    int indexOnCode = 0;
    bool canInputCode = false;

    public GameObject playQuitPanel;
    public GameObject slotsPanel;
    public GameObject createPanel;
    public GameObject settingPanel;
    public GameObject speedrunPanel;

    public GameObject createButton;
    public Text saveNameField;

    public Text slot1Text;
    public Text slot2Text;
    public Text slot3Text;

    string slotNumber;

    SaveManager saveManager;

    LoadManager loadManager;

    DiscordThing discord;

    GameObject fader;

    public Toggle muteMusicToggle;
    public Toggle showTimerToggle;
    public Toggle showTimerToggle2;
    public Slider sensitivitySlider;
    public Text sensText;

    bool changeLogShowing = false;
    public GameObject changeLog;

    GameObject debugNotification;

    Music musicScript;

    Sprite[] loadingScreens;
    public Sprite loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5;

    IEnumerator changeScene(string sceneName)
    {
        saveManager.currentLoadingScreen = loadingScreens[Random.Range(1, 5)];
        fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator loadingScreen()
    {
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1;
        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 0, 0.2f);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 0, 0.2f);
    }

    void Start()
    {
        // Cursor and fader stuff \\

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        fader = GameObject.FindGameObjectWithTag("Fader");
        fader.GetComponent<Image>().enabled = true;
        fader.transform.Find("Text").GetComponent<Text>().enabled = true;

        // Initialize loading screens \\

        loadingScreens = new Sprite[]
        {
            loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5
        };       

        // References \\

        discord = GameObject.FindGameObjectWithTag("DiscordManager").GetComponent<DiscordThing>();
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        musicScript = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<Music>();

        // If no loading screen yet, then made one the loading screen \\

        if (saveManager.pastStartup == false)
        {
            saveManager.pastStartup = true;
            saveManager.currentLoadingScreen = loadingScreens[Random.Range(1, 5)];
            fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;
        }
        else
            fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        StartCoroutine(loadingScreen());

        // ------------------------------------------------------- \\

        if (File.Exists(Application.persistentDataPath + "/1.json"))
        {
            slot1Text.text = saveManager.getSaveData("1").saveName;
        }
        if (File.Exists(Application.persistentDataPath + "/2.json"))
        {
            slot2Text.text = saveManager.getSaveData("2").saveName;
        }
        if (File.Exists(Application.persistentDataPath + "/3.json"))
        {
            slot3Text.text = saveManager.getSaveData("3").saveName;
        }

        discord.UpdateActivity("In Menus", "Staring at the play button");

        if (!File.Exists(Application.persistentDataPath + "/settings.json"))
        {
            saveManager.CreateSettingsData(false, true, 1, "01001110 01101111");
        }
        else
        {
            SaveManager.SettingsSaveData data = saveManager.getSettingsData();
            muteMusicToggle.isOn = data.muteMusic;
            showTimerToggle.isOn = data.showTimer;
            showTimerToggle2.isOn = data.showTimer;
            sensitivitySlider.value = data.sensitivity;
            sensText.text = data.sensitivity.ToString();
        }

        debugCode = new string[] { "2", "3", "4", "2" };
        debugNotification = transform.Find("DebugNotification").gameObject;
    }

    IEnumerator showDebugModeThing(bool value)
    {
        if (value == true)
            debugNotification.GetComponentInChildren<Text>().text = "Debug Mode Enabled!";
        else
            debugNotification.GetComponentInChildren<Text>().text = "Debug Mode Disabled!";

        LeanTween.moveLocalY(debugNotification, 200, 0.2f);
        yield return new WaitForSecondsRealtime(2);
        LeanTween.moveLocalY(debugNotification, 260, 0.2f);

        canInputCode = true;
    }

    void Update()
    {
        if (string.IsNullOrWhiteSpace(saveNameField.text))
        {
            createButton.GetComponent<Image>().raycastTarget = false;
            createButton.GetComponent<Image>().color = new Color32(150, 150, 150, 255);
        }
        else
        {
            createButton.GetComponent<Image>().raycastTarget = true;
            createButton.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        if (canInputCode)
        {
            if (Input.anyKeyDown)
            {
                if (Input.GetKeyDown(debugCode[indexOnCode]))
                    indexOnCode++;
                else
                {
                    indexOnCode = 0;
                    canInputCode = false;
                }
            }
        }

        if (indexOnCode == debugCode.Length)
        {
            indexOnCode = 0;

            saveManager.ToggleDebugMode();

            canInputCode = false;
            bool value;

            SaveManager.SettingsSaveData data = saveManager.getSettingsData();

            if (data._01000100_01100101_01100010_01110101_01100111 == "01000110 01100001 01101100 01110011 01100101")
                value = true;
            else
                value = false;

            StartCoroutine(showDebugModeThing(value));
        }
    }

    public void mouseEntered(BaseEventData data)
    {
        PointerEventData newCastedData = data as PointerEventData;
        hovered = newCastedData.pointerCurrentRaycast.gameObject;
        if (hovered.name != "Delete" && hovered.name != "ChangelogTab" && hovered.name != "TestScene")
            LeanTween.moveLocalX(hovered, -10, 0.05f);
        else if (hovered.name == "Delete")
            hovered.GetComponent<Image>().color = new Color32(255, 0, 0, 255);
        else if (hovered.name == "ChangelogTab" || hovered.name == "TestScene")
            hovered.GetComponent<Image>().color = new Color32(200, 200, 200, 255);
    }

    public void mouseExited(BaseEventData data)
    {
        if (hovered != null)
        {
            if (hovered.name != "Delete" && hovered.name != "ChangelogTab" && hovered.name != "TestScene")
                LeanTween.moveLocalX(hovered, 0, 0.05f);
            else
                hovered.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            hovered = null;
        }
    }

    public void mouseClicked(BaseEventData data)
    {
        if (hovered != null)
        {
            PointerEventData newCastedData = data as PointerEventData;
            if (hovered.name == "Play")
            {
                LeanTween.moveLocalX(playQuitPanel, 500, 0.2f);
                LeanTween.moveLocalX(slotsPanel, 300, 0.2f);
                currentPanel = "slotsPanel";
                discord.UpdateActivity("In Menus", "Staring at save slots");
            }

            if (hovered.name == "Speedruns")
            {
                LeanTween.moveLocalX(playQuitPanel, 500, 0.2f);
                LeanTween.moveLocalX(speedrunPanel, 300, 0.2f);
                currentPanel = "speedrunPanel";
                discord.UpdateActivity("In Menus", "About to try an epic speedrun");
            }

            if (hovered.name == "Settings")
            {
                LeanTween.moveLocalX(playQuitPanel, 500, 0.2f);
                LeanTween.moveLocalX(settingPanel, 300, 0.2f);
                currentPanel = "settingsPanel";
                discord.UpdateActivity("In Menus", "Staring at settings");
            }

            if (hovered.name == "Quit")
            {
                Application.Quit();
            }

            if (hovered.name == "ChangelogTab" && !LeanTween.isTweening(changeLog))
            {
                if (changeLogShowing)
                {
                    LeanTween.moveLocalX(changeLog, 527, 0.2f);
                }
                else
                {
                    LeanTween.moveLocalX(changeLog, 260, 0.2f);
                }
                changeLogShowing = !changeLogShowing;
            }

            if (hovered.name == "Back")
            {
                if (currentPanel == "slotsPanel")
                {
                    LeanTween.moveLocalX(playQuitPanel, 300, 0.2f);
                    LeanTween.moveLocalX(slotsPanel, 500, 0.2f);
                    currentPanel = "playQuit";
                    discord.UpdateActivity("In Menus", "Staring at the play button");
                }

                if (currentPanel == "createPanel")
                {
                    LeanTween.moveLocalX(createPanel, 500, 0.2f);
                    LeanTween.moveLocalX(slotsPanel, 300, 0.2f);
                    loadManager.saveSlot = null;
                    currentPanel = "slotsPanel";
                    discord.UpdateActivity("In Menus", "Staring at save slots");
                }

                if (currentPanel == "settingsPanel")
                {
                    LeanTween.moveLocalX(playQuitPanel, 300, 0.2f);
                    LeanTween.moveLocalX(settingPanel, 500, 0.2f);
                    currentPanel = "playQuit";
                    discord.UpdateActivity("In Menus", "Staring at the play button");
                }

                if (currentPanel == "speedrunPanel")
                {
                    LeanTween.moveLocalX(playQuitPanel, 300, 0.2f);
                    LeanTween.moveLocalX(speedrunPanel, 500, 0.2f);
                    currentPanel = "playQuit";
                    discord.UpdateActivity("In Menus", "Staring at the play button");
                }
            }

            if (hovered.name.Split(' ')[0] == "Slot")
            {
                if (hovered.name.Split(' ')[1] == "1")
                {
                    loadManager.saveSlot = "1";
                    if (hovered.GetComponentInChildren<Text>().text == "Empty")
                    {
                        slotNumber = "1";
                        LeanTween.moveLocalX(slotsPanel, 500, 0.2f);
                        LeanTween.moveLocalX(createPanel, 300, 0.2f);
                        currentPanel = "createPanel";
                        discord.UpdateActivity("In Menus", "Creating a save in slot 1");
                    }
                    else
                    {
                        loadIntoGame(saveManager.getSaveData("1").sceneName);
                    }
                }
                if (hovered.name.Split(' ')[1] == "2")
                {
                    loadManager.saveSlot = "2";
                    if (hovered.GetComponentInChildren<Text>().text == "Empty")
                    {
                        slotNumber = "2";
                        LeanTween.moveLocalX(slotsPanel, 500, 0.2f);
                        LeanTween.moveLocalX(createPanel, 300, 0.2f);
                        currentPanel = "createPanel";
                        discord.UpdateActivity("In Menus", "Creating a save in slot 2");
                    }
                    else
                    {
                        loadIntoGame(saveManager.getSaveData("2").sceneName);
                    }
                }
                if (hovered.name.Split(' ')[1] == "3")
                {
                    loadManager.saveSlot = "3";
                    if (hovered.GetComponentInChildren<Text>().text == "Empty")
                    {
                        slotNumber = "3";
                        LeanTween.moveLocalX(slotsPanel, 500, 0.2f);
                        LeanTween.moveLocalX(createPanel, 300, 0.2f);
                        currentPanel = "createPanel";
                        discord.UpdateActivity("In Menus", "Creating a save in slot 3");
                    }
                    else
                    {
                        loadIntoGame(saveManager.getSaveData("3").sceneName);
                    }
                }
            }

            if (hovered.name == "Create")
            {
                saveManager.createNewSave(slotNumber, saveNameField.text);

                loadIntoGame("Island1Dungeon");
            }

            if (hovered.name == "Delete")
            {
                if (hovered.transform.localPosition.y == 122)
                {
                    File.Delete(Application.persistentDataPath + "/1.json");
                    slot1Text.text = "Empty";
                }
                if (hovered.transform.localPosition.y == 68)
                {
                    File.Delete(Application.persistentDataPath + "/2.json");
                    slot2Text.text = "Empty";
                }
                if (hovered.transform.localPosition.y == 14)
                {
                    File.Delete(Application.persistentDataPath + "/3.json");
                    slot3Text.text = "Empty";
                }
            }

            // TEST SCENE \\

            if (hovered.name == "TestScene")
            {
                loadManager.saveSlot = "Test";
                saveManager.loadTestScene("Test", "Scene");
                loadIntoGame("Test");
            }
        }
    }   

    public void ToggleDebugMode()
    {
        indexOnCode = 0;
        canInputCode = true;
    }

    public void MusicToggleChanged(bool value)
    {
        SaveManager.SettingsSaveData data = saveManager.getSettingsData();
        muteMusicToggle.isOn = value;
        saveManager.CreateSettingsData(muteMusicToggle.isOn, showTimerToggle.isOn, float.Parse(sensText.text),
                                       data._01000100_01100101_01100010_01110101_01100111);

        musicScript.checkToggle();
    }

    void loadIntoGame(string sceneName)
    {
        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 0.2f);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 1, 0.2f);
        StartCoroutine(changeScene(sceneName));
    }

    public void TimerToggleChanged(bool value)
    {
        SaveManager.SettingsSaveData data = saveManager.getSettingsData();
        showTimerToggle.isOn = value;
        showTimerToggle2.isOn = value;
        saveManager.CreateSettingsData(muteMusicToggle.isOn, showTimerToggle.isOn, float.Parse(sensText.text),
                                       data._01000100_01100101_01100010_01110101_01100111);
    }

    public void SensitivitySliderChanged(float value)
    {
        SaveManager.SettingsSaveData data = saveManager.getSettingsData();
        value = Round(value, 1);
        sensText.text = value.ToString();
        saveManager.CreateSettingsData(muteMusicToggle.isOn, showTimerToggle.isOn, float.Parse(sensText.text),
                                       data._01000100_01100101_01100010_01110101_01100111);
    }
    public static float Round(float value, int digits)
    {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }
}
