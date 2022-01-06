using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class GameData : MonoBehaviour
{
    public static GameData singleton = null;
    public bool Static_unloaded = true;
    public int numberofsaves =0;
    public bool NewSession=true;
    public JsonData MeshData;
    public string CurrentOnMeshName = "NULL";
    public string CurrentOnMeshlayer = "NULL";

    public int LastStaticMeshIndex = -1;
    public void Awake()
    {
         if (GameData.singleton == null)
        {
            singleton = this;
        }


    }




}
