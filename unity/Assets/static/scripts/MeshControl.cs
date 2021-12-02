using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshControl : MonoBehaviour
{

    public GameObject controllerLeft;
    public GameObject mesh;

    private SteamVR_TrackedController controller;


    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private Vector2 previous_touchpad;




    Vector2 touchpad;

    private float sensitivity = 3.5f;

    private Vector2 padScrolling;


    private float xtemp = float.MaxValue;  //variable for the function ModifiyRadius  

    private float magnification = 1.0F;

    private float deltaX = 0F;

    private float rescale=1;
    private Vector3 mesh_scale;

    void Start()
    {
        trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        previous_touchpad = new Vector2(.0f, .0f);
    }
    private void Awake()
    {
        mesh_scale =new Vector3(1f,1f,1f)*GameObject.Find("ControlManager").GetComponent<MainControl>().TransformRate();
    }

    private GameObject FindObject(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
    private bool PadIsPressing()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
    }

    protected virtual bool TriggerIsPressed()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
    }

    protected virtual bool RightGripIsPressed()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Grip);
    }
    protected virtual bool TriggerIsUp()
    {
        return device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
    }


    private bool PadIsUp()
    {
        return (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad));
    }
    private bool PadIsTouching()
    {
        return device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
    }
    // Update is called once per frame
    void Update()
    {
        changeRotation();
        ChangeScale();
    }
    void changeRotation()
    {
        if (PadIsPressing())
        {
            Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            float tiltAroundX = touchpad.x * sensitivity;
            float tiltAroundY = touchpad.y * sensitivity;
            //print("Pressing Touchpad");
            //print(touchpad);
            mesh.transform.Rotate(new Vector3(0, tiltAroundX, tiltAroundY), Space.World);
        }
    }
    void ChangeScale()
    {
        if (!PadIsPressing() && PadIsTouching()&& RightGripIsPressed())
        {


            padScrolling = device.GetAxis();

            if (xtemp == float.MaxValue)
            {
                xtemp = padScrolling.x;
              
            }
            else
            {


                deltaX = xtemp - padScrolling.x;
                xtemp = padScrolling.x;
                if (deltaX >= -0.1 && deltaX <= 0.1)
                { 

                rescale += deltaX * magnification;
                   

                if (rescale < 0.5) rescale = 0.5F;
                if (rescale > 5) rescale = 5f;

                mesh.transform.localScale = mesh_scale * rescale;
                  }

            }
        }
    }
}
