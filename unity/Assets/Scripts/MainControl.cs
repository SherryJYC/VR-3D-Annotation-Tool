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
    public GameObject ControllerLeft;
    public GameObject ControllerRight;
    public GameObject CameraRig;
    public GameObject prefabPaintball;
    public GameObject prefabShootingLabelControl;
    public GameObject prefabSprayGunControl;

    //Private reference to prefab
    private GameObject[] prefabArrayLeftController = new GameObject[3];
    private int currentMode;

    public static Dictionary<Color, int> categoriesFromRGB;
    private int mode = 0;

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
        prefabArrayLeftController[0] = Instantiate(prefabShootingLabelControl);
        prefabArrayLeftController[1] = Instantiate(prefabPaintball);
        prefabArrayLeftController[2] = Instantiate(prefabSprayGunControl);

        for (int index = 0; index < prefabArrayLeftController.Length; index++)
        {
            prefabArrayLeftController[index].transform.SetParent(CameraRig.transform, false);
            prefabArrayLeftController[index].SetActive(false);
            //GameObject.Instantiate(prefabArrayLeftController[index]);
            //prefabArrayLeftController[index].GetComponent<Transform>().SetParent(ControllerRight.transform, false);

            //By default the first controller of the list is left activated
        }
        setMode(mode);

        loadGameData();

    }

    void setMode(int _mode)
    {
        prefabArrayLeftController[mode].SetActive(false);
        prefabArrayLeftController[_mode].SetActive(true);
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

  

}
