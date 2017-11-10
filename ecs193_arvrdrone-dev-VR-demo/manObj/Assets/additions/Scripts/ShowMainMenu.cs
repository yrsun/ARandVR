using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowMainMenu : MonoBehaviour {
    string[] menuOptions = new string[] { "Create", "Edit", "Delete" };
    int selectedIndex = 1;

    bool paused = false;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GameObject.Find("MainMenu").GetComponent<Canvas>().enabled = !paused;
            paused = !paused;
            //paused = togglePause();
        }

        if (paused)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(.5f,.5f,.5f, 1);
                if (selectedIndex == 0)
                {
                    selectedIndex = menuOptions.Length - 1;
                }
                else
                {
                    selectedIndex -= 1;
                }
                GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(.5f, .5f, .5f, 1);

                if (selectedIndex == menuOptions.Length - 1)
                {
                    selectedIndex = 0;
                }
                else
                {
                    selectedIndex += 1;
                }
                GameObject.Find(menuOptions[selectedIndex].ToString()).transform.GetComponent<Image>().color = new Vector4(1, 1, 1, 1);

            }
        }
	}


    public string getMode()
    {
        GameObject.Find("Mode").GetComponent<Text>().text = menuOptions[selectedIndex];
        return paused ? null : menuOptions[selectedIndex];
    }


    

    bool togglePause()
    {
        if (Time.timeScale == 0f)
        {
            
            Time.timeScale = 1f;
            return (false);
        }
        else
        {
            Time.timeScale = 0f;
            return (true);
        }
    }
}
