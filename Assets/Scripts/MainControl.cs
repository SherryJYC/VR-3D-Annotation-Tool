using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MainControl : MonoBehaviour
{
    
    
    public GameObject controllerRight;
    public GameObject controllerLeft;

    public static Dictionary<Color, int> categoriesFromRGB;
    private int _mode;

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
        controllerLeft = GameObject.Find("Controller (left)");
        controllerRight = GameObject.Find("Controller (right)");
        loadGameData();

    }

    void setMode(int mode)
    {
        _mode = mode;
    }

    int queryMode()
    {
        return _mode;

    }

    void loadGameData()
    { 
       // get data from previous scene


    }

  

    void Update()
    {
        
    }
}
