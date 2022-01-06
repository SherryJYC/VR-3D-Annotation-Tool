using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class MeshController : MonoBehaviour
{


    public GameObject meshesRGB, meshesEmpty;



    public static Dictionary<string, int[]> faceLabel;
    public static MeshController singleton = null;
    public List<string> meshName = new List<string>();
    public List<string> meshNameRGB = new List<string>();
    //public MeshModified meshesModified;


    void Awake()
    {



        

        LoadUserConfiguration();

        if (MeshController.singleton == null)
        {

            singleton = this;
            meshesEmpty = GameObject.Find("MeshesEmpty");
            meshesRGB = GameObject.Find("MeshesRGB");
            faceLabel = new Dictionary<string, int[]>();


        }
     
  

    }
    private void Update()
    {

    }

    public void ResetMesh()
    {
        meshName = new List<string>();
        faceLabel = new Dictionary<string, int[]>();
        foreach (Transform child in meshesRGB.transform) GameObject.Destroy(child.gameObject);
        foreach (Transform child in meshesEmpty.transform) GameObject.Destroy(child.gameObject);

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
    public int GetIndexByNameRGB(string name)
    {
        if (meshNameRGB.Contains(name))
        {
            for (int i = 0; i < meshNameRGB.Count; i++)
            {
                if (meshNameRGB[i] == name)
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



    public void LoadPrefab(int i, GameObject pNewObject, bool isRgb = false)
    {


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
    private void LoadUserConfiguration()
    {
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/Resources/config.txt");
        string line;
        int found = 0;

        while ((line = file.ReadLine()) != null)
        {
            found = line.IndexOf(", ");
            if (!string.Equals("#", line.Substring(0, 1)))
            {
                if (string.Equals("pointcloud", line.Substring(0, found)))
                {
                    //this.pointcloud = bool.Parse(line.Substring(found + 2, line.Length - found - 2));
                }
            }
        }

        file.Close();
    }
    public static Vector3 caculateCenter(GameObject target)
    {

        MeshFilter[] filters = target.GetComponentsInChildren<MeshFilter>();
        Vector3 average_center = new Vector3(0f, 0f, 0f);
        int mesh_counts = 0;

        foreach (MeshFilter filter in filters)
        {
            average_center += filter.gameObject.transform.TransformPoint(filter.mesh.bounds.center);
            mesh_counts++;
        }
        average_center = average_center / mesh_counts;
        Debug.Log(average_center);
        return average_center;


    }


}

