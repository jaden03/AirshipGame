using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STOPDISAPPEARING : MonoBehaviour
{
    void Start()
    {
        Mesh m = GetComponent<MeshFilter>().mesh;
        m.bounds = new Bounds(Vector3.zero, Vector3.one * 5000);
    }
}
