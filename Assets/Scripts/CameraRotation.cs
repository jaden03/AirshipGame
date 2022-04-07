using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public GameObject player;
    public float sensitivity;
    SaveManager saveManager;

    public float cameraValue = 0;

    bool smoothing;
    float xRot;
    float yRot;

    void Start()
    {
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();

        sensitivity = saveManager.getSettingsData().sensitivity;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        var MouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        var MouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        if (Input.GetKeyDown(KeyCode.F2))
            smoothing = !smoothing;

        if (!smoothing)
        {
            player.transform.Rotate(0, MouseX, 0);
            transform.Rotate(-MouseY, 0, 0);
            cameraValue += -MouseY;
        }
        else
        {
            xRot += MouseX * 0.025f;
            yRot += MouseY * 0.025f;
            player.transform.Rotate(0, xRot, 0);
            transform.Rotate(-yRot, 0, 0);
            cameraValue += -yRot;
            if (MouseX == 0 && MouseY == 0)
            {
                xRot *= 0.95f;
                yRot *= 0.95f;
            }
        }

        if (cameraValue > 80)
        {
            transform.localEulerAngles = new Vector3(80, 0, 0);
            cameraValue = 80;
        }
        if (cameraValue < -80)
        {
            transform.localEulerAngles = new Vector3(280, 0, 0);
            cameraValue = -80;
        }
    }
}
