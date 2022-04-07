using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Island7BuildingUI : MonoBehaviour
{
    Image shipImage;
    Text shipText;
    Text decor;
    Text blueprint;
    Text notDetected;

    Text noBlueprint;
    Text accepted;
    Text upgraded;

    SaveManager saveManager;
    LoadManager loadManager;

    bool canInteract = true;

    bool upgradedAlready;

    IEnumerator litCoroutine(bool has)
    {
        if (!upgradedAlready)
        {
            if (has)
            {
                blueprint.enabled = false;
                for (int i = 0; i < 5; i++)
                {
                    accepted.enabled = true;
                    yield return new WaitForSecondsRealtime(0.25f);
                    accepted.enabled = false;
                    yield return new WaitForSecondsRealtime(0.25f);
                }
                blueprint.enabled = true;
                saveManager.upgradeShip();
                upgradedAlready = true;
            }
            else
            {
                blueprint.enabled = false;
                for (int i = 0; i < 5; i++)
                {
                    noBlueprint.enabled = true;
                    yield return new WaitForSecondsRealtime(0.25f);
                    noBlueprint.enabled = false;
                    yield return new WaitForSecondsRealtime(0.25f);
                }
                blueprint.enabled = true;
            }
        }
        else
        {
            blueprint.enabled = false;
            for (int i = 0; i < 5; i++)
            {
                upgraded.enabled = true;
                yield return new WaitForSecondsRealtime(0.25f);
                upgraded.enabled = false;
                yield return new WaitForSecondsRealtime(0.25f);
            }
            blueprint.enabled = true;
        }
        canInteract = true;
    }

    void Start()
    {
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();

        shipImage = GetComponent<Image>();
        shipText = transform.Find("Ship").GetComponent<Text>();
        decor = transform.Find("Decor").GetComponent<Text>();
        blueprint = transform.Find("Blueprint").GetComponent<Text>();
        notDetected = transform.Find("NotDetected").GetComponent<Text>();

        noBlueprint = transform.Find("NoBlueprint").GetComponent<Text>();
        accepted = transform.Find("Accepted").GetComponent<Text>();
        upgraded = transform.Find("Upgraded").GetComponent<Text>();

        SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);

        if (data.shipUpgraded)
            upgradedAlready = true;

        if (data.shipInArea == true)
        {
            blueprint.enabled = true;

            shipImage.color = new Color(0, 1, 0);
            shipText.color = new Color(0, 1, 0);
            decor.color = new Color(0, 1, 0);
            blueprint.color = new Color(0, 1, 0);
            notDetected.color = new Color(0, 1, 0);
        }
        else
        {
            notDetected.enabled = true;

            shipImage.color = new Color(1, 0, 0);
            shipText.color = new Color(1, 0, 0);
            decor.color = new Color(1, 0, 0);
            blueprint.color = new Color(1, 0, 0);
            notDetected.color = new Color(1, 0, 0);
        }
    }

    public void Interact(int amount)
    {
        SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);

        if (data.shipInArea == true)
        {
            if (amount == 3 && canInteract)
            {
                canInteract = false;
                StartCoroutine(litCoroutine(true));
            }
            else if (canInteract)
            {
                canInteract = false;
                StartCoroutine(litCoroutine(false));
            }
        }
    }
}
