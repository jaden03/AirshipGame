using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    string saveName = null;
    public string saveSlot = null;

    SaveManager saveManager;

    GameObject player;
    GameObject ship;
    GameObject shipObject;

    SaveManager.SaveData data = new SaveManager.SaveData();

    void Start()
    {
        DontDestroyOnLoad(this);
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    void ChangedActiveScene(Scene current, Scene next)
    {
        data = saveManager.getSaveData(saveSlot);

        GameObject.FindGameObjectWithTag("SaveManager").GetComponent<TimeOfDay>().time = data.timeOfDay;

        if (next.name == "World" && !string.IsNullOrWhiteSpace(saveSlot))
        {
            player = GameObject.FindGameObjectWithTag("Player");

            foreach (GameObject _object in GameObject.FindGameObjectsWithTag("Ship"))
            {
                if (_object.name == "MainShipCollision")
                {
                    ship = _object;
                    shipObject = ship.GetComponent<Airship>().airshipObject;
                }
            }

            saveName = data.saveName;

            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);

            player.transform.eulerAngles = new Vector3(0, data.playerRotY, 0);

            Camera.main.transform.localEulerAngles = new Vector3(data.cameraRotX, 0, 0);

            ship.transform.position = new Vector3(data.shipX, data.shipY, data.shipZ);

            ship.transform.eulerAngles = new Vector3(data.shipRotX, data.shipRotY, data.shipRotZ);

            if (data.shipUpgraded == true)
            {
                shipObject.transform.Find("NonUpgraded").gameObject.SetActive(false);
            }
            else
            {
                shipObject.transform.Find("Upgraded").gameObject.SetActive(false);
                ship.transform.Find("Upgraded").gameObject.SetActive(false);
            }

            player.GetComponent<Player>().itemEquipped = data.itemEquipped;
            player.GetComponent<Player>().leftUnlocked = data.leftUnlocked;
            player.GetComponent<Player>().topUnlocked = data.topUnlocked;
            player.GetComponent<Player>().rightUnlocked = data.rightUnlocked;

            player.GetComponent<Player>().leftUnlocked2 = data.leftUnlocked2;
            player.GetComponent<Player>().topUnlocked2 = data.topUnlocked2;
            player.GetComponent<Player>().rightUnlocked2 = data.rightUnlocked2;

            player.GetComponent<Player>().addedTime = int.Parse(data.timeElapsed);

            player.GetComponent<Player>().shipBlueprintAmount = data.shipBlueprintAmount;

            GameObject[] digSpots = GameObject.FindGameObjectsWithTag("Dig");

            string[] spots = data.digSpots.Split(' ');

            foreach (GameObject _spot in digSpots)
            {
                for (int i = 0; i < spots.Length; i++)
                {
                    if (int.Parse(_spot.name.Split(' ')[2]) == i)
                    {
                        _spot.name = spots[i] + " " + _spot.name.Split(' ')[1] + " " + _spot.name.Split(' ')[2] + " " + _spot.name.Split(' ')[3];

                        if (int.Parse(spots[i]) == 2)
                            _spot.transform.localScale = _spot.transform.localScale * 0.8f;
                        else if (int.Parse(spots[i]) == 1)
                        {
                            _spot.transform.localScale = _spot.transform.localScale * 0.8f;
                            _spot.transform.localScale = _spot.transform.localScale * 0.8f;
                        }
                        else if (int.Parse(spots[i]) == 0)
                            _spot.transform.localScale = Vector3.zero;
                    }
                }
            }

            Camera.main.GetComponent<CameraRotation>().cameraValue = data.cameraValue;
        }
        else if (!string.IsNullOrWhiteSpace(saveSlot) && next.name != "Menu" && !string.IsNullOrWhiteSpace(saveSlot) && next.name != "Death")
        {
            player = GameObject.FindGameObjectWithTag("Player");

            saveName = data.saveName;

            player.transform.position = new Vector3(data.playerX, data.playerY, data.playerZ);

            player.transform.eulerAngles = new Vector3(0, data.playerRotY, 0);

            Camera.main.transform.localEulerAngles = new Vector3(data.cameraRotX, 0, 0);

            player.GetComponent<Player>().itemEquipped = data.itemEquipped;
            player.GetComponent<Player>().leftUnlocked = data.leftUnlocked;
            player.GetComponent<Player>().topUnlocked = data.topUnlocked;
            player.GetComponent<Player>().rightUnlocked = data.rightUnlocked;

            player.GetComponent<Player>().leftUnlocked2 = data.leftUnlocked2;
            player.GetComponent<Player>().topUnlocked2 = data.topUnlocked2;
            player.GetComponent<Player>().rightUnlocked2 = data.rightUnlocked2;

            player.GetComponent<Player>().addedTime = int.Parse(data.timeElapsed);

            player.GetComponent<Player>().shipBlueprintAmount = data.shipBlueprintAmount;

            Camera.main.GetComponent<CameraRotation>().cameraValue = data.cameraValue;
        }

        if (player != null)
            player.GetComponent<Player>().afterLoaded();
    }
}
