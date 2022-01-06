using Valve.VR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SteamInputModule : VRInputModule
{
    public GameObject controller;
    public SteamVR_TrackedObject trackedObj;
    public SteamVR_Controller.Device device;

    protected override void Start()
    {
        trackedObj = controller.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        base.Start();
        
    }
    protected override void Awake()
    {
        base.Awake();

    }

    public override void Process()
    {
        base.Process();
        // Press
        if (TriggerIsPressed())
        {
            Press();
        }
            // Release
        if (TriggerIsUp())
            Release();
    }
    protected virtual bool TriggerIsPressed()
    {
       
        return device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
    }

  
    protected virtual bool TriggerIsUp()
    {
        return device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
    }
}