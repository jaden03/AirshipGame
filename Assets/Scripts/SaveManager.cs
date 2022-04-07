using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class SaveManager : MonoBehaviour
{
    LoadManager loadManager;

    public Sprite currentLoadingScreen;
    public bool pastStartup = false;

    void Start()
    {
        DontDestroyOnLoad(this);
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();
    }

    [System.Serializable]
    public class SaveData
    {
        public string saveName;

        public string sceneName;

        public float playerX, playerY, playerZ;
        public float playerRotY;
        public float cameraRotX;

        public float shipX, shipY, shipZ;
        public float shipRotX, shipRotY, shipRotZ;

        public string itemEquipped;
        public bool leftUnlocked;
        public bool topUnlocked;
        public bool rightUnlocked;

        public string timeElapsed;

        public int shipBlueprintAmount;

        public string digSpots;

        public bool shipInArea;
        public bool shipUpgraded;

        public float timeOfDay;

        public float hatchX, hatchY, hatchZ, hatchR;

        public float cameraValue;

        public bool leftUnlocked2, topUnlocked2, rightUnlocked2;
    }

    [System.Serializable]
    public class SettingsSaveData
    {
        public bool muteMusic;
        public bool showTimer;
        public float sensitivity;
        public string _01000100_01100101_01100010_01110101_01100111;
    }

    [System.Serializable]
    public class SpeedrunSaves
    {
        public string shovel;
        public string blueprint;
    }

    public void createNewSave(string slotNumber, string saveName)
    {
        SaveData data = new SaveData();

        data.saveName = saveName;

        data.sceneName = "Island1Dungeon";

        data.playerX = -20f;
        data.playerY = -5f;
        data.playerZ = -9f;

        data.playerRotY = 0;

        data.cameraRotX = 0;

        data.shipX = -2.5f;
        data.shipY = 12.67f;
        data.shipZ = -31.5f;

        data.shipRotX = 0;
        data.shipRotY = -76.992f;
        data.shipRotZ = 0;

        data.itemEquipped = "None";
        data.leftUnlocked = false;
        data.topUnlocked = false;
        data.rightUnlocked = false;

        data.timeElapsed = "0";

        data.shipBlueprintAmount = 0;

        data.digSpots = "3 3 3";

        data.shipInArea = false;

        data.shipUpgraded = false;

        data.timeOfDay = 6;

        data.cameraValue = 0;

        data.leftUnlocked2 = false;
        data.topUnlocked2 = false;
        data.rightUnlocked2 = false;

        SaveGame(slotNumber, data);
    }

    // TEST SCENE \\
    public void loadTestScene(string slotNumber, string saveName)
    {
        SaveData data = new SaveData();

        data.saveName = saveName;

        data.sceneName = "Test";

        data.playerX = 0;
        data.playerY = 0;
        data.playerZ = 0;

        data.playerRotY = 0;

        data.cameraRotX = 0;

        data.shipX = -2.5f;
        data.shipY = 12.67f;
        data.shipZ = -31.5f;

        data.shipRotX = 0;
        data.shipRotY = -76.992f;
        data.shipRotZ = 0;

        data.itemEquipped = "None";
        data.leftUnlocked = true;
        data.topUnlocked = true;
        data.rightUnlocked = true;

        data.timeElapsed = "0";

        data.shipBlueprintAmount = 0;

        data.digSpots = "3 3 3";

        data.shipInArea = false;

        data.shipUpgraded = false;

        data.timeOfDay = 6;

        data.cameraValue = 0;

        data.leftUnlocked2 = true;
        data.topUnlocked2 = true;
        data.rightUnlocked2 = true;

        SaveGame(slotNumber, data);
    }

    // ------------ \\

    public void CreateSaveData(string slotNumber, string saveName, string sceneName, Vector3 playerPos,
                               float playerRotY, float cameraRotX, Vector3 shipPos, Vector3 shipRot,
                               string itemEquipped, bool leftUnlocked, bool topUnlocked, bool rightUnlocked,
                               string timeElapsed, int shipBlueprintAmount, string digSpots, Vector3 hatchPos,
                               float hatchR, float cameraValue, bool leftUnlocked2, bool topUnlocked2, bool rightUnlocked2)
    {
        SaveData data = new SaveData();

        SaveData loadedData = getSaveData(loadManager.saveSlot);

        data.saveName = saveName;

        data.sceneName = sceneName;

        data.playerX = playerPos.x;
        data.playerY = playerPos.y;
        data.playerZ = playerPos.z;

        data.playerRotY = playerRotY;

        data.cameraRotX = cameraRotX;

        data.shipX = shipPos.x;
        data.shipY = shipPos.y;
        data.shipZ = shipPos.z;

        data.shipRotX = shipRot.x;
        data.shipRotY = shipRot.y;
        data.shipRotZ = shipRot.z;

        data.itemEquipped = itemEquipped;
        data.leftUnlocked = leftUnlocked;
        data.topUnlocked = topUnlocked;
        data.rightUnlocked = rightUnlocked;

        data.timeElapsed = timeElapsed;

        data.shipBlueprintAmount = shipBlueprintAmount;

        data.digSpots = digSpots;

        data.shipInArea = loadedData.shipInArea;

        data.shipUpgraded = loadedData.shipUpgraded;

        data.timeOfDay = transform.GetComponent<TimeOfDay>().time;

        data.hatchX = hatchPos.x;
        data.hatchY = hatchPos.y;
        data.hatchZ = hatchPos.z;
        data.hatchR = hatchR;

        data.cameraValue = cameraValue;

        data.leftUnlocked2 = leftUnlocked2;
        data.topUnlocked2 = topUnlocked2;
        data.rightUnlocked2 = rightUnlocked2;

        SaveGame(slotNumber, data);
    }

    public void updateShipInArea(bool shipInArea)
    {
        SaveData data = getSaveData(loadManager.saveSlot);

        data.shipInArea = shipInArea;

        SaveGame(loadManager.saveSlot, data);
    }

    public void upgradeShip()
    {
        SaveData data = getSaveData(loadManager.saveSlot);

        data.shipUpgraded = true;

        SaveGame(loadManager.saveSlot, data);
    }

    public void CreateSettingsData(bool muteMusic, bool showTimer, float sensitivity,
                                   string _01000100_01100101_01100010_01110101_01100111)
    {
        SettingsSaveData data = new SettingsSaveData();

        data.muteMusic = muteMusic;
        data.showTimer = showTimer;

        data.sensitivity = sensitivity;

        data._01000100_01100101_01100010_01110101_01100111 = _01000100_01100101_01100010_01110101_01100111;

        SaveSettings(data);
    }

    public void ToggleDebugMode()
    {
        SettingsSaveData data = getSettingsData();

        if (data._01000100_01100101_01100010_01110101_01100111 == "01000110 01100001 01101100 01110011 01100101")
            CreateSettingsData(data.muteMusic, data.showTimer, data.sensitivity,
            "01010100 01110010 01110101 01100101");
        else
            CreateSettingsData(data.muteMusic, data.showTimer, data.sensitivity,
            "01000110 01100001 01101100 01110011 01100101");
    }

    public void SaveSpeedrun(string shovel, string blueprint)
    {
        SpeedrunSaves data = new SpeedrunSaves();

        if (shovel != "00:00:00:000")
        {
            data.shovel = shovel;
            data.blueprint = getSpeedruns().blueprint;
        }
        if (blueprint != "00:00:00:000")
        {
            data.shovel = getSpeedruns().shovel;
            data.blueprint = blueprint;
        }

        string Data = JsonUtility.ToJson(data, true);

        Data = EncryptorDecryptor.EncryptDecrypt(Data);

        File.WriteAllText(Application.persistentDataPath + $"/speedruns_{Application.version}", Data);
    }

    public void ResetRuns()
    {
        SpeedrunSaves data = new SpeedrunSaves();

        data.shovel = "00:00:00:000";
        data.blueprint = "00:00:00:000";

        string Data = JsonUtility.ToJson(data, true);

        Data = EncryptorDecryptor.EncryptDecrypt(Data);

        File.WriteAllText(Application.persistentDataPath + $"/speedruns_{Application.version}", Data);
    }

    void SaveGame(string slotNumber, SaveData data)
    {
        string _data = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + $"/{slotNumber}.json", _data);
    }

    void SaveSettings(SettingsSaveData data)
    {
        string _data = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.persistentDataPath + $"/settings.json", _data);
    }

    public SaveData getSaveData(string slotNumber)
    {
        string json = File.ReadAllText(Application.persistentDataPath + $"/{slotNumber}.json");
        return JsonUtility.FromJson<SaveData>(json);
    }

    public SettingsSaveData getSettingsData()
    {
        string json = File.ReadAllText(Application.persistentDataPath + $"/settings.json");
        return JsonUtility.FromJson<SettingsSaveData>(json);
    }

    public SpeedrunSaves getSpeedruns()
    {
        if (File.Exists(Application.persistentDataPath + $"/speedruns_{Application.version}"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + $"/speedruns_{Application.version}");

            string Data = EncryptorDecryptor.EncryptDecrypt(json);

            SpeedrunSaves saves = JsonUtility.FromJson<SpeedrunSaves>(Data);

            return saves;
        }
        else
            return null;
    }
}
