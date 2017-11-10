using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchObj : MonoBehaviour {
    private GameObject crosshair;
    // Use this for initialization
    public GameObject objMenuPrefab;
    public GameObject hoveredGO;
    public enum HoverState { HOVER, NONE };
    public enum SelectState { SELECTED, NONE };
    public HoverState hover_state = HoverState.NONE;
    public SelectState select_state = SelectState.NONE;

    void Start () {
		crosshair = GameObject.Find("CrossHair");
	}
	
	// Update is called once per frame
	void Update () {
        string mainMode = GameObject.Find("CrossHairCanvas").GetComponent<ShowMainMenu>().getMode();

        switch (mainMode)
        {
            case "Edit":
                editMode();
                break;
            case "Delete":
                deleteMode();
                break;
            case "Create":
                createMode();
                break;
            default:
                break;
        }



    }

    private void deleteMode()
    {
        RaycastHit hit;
        float theDistance;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);


        if (Physics.Raycast(transform.position, (forward), out hit) && hit.collider.gameObject.tag != "ground")
        {
            #region hover capture
            if (hover_state == HoverState.NONE)
            {
                hoveredGO = hit.collider.gameObject;
            }
            hover_state = HoverState.HOVER;
            #endregion

            #region crosshair change color
            crosshair.GetComponent<Image>().color = Color.red;
            theDistance = hit.distance;
            print(theDistance + " " + hit.collider.gameObject.name);
            #endregion
        }
        else
        {
            #region hover throw
            if (hover_state == HoverState.HOVER)
            {
                // hoverObject message
                hoveredGO = null;
            }
            hover_state = HoverState.NONE;
            #endregion

            #region crosshair change color
            crosshair.GetComponent<Image>().color = Color.green;
            #endregion
        }

        if (hover_state == HoverState.HOVER)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Destroy(hoveredGO);
                hoveredGO = null;
                select_state = SelectState.NONE;
            }
        }
    }

    private void createMode()
    {

    }

    private void editMode()
    {
        if (select_state == SelectState.NONE)
        {
            searchMode();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Destroy(hoveredGO.transform.FindChild("ObjectMenu").gameObject);
                Renderer rend = hoveredGO.transform.GetComponent<Renderer>();
                rend.material.SetColor("_EmissionColor", Color.black);
                select_state = SelectState.NONE;
            }
        }
    }

    private void searchMode()
    {
        RaycastHit hit;
        float theDistance;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, forward, Color.green);

        
        if (Physics.Raycast(transform.position, (forward), out hit) && hit.collider.gameObject.tag != "ground")
        {
            #region hover capture
            if (hover_state == HoverState.NONE)
            {
                hoveredGO = hit.collider.gameObject;
            }
            hover_state = HoverState.HOVER;
            #endregion

            #region crosshair change color
            crosshair.GetComponent<Image>().color = Color.red;
            theDistance = hit.distance;
            print(theDistance + " " + hit.collider.gameObject.name);
            #endregion
        }
        else
        {
            #region hover throw
            if (hover_state == HoverState.HOVER)
            {
                // hoverObject message
                hoveredGO = null;
            }
            hover_state = HoverState.NONE;
            #endregion

            #region crosshair change color
            crosshair.GetComponent<Image>().color = Color.green;
            #endregion
        }

        if (hover_state == HoverState.HOVER)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject objectMenu = Instantiate(objMenuPrefab);
                objectMenu.transform.SetParent(hoveredGO.transform);
                objectMenu.name = "ObjectMenu";
                objectMenu.transform.position = hoveredGO.transform.position;
                Renderer rend = hoveredGO.transform.GetComponent<Renderer>();

                if (rend)
                {
                    // Change the material of all hit colliders
                    // to use a transparent shader.
                    rend.material.SetColor("_EmissionColor", Color.grey);
                }
                select_state = SelectState.SELECTED;
            }
        }
    }






    
}
