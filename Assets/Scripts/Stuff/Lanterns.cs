using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class Lanterns : MonoBehaviour
{
    HDAdditionalLightData _light;
    float intensity;
    bool rising = true;
    float midIntensity;

    IEnumerator flicker()
    {
        rising = !rising;
        yield return new WaitForSecondsRealtime(Random.Range(0.1f, 0.3f));
        StartCoroutine(flicker());
    }

    void Start()
    {
        _light = GetComponent<HDAdditionalLightData>();
        midIntensity = _light.intensity;
        intensity = Random.Range(midIntensity, midIntensity + 1);
        StartCoroutine(flicker());
    }

    private void Update()
    {
        if (rising && intensity < midIntensity + 1)
        {
            intensity += Random.Range(2f, 6f) * Time.deltaTime;
            _light.intensity = intensity;
        }
        else if (!rising && intensity > midIntensity)
        {
            intensity -= Random.Range(2f, 6f) * Time.deltaTime;
            _light.intensity = intensity;
        }
    }
}
