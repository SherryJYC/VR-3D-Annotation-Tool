using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainControl : MonoBehaviour
{

    //Public attribute to attach Prefabs
    public GameObject prefabPaintball;
    public GameObject prefabSprayGunControl;

    //List of Controller 
    public GameObject ControllerLeft;
    public GameObject ControllerRight;
    public static GameObject ControllerLeftInstance;
    public static GameObject ControllerRightInstance;
    public static GameObject PlayerCameraRig;
    public static GameObject TopCameraMiniMap;
    public static GameObject MapControl;

    //
    private SteamVR_TrackedObject trackedObjectRight;
    private SteamVR_Controller.Device deviceRight;

    // Right Controller functionnalities
    private PaintballController paintballController;
    private MenuLaserPointer menuLaserPointer;
    private TeleportController teleportController;

    // Variable to know the state of the controller
    private bool isTeleporting = false;
    private bool isMenuActive = false;
    private bool isLabelling = false;

    //Private reference to prefab
    private int mode = 0; // 0 is User-static and 1 is User-dynamic
    private int currentMode;
    private GameObject[] prefabArrayRightController = new GameObject[2];
   
 
    public static Dictionary<Color, int> categoriesFromRGB;
    

    [System.Serializable]
    public struct label
    {
        public int ID;
        public string labelname;
        public Color color;
    }

    public static label[] labellist;

    void Start()
    {

        //Initialize Camera 
        PlayerCameraRig = GameObject.FindGameObjectWithTag("MainCamera");
        //TopCameraMiniMap = GameObject.FindGameObjectWithTag("MiniMapCamera");
        //MapControl = GameObject.FindObjectOfType<MapControl>().gameObject;

        //Instantiate Controllers
        ControllerLeftInstance = ControllerLeft;
        ControllerRightInstance = ControllerRight;
        trackedObjectRight = ControllerRightInstance.GetComponent<SteamVR_TrackedObject>();
        deviceRight = SteamVR_Controller.Input((int)trackedObjectRight.index);

        //Instantiate the prefab of the different modes
        //User-stitic mode
        prefabArrayRightController[0] = Instantiate(prefabSprayGunControl);
        prefabArrayRightController[0].transform.SetParent(PlayerCameraRig.transform, false);
        prefabArrayRightController[0].SetActive(false);

        //User-Dynamic mode
        prefabArrayRightController[1] = Instantiate(prefabPaintball);
        prefabArrayRightController[1].transform.SetParent(PlayerCameraRig.transform, false);

        // Add script on left controller for User-dynamic mode
        teleportController = prefabArrayRightController[1].AddComponent<TeleportController>();
        teleportController.enabled = false;
        menuLaserPointer = prefabArrayRightController[1].AddComponent<MenuLaserPointer>();
        menuLaserPointer.enabled = false; 
        paintballController = prefabArrayRightController[1].AddComponent<PaintballController>();
        //Default mode is 0 (User-static Mode)
        setMode(1); // set vlaue to 0 once merge done
        loadGameData();
    }

    void updateRightController()
    {
        if (isMenuActive)
        {
            paintballController.enabled = false;
            teleportController.enabled = false;
            menuLaserPointer.enabled = true;
        }
        else
        {
            if (PadIsPressing(deviceRight))
            {
                paintballController.enabled = false;
                menuLaserPointer.enabled = false;
                teleportController.enabled = true;
            }
            else
            {
                menuLaserPointer.enabled = false;
                teleportController.enabled = false;
                paintballController.enabled = true;
            }
        }

        if (MenuButtonIsPress(deviceRight))
        {
            if (isMenuActive)
            {
                isMenuActive = false;
            }
            else
            {
                isMenuActive = true;
            }
        }
    }

    void updateLeftController()
    { 
        
    }

    void setMode(int _mode)
    {
        prefabArrayRightController[mode].SetActive(false);
        prefabArrayRightController[_mode].SetActive(true);
        mode = _mode;
    }

    int queryMode()
    {
        return mode;

    }

    void loadGameData()
    { 
       // get data from previous scene


    }

    void SaveGameData()
    {
        //save data in the localapp folder
    }

    void ExportMesh()
    { 
    }

    private bool PadIsPressing(SteamVR_Controller.Device device)
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private bool PadIsUp(SteamVR_Controller.Device device)
    {
        return (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad));
    }

    private bool MenuButtonIsPress(SteamVR_Controller.Device device)
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.ApplicationMenu);
    }

}
