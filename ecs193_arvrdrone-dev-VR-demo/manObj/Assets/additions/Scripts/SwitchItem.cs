using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchItem : MonoBehaviour {
    public Material TGgrass;
    public Material TGsand;
    public Material TGsnow;
    public Material TGpreview;
    public bool previewFlag = true;

    string[] itemOptions = new string[] { "grass", "sand", "snow", "preview" };
    int selectedIndex = 3;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedIndex == 0)
            {
                selectedIndex = itemOptions.Length - 1;
            }
            else
            {
                selectedIndex -= 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (selectedIndex == itemOptions.Length - 1)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex += 1;
            }
        }


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
}
