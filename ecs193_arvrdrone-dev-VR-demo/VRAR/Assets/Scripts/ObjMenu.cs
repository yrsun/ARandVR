using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjMenu : MonoBehaviour
{
    public Sprite TGgrass;
    public Sprite TGsand;
    public Sprite TGsnow;
    public Sprite TGpreview;
    public bool previewFlag = true;

    string[] itemOptions = new string[] { "grass", "sand", "snow", "preview" };
    int selectedIndex = 3;
    private Image r;
    private GameObject pcp;
    // Use this for initialization
    void Start()
    {
        r = GameObject.Find("ObjPreviewImage").GetComponent<Image>();
        pcp = GameObject.Find("PresetColor");
        if (transform.parent.GetComponent<Renderer>().material.name == "TGrass (Instance)")
        {
            r.sprite = TGgrass;
            r.color = new Vector4(1, 1, 1, 1);
            pcp.GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
            selectedIndex = 0;
        }
        else if (transform.parent.GetComponent<Renderer>().material.name == "TSand (Instance)")
        {
            r.sprite = TGsand;
            r.color = new Vector4(1, 1, 1, 1);
            pcp.GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
            selectedIndex = 1;
        }
        else if (transform.parent.GetComponent<Renderer>().material.name == "TSnow (Instance)")
        {
            r.sprite = TGsnow;
            r.color = new Vector4(1, 1, 1, 1);
            pcp.GetComponent<Image>().color = new Vector4(.8f, .8f, .8f, .2f);
            selectedIndex = 2;
        }
        else
        {
            r.sprite = TGpreview;
            r.color = new Vector4(transform.parent.GetComponent<Renderer>().material.color.r,
                                    transform.parent.GetComponent<Renderer>().material.color.g,
                                    transform.parent.GetComponent<Renderer>().material.color.b, 1);
            selectedIndex = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickLeft))
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
        }

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickRight))
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
        }

        if(selectedIndex == 3 && pcp)
        {
            r.color = pcp.GetComponent<MyColorPicker>().color;
        }



    }

    void updatehelper()
    {
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

    public int getMaterialState()
    {
        return selectedIndex;
    }
}