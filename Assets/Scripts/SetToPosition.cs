using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetToPosition : MonoBehaviour
{
    public GameObject moveThis;
    public GameObject toThis;

    void Update()
    {
        moveThis.transform.position = toThis.transform.position;
        moveThis.transform.rotation = toThis.transform.rotation;
    }
}
