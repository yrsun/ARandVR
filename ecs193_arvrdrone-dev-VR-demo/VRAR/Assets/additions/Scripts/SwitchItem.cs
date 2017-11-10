using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchItem : MonoBehaviour {
    public GameObject landscape;
    public Material TGgrass;
    public Material TGsand;
    public Material TGsnow;
    public Material TGpreview;
    public bool previewFlag = true;

    string[] itemOptions = new string[] { "grass", "sand", "snow", "preview" };
    int selectedIndex;

    // botton state:
    bool secondaryRightInUse;
    bool secondaryLeftInUse;

    // Use this for initialization
    void Start () {
        selectedIndex = 3;
        secondaryRightInUse = false;
        secondaryLeftInUse = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < 0.5f)
        {
            secondaryRightInUse = false;
        }

        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > -0.5f)
        {
            secondaryLeftInUse = false;
        }

        // Debug.Log(OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x);
        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x < -0.5f && secondaryLeftInUse == false)
        {
            if (selectedIndex == 0)
            {
                selectedIndex = itemOptions.Length - 1;
            }
            else
            {
                selectedIndex -= 1;
            }
            updatehelper();

            secondaryLeftInUse = true;
            landscape.GetComponent<Landscape>().setBlockType(selectedIndex + 1);
        }

        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).x > 0.5f && secondaryRightInUse == false)
        {
            if (selectedIndex == itemOptions.Length - 1)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex += 1;
            }
            updatehelper();

            secondaryRightInUse = true;
            landscape.GetComponent<Landscape>().setBlockType(selectedIndex + 1);
        }
    }

    void updatehelper()
    {
        switch (selectedIndex)
        {
            case 0:
                GameObject.Find("PreviewCube").GetComponent<Renderer>().material = TGgrass;
                GameObject.Find("PresetColorPicker").GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
                GameObject.Find("TitleText").GetComponent<Text>().text = "Grass";
                previewFlag = false;
                break;
            case 1:
                GameObject.Find("PreviewCube").GetComponent<Renderer>().material = TGsand;
                GameObject.Find("PresetColorPicker").GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
                GameObject.Find("TitleText").GetComponent<Text>().text = "Sand";
                previewFlag = false;
                break;
            case 2:
                GameObject.Find("PreviewCube").GetComponent<Renderer>().material = TGsnow;
                GameObject.Find("PresetColorPicker").GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
                GameObject.Find("TitleText").GetComponent<Text>().text = "Snow";
                previewFlag = false;
                break;
            case 3:
                GameObject.Find("PreviewCube").GetComponent<Renderer>().material = TGpreview;
                GameObject.Find("PresetColorPicker").GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
                GameObject.Find("TitleText").GetComponent<Text>().text = "Block Color";
                previewFlag = true;
                break;
            default:
                break;
        }
    }

    public int getIndex()
    {
        return selectedIndex;
    }
}
