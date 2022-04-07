using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerManager : MonoBehaviour
{
    public GameObject musicManager;
    public GameObject discordManger;
    public GameObject saveManager;
    public GameObject loadManager;

    void Awake()
    {
        if (!GameObject.FindGameObjectWithTag("MusicManager"))
            Instantiate(musicManager);
        if (!GameObject.FindGameObjectWithTag("DiscordManager"))
            Instantiate(discordManger);
        if (!GameObject.FindGameObjectWithTag("SaveManager"))
            Instantiate(saveManager);
        if (!GameObject.FindGameObjectWithTag("LoadManager"))
            Instantiate(loadManager);
    }
}
