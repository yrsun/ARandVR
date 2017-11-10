using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGetColor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.parent.GetComponent<SwitchItem>().previewFlag)
        {
            GameObject pcp = GameObject.Find("PresetColorPicker");
            this.GetComponent<Renderer>().material.color = pcp.GetComponent<MyColorPicker>().color;
        }else
        {
            GameObject pcp = GameObject.Find("PresetColorPicker");
            this.GetComponent<Renderer>().material.color = new Vector4(1,1,1,1);
        }
        
    }
}
