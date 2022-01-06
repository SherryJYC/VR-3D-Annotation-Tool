using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVisualization : MonoBehaviour {

    public GameObject controllerLeft;

    private SteamVR_TrackedController controller;
    public static GameObject meshRGB;
    public static GameObject meshEmpty;
    public int lastscene = 1; //1 is for empty 2 is for RGB

    // Use this for initialization
    void Start () {
        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += VisualizeRGB;

    }
    private void Awake()
    {
        if (MainMenuLaserPointer.Tutorial)
        {
            MeshController.singleton.meshesEmpty.SetActive(false);
            MeshController.singleton.meshesRGB.SetActive(false);
            meshEmpty = GameObject.Find("MeshesEmpty");
            meshRGB = GameObject.Find("MeshesRGB");
            meshRGB.SetActive(false);
            meshEmpty.SetActive(true);
        }
        else
        {
            meshRGB = MeshController.singleton.meshesRGB;
            meshEmpty = MeshController.singleton.meshesEmpty;
            meshRGB.SetActive(false);
            meshEmpty.SetActive(true);
        }
    }

    private void VisualizeRGB(object sender, ClickedEventArgs e)
    {
 
            if (lastscene ==2)
            {
                lastscene = 1;
                meshRGB.SetActive(false);
                meshEmpty.SetActive(true);
            }
            else
            {
                lastscene = 2;
                meshRGB.SetActive(true);
                meshEmpty.SetActive(false);
            }
        
    }

 
   
}
