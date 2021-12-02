using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class MeshController : MonoBehaviour
{

   
    public GameObject meshesRGB, meshesEmpty;

    protected string[] emptyNameFiles;
    protected string[] rgbNameFiles;

    private string emptyPrefabs_dir;
    private string RGBPrefabs_dir;

    
    protected int numberOfChuncks;
    public static Dictionary<string, int[]> faceLabel;
    public static MeshController singleton = null;
    public List<string> meshName = new List<string>();
    //public MeshModified meshesModified;


    public void load(string empty_dir, string rgb_dir)

    {
        if (MeshController.singleton == null)
        {
            emptyPrefabs_dir = empty_dir;
            RGBPrefabs_dir = rgb_dir;
             singleton = this;
            meshesEmpty = GameObject.Find("MeshesEmpty");
            meshesRGB = GameObject.Find("MeshesRGB");
            faceLabel = new Dictionary<string, int[]>();
            InstatiatePrefabs();
            InstatiatePrefabs(true);

        }
     
    }
    

    public int GetIndexByName(string name)
    {
        if (meshName.Contains(name))
        {
            for (int i = 0; i < meshName.Count; i++)
            {
                if (meshName[i] == name)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    public int Count()
    {
        return meshName.Count;
    }

    public void InstatiatePrefabs(bool isRGB = false)
    {
        if (isRGB)
        {
            rgbNameFiles = getListFiles(RGBPrefabs_dir);
            for (int i = 0; i < rgbNameFiles.Length; i++)
            {
                string nameFile = rgbNameFiles[i].Substring(Application.dataPath.Length + "Resources".Length + 2);
                nameFile = nameFile.Split('.')[0];
                //string nameFile = "static/mesh/rgb/refine";
                print(nameFile);
                UnityEngine.Object pPrefab = Resources.Load(nameFile);
                LoadPrefab(i, pPrefab, true);
               
            }
        }
        else
        {
            UnityEngine.Object[] emptyPrefabs = Resources.LoadAll(emptyPrefabs_dir);
            for (int i = 0; i < emptyPrefabs.Length; i++)
            {
                LoadPrefab(i, emptyPrefabs[i], false);
            }
        }
    }

    private void LoadPrefab(int i, UnityEngine.Object pPrefab, bool isRgb = false)
    {
        GameObject pNewObject = (GameObject)GameObject.Instantiate(pPrefab);

        if (isRgb)
        {
            pNewObject.transform.parent = meshesRGB.transform;
            pNewObject.SetActive(true);
        }
        else
        {
            pNewObject.transform.parent = meshesEmpty.transform;

           
            if (pNewObject.transform.childCount == 0)
            {
                Mesh region = pNewObject.GetComponent<LODMeshes>().highPolyMesh;
                int[] faceLabelArray = new int[region.triangles.Length / 3];
                faceLabel.Add(pNewObject.name, faceLabelArray);
            }
            else if (pNewObject.transform.childCount > 0)
            {
                for (int j = 0; j < pNewObject.transform.childCount; j++)
                {
                    GameObject child = pNewObject.transform.GetChild(j).gameObject;
                    Mesh region = child.GetComponent<LODMeshes>().highPolyMesh;
                    int[] faceLabelArray = new int[region.triangles.Length / 3];
                    faceLabel.Add(child.name, faceLabelArray);
                }
            }
        }
    }

    protected string[] getListFiles(String s)
    {
        string[] nameFiles = Directory.GetFiles(Application.dataPath + s, "*.prefab");
        return nameFiles;
    }

   
    
}

