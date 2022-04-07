using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    [SerializeField, Range(0, 24)] public float time;

    public bool dayCycle = true;

    void Update()
    {
        if (Application.isPlaying && dayCycle)
        {
            time += Time.deltaTime * 0.025f;
            time %= 24;
        }
    }
}
