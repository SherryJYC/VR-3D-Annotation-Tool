using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWeapon : MonoBehaviour {

    public GameObject[] weapons;
    public GameObject controllerRight;

    private int currentWeapon;
    private int nextWeapon;
    private SteamVR_TrackedController controller;

    // Use this for initialization
    void Start () {
        controller = controllerRight.GetComponent<SteamVR_TrackedController>();
        controller.Gripped += NextWeapon;
        currentWeapon = 0;
       
        for (int i = 1; i < weapons.Length; i++)
        {

            weapons[i].SetActive(false);
        }
        BroadcastMessage("ToolOnEnable", SendMessageOptions.DontRequireReceiver);
    }

    private bool LeftControllerMeshScaling()
    {
        if (GameObject.Find("ControlManager") == null)
        {
            if(GameObject.Find("ControlManager").GetComponent<MainControl>().left_controller_state.padTouched())
            {
                if (!GameObject.Find("ControlManager").GetComponent<MainControl>().left_controller_state.padPressed())
                {
                    return true;
                }
            }


        }

        return false;
    }
    private void NextWeapon(object sender, ClickedEventArgs e)
    {
        if (LeftControllerMeshScaling())
            { return; }

        nextWeapon = currentWeapon + 1;

        if (nextWeapon < weapons.Length)
        {
            EnableWeapon();
            currentWeapon++;
        }
        else{

            currentWeapon = 0;
            nextWeapon = 0;
            EnableWeapon();
        }
    }

    private void EnableWeapon()
    {
        int active_index=0;
        
        for (int i = 0; i < weapons.Length; i++)
        {
            if (i == nextWeapon)
            {
                active_index = i;
                
            }
            else 
            {            
                BroadcastMessage("ToolOnDisable", SendMessageOptions.DontRequireReceiver);
                weapons[i].SetActive(false);
            }
        }
       
        weapons[active_index].SetActive(true);
        BroadcastMessage("ToolOnEnable", SendMessageOptions.DontRequireReceiver);


    }

    // Update is called once per frame
    void Update () {
		
	}
}
