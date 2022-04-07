using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clouds : MonoBehaviour
{
    bool canGoAway = true;
    float fartherestX;

    void Start()
    {
        gameObject.transform.localScale = Vector3.zero;
        Vector3 scale = new Vector3(Random.Range(15, 30), Random.Range(10, 20), Random.Range(10, 20));
        LeanTween.scale(gameObject, scale, 10);
    }

    void Update()
    {
        transform.position += new Vector3(1, 0, 0) * Time.deltaTime * 10;

        if (canGoAway && transform.position.x > fartherestX + 200)
        {
            canGoAway = false;
            LeanTween.scale(gameObject, Vector3.zero, 10).setDestroyOnComplete(true);
        }
    }
}
