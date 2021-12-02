using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


    [RequireComponent(typeof(MeshController))]

public class MainControl : MonoBehaviour
{

    //Public attribute to attach Prefabs
    public GameObject ControllerLeft;
    public GameObject ControllerRight;
    public GameObject CameraRig;
    public GameObject prefabPaintball;
    public GameObject prefabShootingLabelControl;
    public GameObject prefabSprayGunControl;

    public GameObject Meshes;

    //Private reference to prefab
    private GameObject[] prefabArrayLeftController = new GameObject[3];
    private int currentMode;

    public static Dictionary<Color, int> categoriesFromRGB;
    public int mode = 1;
    public float transform_rate=0.05f;
    private string emptyPrefabs_dir;
    private string RGBPrefabs_dir;
    MeshController meshcontroller;



    [System.Serializable]
   
    public struct label
    {
        public int ID;
        public string labelname;
        public Color color;
    }

    public static label[] labellist;

    void Start()
    {   /*
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
       */


    }
    void Awake()
    {
        loadGameData();
        meshcontroller = GetComponent<MeshController>();
        meshcontroller.load(emptyPrefabs_dir, RGBPrefabs_dir);
        setMode(mode);
    }

    public void TransformRate(float rate)
    {
        transform_rate = rate;
    }
    public float TransformRate() //overload
    {
        return transform_rate;
    }

    void setMode(int _mode)
    {
        if (mode == 1)
        {
            

            Meshes.transform.SetParent(ControllerLeft.transform );
            
            Meshes.transform.localPosition = new Vector3(0.0f, 0.0f, 0.15f);
            Meshes.transform.eulerAngles = new Vector3(52.8f,48.4f,45.0f);
            Meshes.transform.localScale= new Vector3(transform_rate, transform_rate, transform_rate);
            
        }    
    }

    int queryMode()
    {
        return mode;

    }









    void loadGameData()
    {
        //emptyPrefabs_dir = "/static/mesh/empty";
        //RGBPrefabs_dir = "/static/mesh/rgb";
        emptyPrefabs_dir = "Prefabs/EmptyMeshes/";
        RGBPrefabs_dir = "/Resources/Prefabs/RGBMeshes/";


    }

    void SaveGameData()
    {
        //save data in the localapp folder
    }

    void ExportMesh()
    {
    }

    string camera_rig_name()
    {
        if (mode == 0)
            return "[camera_rig]";
        else
            return "[camera_rig_1]";
    }

}
