using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyColorPicker : MonoBehaviour {
    public Scrollbar scrollbar;
    public Color color;
    // public GameObject landscape;

    /// <summary>
    /// In other script, the following code would be an example to fetch the color
    /// 
    /// GameObject pcp = GameObject.Find("PresetColorPicker");
    /// yourObjects.GetComponent<Renderer>().material.color = pcp.GetComponent<MyColorPicker>().color;
    /// </summary>


	// Use this for initialization
	void Start () {
        color = new Color(1f, 0f, 0f);
        scrollbar.value = 0;
        scrollbar.enabled = false;
    }
	

	// Update is called once per frame
	void Update () {
        //value from 0 - 1
        //color from (255, 0, 0) to (255, 255, 0) to (0, 255, 0)
        //      from (0, 255, 0) to (0, 255, 255) to (0, 0, 255)
        //      from (0, 0, 255) to (255, 0, 255) to (255, 0, 0)

        if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y > 0.5f)
        {
            scrollbar.value -= .005f;
        }
        else if (OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y < -0.5f)
        {
            scrollbar.value += .005f;
        }

        if (scrollbar.value >= 0 && scrollbar.value < ((float)1/6))
        {
            color = new Color(1f, (float)(scrollbar.value * 6f), 0f);
        }
        else if (scrollbar.value >= ((float)1 / 6) && scrollbar.value < ((float)2 / 6))
        {
            color = new Color((float)(1f - ((float)scrollbar.value - ((float)1 / 6)) * 6f), 1f, 0f);
        }
        else if (scrollbar.value >= ((float)2 / 6) && scrollbar.value < ((float)3 / 6))
        {
            color = new Color(0f, 1f, (float)((float)(scrollbar.value - ((float)2 / 6)) * 6f));
        }
        else if (scrollbar.value >= ((float)3 / 6) && scrollbar.value < ((float)4 / 6))
        {
            color = new Color(0f, (float)(1f - ((float)scrollbar.value - ((float)3 / 6)) * 6f), 1f);
        }
        else if (scrollbar.value >= ((float)4 / 6) && scrollbar.value < ((float)5 / 6))
        {
            color = new Color((float)((float)(scrollbar.value - ((float)4 / 6)) * 6f), 0f, 1f);
        }
        else if(scrollbar.value >= ((float)5 / 6) && scrollbar.value <= 1f)
        {
            color = new Color(1f, 0f, (float)(1f - ((float)scrollbar.value - ((float)5 / 6)) * 6f));
        }

        GameObject.Find("Landscape").GetComponent<Landscape>().setColor(color);
        // landscape.GetComponent<Landscape>().setColor(color);
    }
}
