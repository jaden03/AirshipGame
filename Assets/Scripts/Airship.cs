using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Airship : MonoBehaviour
{
    public GameObject airshipObject;
    Rigidbody rb;

    GameObject throttleLever;
    GameObject altitudeLever;
    GameObject helm;

    public float turnSpeed = 1;
    public bool engines = true;

    float turnValue;
    float turnModifier;
    float throttleModifier;
    float currentShipSpeed;
    float setSpeed;
    float rootSpeed = 30;
    public float setHeight;

    Text throttleText;
    Text speedText;
    Text altitudeText;
    Text setAltitudeText;
    Text directionText;
    Text headingText;

    Animator leftEngine;
    Animator rightEngine;

    SaveManager saveManager;
    LoadManager loadManager;
    bool upgraded;

    public float throttlePos;

    void Start()
    {
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        loadManager = GameObject.FindGameObjectWithTag("LoadManager").GetComponent<LoadManager>();

        SaveManager.SaveData data = saveManager.getSaveData(loadManager.saveSlot);
        if (data.shipUpgraded == true)
            upgraded = true;

        setHeight = transform.position.y;

        rb = GetComponent<Rigidbody>();

        if (upgraded == true && engines == true)
        {
            throttleLever = airshipObject.transform.Find("Upgraded").Find("ThrottleLever").Find("ThrottleLever").gameObject;
            altitudeLever = airshipObject.transform.Find("Upgraded").Find("AltitudeLever").Find("AltitudeLever").gameObject;
            helm = airshipObject.transform.Find("Upgraded").Find("Steering").Find("SteeringPeg").gameObject;

            throttleText = airshipObject.transform.Find("Upgraded").Find("ThrottleDisplay").Find("Canvas").Find("5").GetComponent<Text>();
            speedText = airshipObject.transform.Find("Upgraded").Find("ThrottleDisplay").Find("Canvas").Find("2").GetComponent<Text>();

            altitudeText = airshipObject.transform.Find("Upgraded").Find("AltitudeDisplay").Find("Canvas").Find("2").GetComponent<Text>();
            setAltitudeText = airshipObject.transform.Find("Upgraded").Find("AltitudeDisplay").Find("Canvas").Find("5").GetComponent<Text>();

            directionText = airshipObject.transform.Find("Upgraded").Find("HeadingDisplay").Find("Canvas").Find("1").GetComponent<Text>();
            headingText = airshipObject.transform.Find("Upgraded").Find("HeadingDisplay").Find("Canvas").Find("2").GetComponent<Text>();
        }
        else
        {
            throttleLever = airshipObject.transform.Find("NonUpgraded").Find("ThrottleLever").Find("ThrottleLever").gameObject;
            altitudeLever = airshipObject.transform.Find("NonUpgraded").Find("AltitudeLever").Find("AltitudeLever").gameObject;
            helm = airshipObject.transform.Find("NonUpgraded").Find("Steering").Find("SteeringPeg").gameObject;

            throttleText = airshipObject.transform.Find("NonUpgraded").Find("ThrottleDisplay").Find("Canvas").Find("5").GetComponent<Text>();
            speedText = airshipObject.transform.Find("NonUpgraded").Find("ThrottleDisplay").Find("Canvas").Find("2").GetComponent<Text>();

            altitudeText = airshipObject.transform.Find("NonUpgraded").Find("AltitudeDisplay").Find("Canvas").Find("2").GetComponent<Text>();
            setAltitudeText = airshipObject.transform.Find("NonUpgraded").Find("AltitudeDisplay").Find("Canvas").Find("5").GetComponent<Text>();

            directionText = airshipObject.transform.Find("NonUpgraded").Find("HeadingDisplay").Find("Canvas").Find("1").GetComponent<Text>();
            headingText = airshipObject.transform.Find("NonUpgraded").Find("HeadingDisplay").Find("Canvas").Find("2").GetComponent<Text>();
        }

        if (engines)
        {
            leftEngine = airshipObject.transform.Find("Propeller Left").GetComponent<Animator>();
            rightEngine = airshipObject.transform.Find("Propeller Right").GetComponent<Animator>();
        }
    }

    void Update()
    {
        // If barely moving then stop moving \\
        if (rb.velocity.magnitude < 0.1f && throttleModifier == 0 && rb.transform.position.y == setHeight)
            rb.velocity = Vector3.zero;

        // If turning then turn if not, then just dont turn moron \\
        if (helm.transform.localEulerAngles != Vector3.zero && throttleModifier != 0)
            rb.angularVelocity = new Vector3(0, ((turnModifier / 5) * throttleModifier) * turnSpeed, 0);
        else
            rb.angularVelocity = rb.angularVelocity * 0.99f;

        // If moving vertical too fast, then cap it \\
        if (rb.velocity.y < -5)
            rb.velocity = new Vector3(rb.velocity.x, -5, rb.velocity.z);
        if (rb.velocity.y > 5)
            rb.velocity = new Vector3(rb.velocity.x, 5, rb.velocity.z);

        // If not at setHeight then go there otherwise, stop moving vertical \\
        if (rb.transform.position.y < setHeight - 0.2f)
            rb.AddForce(new Vector3(0, 500f, 0));
        else if (rb.transform.position.y > setHeight + 0.2f)
            rb.AddForce(new Vector3(0, -500f, 0));
        else
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * 0.9f, rb.velocity.z);

        // If barely moving in ways that arent vertical, stop moving in every way except vertical \\
        if (rb.velocity.x < 0.1f && rb.velocity.z < 0.1f && throttleModifier < 0.1f)
            rb.velocity = new Vector3(rb.velocity.x * 0.99f, rb.velocity.y, rb.velocity.z * 0.99f);

        // Set the throttle text to the current throttle amount \\
        currentShipSpeed = rootSpeed * (throttleModifier);
        setSpeed = throttleModifier * rootSpeed;
        string throttleString = $"{Mathf.RoundToInt(throttleModifier * 100)}%";
        throttleText.text = throttleString;

        // Calculate driftness \\
        float horizontalSpeed = rb.transform.InverseTransformDirection(rb.velocity).z;
        var shipVelocity = (-rb.transform.forward * currentShipSpeed * 150 * throttleModifier);

        // Set the speed text to the current speed \\
        int speedForward = -Mathf.RoundToInt(rb.transform.InverseTransformDirection(rb.velocity).z);
        string speedF = $"{speedForward}m/s";
        speedText.text = speedF;

        // Actually move the ship if throttle is forward \\
        if (rb.velocity.magnitude < setSpeed)
        {
            rb.AddForce(shipVelocity);
        }
        else
            rb.velocity = new Vector3(rb.velocity.x * 0.9999999f, rb.velocity.y, rb.velocity.z * 0.9999999f);

        // If drifting then stop \\ 
        if (horizontalSpeed > 0.025f || speedForward < 0 || horizontalSpeed < -0.025f)
            rb.velocity = new Vector3(rb.velocity.x * 0.99f, rb.velocity.y, rb.velocity.z * 0.99f);

        // Set altitude text n stuff ( 1300m is added to it to make it feel more realistic ) \\

        if (engines)
        {
            altitudeText.text = $"{Mathf.RoundToInt(rb.transform.position.y) + 1300 - 13}m";
            setAltitudeText.text = $"{Mathf.RoundToInt(setHeight) + 1300 - 13}m";
        }
        else
        {
            altitudeText.text = $"{Mathf.RoundToInt(rb.transform.position.y) + 1300}m";
            setAltitudeText.text = $"{Mathf.RoundToInt(setHeight) + 1300}m";
        }

        // Calculate and set compass text \\
        headingText.text = $"{Mathf.RoundToInt(rb.transform.eulerAngles.y)}";

        if (rb.transform.eulerAngles.y > 315 || rb.transform.eulerAngles.y < 45)
            directionText.text = "N";
        else if (rb.transform.eulerAngles.y > 45 && rb.transform.eulerAngles.y < 135)
            directionText.text = "E";
        else if (rb.transform.eulerAngles.y > 135 && rb.transform.eulerAngles.y < 225)
            directionText.text = "S";
        else if (rb.transform.eulerAngles.y > 225 && rb.transform.eulerAngles.y < 315)
            directionText.text = "W";

        // Set throttle, height, and rotation \\
        throttleModifier = Mathf.InverseLerp(0.06f, -0.36f, throttleLever.transform.localPosition.z);
        throttlePos = throttleLever.transform.localPosition.z;

        if (altitudeLever.transform.localPosition.z != -0.155f)
            setHeight += 0.1f * ((Mathf.InverseLerp(0.06f, -0.36f, altitudeLever.transform.localPosition.z) - 0.5f) * 1000) * Time.deltaTime;

        if (setHeight < -787)
            setHeight = -787;

        if (helm.transform.localEulerAngles.y >= 275 && helm.transform.localEulerAngles.y <= 360)
        {
            // Left
            turnValue = helm.transform.localEulerAngles.y - 275 - 85;
        }
        if (helm.transform.localEulerAngles.y >= 0 && helm.transform.localEulerAngles.y <= 85)
        {
            // Right
            turnValue = helm.transform.localEulerAngles.y;
        }
        if (helm.transform.localEulerAngles == Vector3.zero)
            turnValue = 0;

        turnModifier = turnValue / 85;

        // Animate the engines \\
        if (engines)
        {
            leftEngine.SetFloat("Speed", throttleModifier);
            rightEngine.SetFloat("Speed", throttleModifier);

            // Set Trails \\
            if (throttleModifier != 0)
            {
                foreach (Transform child in leftEngine.transform)
                {
                    if (child.name == "Trail")
                        child.GetComponent<TrailRenderer>().emitting = true;
                }
                foreach (Transform child in rightEngine.transform)
                {
                    if (child.name == "Trail")
                        child.GetComponent<TrailRenderer>().emitting = true;
                }
            }
            else
            {
                foreach (Transform child in leftEngine.transform)
                {
                    if (child.name == "Trail")
                        child.GetComponent<TrailRenderer>().emitting = false;
                }
                foreach (Transform child in rightEngine.transform)
                {
                    if (child.name == "Trail")
                        child.GetComponent<TrailRenderer>().emitting = false;
                }
            }
        }




    }
}
