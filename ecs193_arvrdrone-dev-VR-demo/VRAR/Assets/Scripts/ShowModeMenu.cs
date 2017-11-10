using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowModeMenu : MonoBehaviour {
    string[] menuOptions = new string[] { "Single", "Batch", "Pre", "Edit" };
    int selectedIndex = 0;
    public GameObject previewMenuController;
    public GameObject modeMenu;
    public GameObject landscape;

    bool paused = false;
	
	// Update is called once per frame
	void Update () {
        if (!previewMenuController.GetComponent<PreviewMenuController>().instantiationState() &&
            !landscape.GetComponent<Landscape>().getObjectMenuSelect())
        {
            ToggleMenu();
            NextMode();
            PreviousMode();
        }

	}

    private void PreviousMode()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            GameObject.Find(menuOptions[selectedIndex]).transform.GetComponent<Image>().color = new Vector4(.5f, .5f, .5f, 1);
            if (selectedIndex == 0)
            {
                selectedIndex = menuOptions.Length - 1;
            }
            else
            {
                selectedIndex -= 1;
            }
            GameObject.Find(menuOptions[selectedIndex]).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            GameObject.Find("ShowModeText").GetComponent<Text>().text = menuOptions[selectedIndex];
        }
    }

    private void NextMode()
    {
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            GameObject.Find(menuOptions[selectedIndex]).transform.GetComponent<Image>().color = new Vector4(.5f, .5f, .5f, 1);
            if (selectedIndex == menuOptions.Length - 1)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex += 1;
            }
            GameObject.Find(menuOptions[selectedIndex]).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            GameObject.Find("ShowModeText").GetComponent<Text>().text = menuOptions[selectedIndex];
        }
    }

    private void ToggleMenu()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            modeMenu.GetComponent<Canvas>().enabled = !paused;
            paused = !paused;
        }
    }

    public string getMode()
    {
        return menuOptions[selectedIndex];
    }

    public int getModeIndex()
    {
        return selectedIndex + 1;
    }

    public bool Enabled()
    {
        return paused;
    }
}
