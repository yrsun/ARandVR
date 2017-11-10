using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjMenu : MonoBehaviour {
    public Sprite TGgrass;
    public Sprite TGsand;
    public Sprite TGsnow;
    public Sprite TGpreview = null;
    public bool previewFlag = true;

    string[] itemOptions = new string[] { "grass", "sand", "snow", "preview" };
    int selectedIndex = 3;
    private Image r;
    private GameObject pcp;
    // Use this for initialization
    void Start () {
        r = GameObject.Find("PreviewImage").GetComponent<Image>();
        pcp = GameObject.Find("PresetColor");
        if (transform.parent.GetComponent<Renderer>().material.name == "TGgrass")
        {
            r.sprite = TGgrass;
            r.color = new Vector4(1, 1, 1, 1);
            selectedIndex = 0;
        }else if (transform.parent.GetComponent<Renderer>().material.name == "TGsand")
        {
            r.sprite = TGsand;
            r.color = new Vector4(1, 1, 1, 1);
            selectedIndex = 1;
        }
        else if (transform.parent.GetComponent<Renderer>().material.name == "TGsnow")
        {
            r.sprite = TGsnow;
            r.color = new Vector4(1, 1, 1, 1);
            selectedIndex = 2;
        }
        else
        {
            r.sprite = null;
            r.color = transform.parent.GetComponent<Renderer>().material.color;
            selectedIndex = 3;
        }
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
                r.sprite = TGgrass;
                r.color = new Vector4(1, 1, 1, 1);
                GameObject.Find("PresetColor").GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
                previewFlag = false;
                break;
            case 1:
                r.sprite = TGsand;
                r.color = new Vector4(1, 1, 1, 1);
                GameObject.Find("PresetColor").GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
                previewFlag = false;
                break;
            case 2:
                r.sprite = TGsnow;
                r.color = new Vector4(1, 1, 1, 1);
                GameObject.Find("PresetColor").GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
                previewFlag = false;
                break;
            case 3:
                r.sprite = TGpreview;
                r.color = pcp.GetComponent<MyColorPicker>().color;
                GameObject.Find("PresetColor").GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
                previewFlag = true;
                break;
            default:
                break;
        }
    }
    
}
