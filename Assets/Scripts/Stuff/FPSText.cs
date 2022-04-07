using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSText : MonoBehaviour
{
    Text text;

    IEnumerator Start()
    {
        text = GetComponent<Text>();

        while (true)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            float fps = (int)(1f / Time.deltaTime);
            text.text = $"FPS - {Mathf.RoundToInt(fps)}";
        }
    }
}
