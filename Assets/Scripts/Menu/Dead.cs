using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dead : MonoBehaviour
{

    GameObject fader;
    DiscordThing discord;
    GameObject hovered;
    SaveManager saveManager;
    LoadManager loadManager;

    Sprite[] loadingScreens;
    public Sprite loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5;

    IEnumerator changeScene(string sceneName)
    {
        saveManager.currentLoadingScreen = loadingScreens[Random.Range(1, 5)];
        fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 0.2f).setIgnoreTimeScale(true);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 1, 0.2f).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(0.2f);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    IEnumerator loadingScreen()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(5);
        Time.timeScale = 1;

        LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 0, 0.2f);
        LeanTween.alpha(fader.transform.Find("Text").GetComponent<Text>().rectTransform, 0, 0.2f);
    }

    void Start()
    {
        discord = GameObject.FindGameObjectWithTag("DiscordManager").GetComponent<DiscordThing>();
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        fader = GameObject.FindGameObjectWithTag("Fader");
        fader.GetComponent<Image>().enabled = true;
        fader.transform.Find("Text").GetComponent<Text>().enabled = true;

        fader.GetComponent<Image>().sprite = saveManager.currentLoadingScreen;

        discord.UpdateActivity("Dead", "Regretting Life Choices");

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Initialize loading screens \\

        loadingScreens = new Sprite[]
        {
            loadingScreen1, loadingScreen2, loadingScreen3, loadingScreen4, loadingScreen5
        };

        StartCoroutine(loadingScreen());
    }

    public void mouseEntered(BaseEventData data)
    {
        PointerEventData newCastedData = data as PointerEventData;
        hovered = newCastedData.pointerCurrentRaycast.gameObject;
        LeanTween.moveLocalX(hovered, -10, 0.05f);
    }

    public void mouseExited(BaseEventData data)
    {
        LeanTween.moveLocalX(hovered, 0, 0.05f);
        hovered = null;
    }

    public void mouseClicked(BaseEventData data)
    {
        if (hovered.name == "Play")
        {
            LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 1);
            StartCoroutine(changeScene("World"));
        }
        if (hovered.name == "Quit")
        {
            LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 1);
            StartCoroutine(changeScene("Menu"));
        }
        if (hovered.name == "Spawn")
        {
            SaveManager.SaveData saveData = saveManager.getSaveData("Dead");

            saveManager.CreateSaveData(loadManager.saveSlot, saveData.saveName, "World",
                                       new Vector3(saveData.playerX, saveData.playerY, saveData.playerZ), saveData.playerRotY,
                                       saveData.cameraRotX, new Vector3(saveData.shipX, saveData.shipY, saveData.shipZ),
                                       new Vector3(saveData.shipRotX, saveData.shipRotY, saveData.shipRotZ), saveData.itemEquipped,
                                       saveData.leftUnlocked, saveData.topUnlocked, saveData.rightUnlocked, saveData.timeElapsed,
                                       saveData.shipBlueprintAmount, saveData.digSpots, new Vector3(saveData.hatchX,
                                       saveData.hatchY, saveData.hatchZ), saveData.hatchR, saveData.cameraValue,
                                       saveData.leftUnlocked2, saveData.topUnlocked2, saveData.rightUnlocked2);

            LeanTween.alpha(fader.GetComponent<Image>().rectTransform, 1, 1);
            StartCoroutine(changeScene("World"));
        }
    }
}
