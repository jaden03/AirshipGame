using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateTest : MonoBehaviour
{
    GameObject leftBox;
    GameObject midBox;
    GameObject rightBox;
    GameObject outputBox;

    public Gate gateDropDown = new Gate();

    public enum Gate { AND, NAND, OR, NOR };

    BBoolean.BetterBoolean boxBoolean;

    void Start()
    {
        boxBoolean = new BBoolean.BetterBoolean();

        string[] names = new string[] { "Left", "Middle", "Right" };

        bool[] booleans = new bool[] { false, false, false };

        boxBoolean.booleans = booleans;
        boxBoolean.names = names;

        outputBox = gameObject;

        leftBox = gameObject.transform.Find("Left").gameObject;
        midBox = gameObject.transform.Find("Middle").gameObject;
        rightBox = gameObject.transform.Find("Right").gameObject;

        leftBox.GetComponent<MeshRenderer>().material.color = Color.red;
        midBox.GetComponent<MeshRenderer>().material.color = Color.red;
        rightBox.GetComponent<MeshRenderer>().material.color = Color.red;
        outputBox.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    void Update()
    {
        if (gateDropDown == Gate.AND)
        {
            if (BBoolean.AND(boxBoolean))
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        else if (gateDropDown == Gate.NAND)
        {
            if (BBoolean.NAND(boxBoolean))
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        else if (gateDropDown == Gate.OR)
        {
            if (BBoolean.OR(boxBoolean))
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
        else if (gateDropDown == Gate.NOR)
        {
            if (BBoolean.NOR(boxBoolean))
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            }
            else
            {
                outputBox.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            }
        }
    }

    public void swapBoolean(string name)
    {
        BBoolean.swapBoolean(boxBoolean, name);
        if (name == "Left")
        {
            if (leftBox.GetComponent<MeshRenderer>().material.color == Color.red)
                leftBox.GetComponent<MeshRenderer>().material.color = Color.green;
            else
                leftBox.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (name == "Middle")
        {
            if (midBox.GetComponent<MeshRenderer>().material.color == Color.red)
                midBox.GetComponent<MeshRenderer>().material.color = Color.green;
            else
                midBox.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        if (name == "Right")
        {
            if (rightBox.GetComponent<MeshRenderer>().material.color == Color.red)
                rightBox.GetComponent<MeshRenderer>().material.color = Color.green;
            else
                rightBox.GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }
}
