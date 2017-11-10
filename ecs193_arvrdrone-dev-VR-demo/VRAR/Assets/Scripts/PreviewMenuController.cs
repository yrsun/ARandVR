using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewMenuController : MonoBehaviour {
    bool instantiated;
    public GameObject previewMenuInstance;
    public GameObject modeMenu;

    // Use this for initialization
    void Start () {
        instantiated = previewMenuInstance.activeInHierarchy;
    }
	
	// Update is called once per frame
	void Update () {
        ToggleMenu();
	}

    private void ToggleMenu()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (instantiated == false && (modeMenu.GetComponent<ShowModeMenu>().getMode() == "Single" ||
                modeMenu.GetComponent<ShowModeMenu>().getMode() == "Batch") &&
                !modeMenu.GetComponent<ShowModeMenu>().Enabled())
            {
                previewMenuInstance.SetActive(true);
                instantiated = true;
            }
            else
            {
                previewMenuInstance.SetActive(false);
                instantiated = false;
            }
        }
    }

    public bool instantiationState()
    {
        return instantiated;
    }
}
