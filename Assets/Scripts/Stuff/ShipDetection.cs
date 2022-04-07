using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDetection : MonoBehaviour
{
    MeshRenderer redLightRend;
    MeshRenderer greenLightRend;
    Light pointLight;

    SaveManager saveManager;

    void Start()
    {
        redLightRend = transform.parent.Find("redLight/default").GetComponent<MeshRenderer>();
        greenLightRend = transform.parent.Find("greenLight/default").GetComponent<MeshRenderer>();
        pointLight = transform.parent.Find("Light").GetComponent<Light>();
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.transform.parent && collider.transform.parent.name == "MainShipCollision")
        {
            redLightRend.enabled = false;
            greenLightRend.enabled = true;
            pointLight.color = new Color(0, 1, 0);
            saveManager.updateShipInArea(true);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.transform.parent && collider.transform.parent.name == "MainShipCollision")
        {
            redLightRend.enabled = true;
            greenLightRend.enabled = false;
            pointLight.color = new Color(1, 0, 0);
            saveManager.updateShipInArea(false);
        }
    }
}
