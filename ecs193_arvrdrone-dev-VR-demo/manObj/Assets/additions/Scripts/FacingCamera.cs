using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour {
    private GameObject character;
    // Use this for initialization
    void Start () {
        transform.position += new Vector3(0, transform.parent.gameObject.transform.lossyScale.y + 1, 0);
        character = GameObject.Find("FPSController");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 v = character.transform.position - transform.position;
        v.x = v.z = 0.0f;
        transform.LookAt(character.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
