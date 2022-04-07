using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentPlayerToAirship : MonoBehaviour
{
    public GameObject airshipObject;
    Player playerScript;

    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.parent = airshipObject.transform;
            playerScript.inShip = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.parent = null;
            playerScript.inShip = false;
        }
    }
}
