using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    SaveManager saveManager;
    AudioSource source;

    void Start()
    {
        DontDestroyOnLoad(this);

        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();

        source = GetComponent<AudioSource>();


        SaveManager.SettingsSaveData data = saveManager.getSettingsData();

        source.mute = data.muteMusic;
    }

    public void checkToggle()
    {
        SaveManager.SettingsSaveData data = saveManager.getSettingsData();

        source.mute = data.muteMusic;
    }
}
