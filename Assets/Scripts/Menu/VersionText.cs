using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersionText : MonoBehaviour
{
    void Start()
    {
        if (gameObject.name == "InGame")
            GetComponent<Text>().text = $"V{Application.version} Very Early Access";
        else
            GetComponent<Text>().text = $"V{Application.version}";
    }
}
