using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Screenshots : MonoBehaviour
{
    public int screenShotCounter;
    public MeshRenderer mr;

    void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/screenshots"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/screenshots");
            screenShotCounter = (Directory.GetFiles(Application.persistentDataPath + "/screenshots").Length);
        }
        else
            screenShotCounter = (Directory.GetFiles(Application.persistentDataPath + "/screenshots").Length);

        RenderTexture rt = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        GetComponent<Camera>().targetTexture = rt;

        mr.material.mainTexture = rt;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            screenShot();
    }

    void screenShot()
    {
        Camera camera = GetComponent<Camera>();

        RenderTexture rt = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        camera.Render();

        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height, TextureFormat.RGB24, false, true);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = rt;

        var bytes = image.EncodeToPNG();
        Destroy(image);

        File.WriteAllBytes(Application.persistentDataPath + "/screenshots/" + screenShotCounter + ".png", bytes);
        screenShotCounter++;
    }
}
