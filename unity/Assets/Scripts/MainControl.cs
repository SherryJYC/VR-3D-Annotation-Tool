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
    public GameObject prefabShootingLabelControl;
    public GameObject prefabSprayGunControl;

    //Private reference to prefab
    private GameObject[] prefabArray = new GameObject[3];
    private int currentMode;

    public static Dictionary<Color, int> categoriesFromRGB;
    private int mode;

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
        prefabArray[0] = prefabShootingLabelControl;
        prefabArray[1] = prefabPaintball;
        prefabArray[2] = prefabSprayGunControl;

        for (int index = 0; index <= prefabArray.Length; index++)
        {
            GameObject.Instantiate(prefabArray[index]);
            //prefabArray[index].GetComponent<Transform>().SetParent(transform, false);

            //By default the first controller of the list is left activated
            if (index == 0)
            {
                currentMode = 0;
            }
            else
            {
                prefabArray[index].SetActive(false);
            }
        }

        loadGameData();

    }

    void setMode(int _mode)
    {
        prefabArray[mode].SetActive(false);
        prefabArray[_mode].SetActive(true);
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
