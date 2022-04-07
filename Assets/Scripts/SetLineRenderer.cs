using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLineRenderer : MonoBehaviour
{
    public GameObject object1;
    public GameObject object2;
    LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lr.SetPosition(0, object1.transform.position);
        lr.SetPosition(1, object2.transform.position);
    }
}
