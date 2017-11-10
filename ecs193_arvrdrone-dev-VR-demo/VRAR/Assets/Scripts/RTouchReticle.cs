using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTouchReticle : MonoBehaviour
{
    // Pass CenterEyeAnchor into _CameraFacing
    public Camera _CameraFacing;
    // Assign the correct controller
    OVRInput.Controller _controller = OVRInput.Controller.RTouch;
    public float _xScale = 0.1f;
    public float _yScale = 0.1f;
    public float _zScale = 0.1f;
    private Vector3 _originalScale;

    // Use this for initialization
    void Start()
    {
        _originalScale.x = _xScale;
        _originalScale.y = _yScale;
        _originalScale.z = _zScale;
        // _originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //
        // Cast a forward ray and record the first thing in the way.
        // Record distance.
        //
        RaycastHit hit;
        float distance;
        if (Physics.Raycast(new Ray(_CameraFacing.transform.position + OVRInput.GetLocalControllerPosition(_controller),
                                     OVRInput.GetLocalControllerRotation(_controller) * Vector3.forward),
                                     out hit))
        {
            distance = hit.distance;
        }
        else
        {
            distance = _CameraFacing.farClipPlane * 0.95f;
        }

        transform.position = _CameraFacing.transform.position +
            OVRInput.GetLocalControllerPosition(_controller) +
            //
            // Vector3.forward == new Vector3(0, 0, 1)
            //
            OVRInput.GetLocalControllerRotation(_controller) * Vector3.forward * distance;
        transform.LookAt(_CameraFacing.transform.position);
        transform.Rotate(0.0f, 180.0f, 0.0f);
        //
        // Vector3.one == new Vector3(1.0f, 1.0f, 1.0f)
        //
        transform.localScale = _originalScale * distance;
    }
}
