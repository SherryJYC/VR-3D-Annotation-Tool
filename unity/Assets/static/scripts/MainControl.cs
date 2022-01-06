using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


public class MainControl : MonoBehaviour
{

    //Public attribute to attach Prefabs
    public GameObject ControllerLeft;
    public GameObject ControllerRight;
    public GameObject CameraRig;
    
    // public GameObject prefabPaintball;
    //public GameObject prefabSprayGunControl;

    public GameObject Meshes;
    public GameObject MeshesRGB ;
    public GameObject MeshesEmpty;

    //Private reference to prefab
    private GameObject[] prefabArrayLeftController = new GameObject[3];
    private int currentMode;

    public static Dictionary<Color, int> categoriesFromRGB;
    public int mode = 1;
    public float transform_rate = 1f;
    private string emptyPrefabs_dir;
    private string RGBPrefabs_dir;



    public static bool menuActive = false;
    public GameObject PrefabMenu;
    public static GameObject menu;
    
    public GameObject previous_OnMesh = null;
    public int previous_OnMesh_index = -1;
    
    public int Selected_Mesh_index = -1;
    
    public GameObject Selected_Mesh_empty = null;
    public GameObject Selected_Mesh_RGB = null;
    public GameObject cloned_Mesh_empty = null;
    private void Reset()
    {
             previous_OnMesh = null;
       previous_OnMesh_index = -1;

     Selected_Mesh_index = -1;

    Selected_Mesh_empty = null;
    Selected_Mesh_RGB = null;
    cloned_Mesh_empty = null;
}
    public GameObject MeshCursor;
    [System.Serializable]
    public class controller_state
    {
        private SteamVR_TrackedController controller;


        public controller_state(GameObject Controller)
        {
            controller = Controller.GetComponent<SteamVR_TrackedController>();
        }

        public bool triggerPressed() { return controller.triggerPressed; }
        public bool steamPressed() { return controller.steamPressed; }
        public bool menuPressed() { return controller.menuPressed; }
        public bool padPressed() { return controller.padPressed; }
        public bool padTouched() { return controller.padTouched; }
        public bool gripped() { return controller.gripped; }

    }
    [System.Serializable]
    public struct label
    {
        public int ID;
        public string labelname;
        public Color color;
    }

    public static label[] labellist;
    public controller_state left_controller_state;
    public controller_state right_controller_state;

    public GameObject HandelMeshesEmpty;
    public GameObject HandelMeshesRGB;


    void Start()
    {
        left_controller_state = new controller_state(ControllerLeft);
        right_controller_state = new controller_state(ControllerRight);
        ControllerRight.GetComponent<SteamVR_TrackedController>().MenuButtonClicked += MenuButton;
        menu = PrefabMenu;
        menuActive = false;


    }
    void Awake()
    {
 
    }
    void onSceneLoad() 
    {    
        HandelMeshesEmpty = GameObject.Find("HandelMeshesEmpty");
        HandelMeshesRGB = GameObject.Find("HandleMeshesRGB");
        Meshes = GameObject.Find("Meshes");
        MeshesRGB = Meshes.transform.Find("MeshesRGB").gameObject;
        MeshesEmpty =Meshes.transform.Find("MeshesEmpty").gameObject;

        rescaleMesh(Meshes, GameData.singleton.MeshData.x_size, GameData.singleton.MeshData.z_size);
        ChangeLayerOfMeshes();
        if(GameData.singleton.CurrentOnMeshName!= "NULL")
        {
           
            SelectMesh(GameData.singleton.CurrentOnMeshName, GameData.singleton.CurrentOnMeshlayer);

        }


    }


    public void TransformRate(float rate)
    {
        transform_rate = rate;
    }
    public float TransformRate() //overload
    {
        return transform_rate * controller_left_rescale;
    }
    public int query_mode()
    {
        return mode;
    }
    public float controller_left_rescale = 1f;

    public void controller_rescale(float rescale)
    {
        controller_left_rescale = rescale;
    }

    private void ChangeLayerOfMeshes(bool setBack = false)
    {

        string RGBLayer;
        string EmptyLayer;
        if (!setBack)
        {
            RGBLayer = "Selectable_RGB";
            EmptyLayer = "Selectable_Empty";
        }
        else
        {
            EmptyLayer = "Drawable";
            RGBLayer = "Teleportable";
        }
        GameObject Emptymesh = Meshes.transform.Find("MeshesEmpty").gameObject;
        GameObject RGBmesh = Meshes.transform.Find("MeshesRGB").gameObject;


        foreach (Transform child in Emptymesh.transform) {
            if (null == child)
                continue;
            child.gameObject.layer = LayerMask.NameToLayer(EmptyLayer);

        }
        foreach (Transform child in RGBmesh.transform)
        {
            if (null == child)
                continue;

            child.gameObject.layer = LayerMask.NameToLayer(RGBLayer);

        }


    }


    public void onSelectableMesh(string meshname, string layername)
    {   
        if (meshname!= "Null")
        {
            int currentIndex=-1;
            GameObject currentMesh=null;
            if( layername == "Selectable_RGB" )
            { 
               currentIndex = MeshController.singleton.GetIndexByNameRGB(meshname);
               currentMesh = Meshes.transform.Find("MeshesRGB").Find(meshname).gameObject;
            }
           else if (layername == "Selectable_Empty")
                { currentIndex = MeshController.singleton.GetIndexByName(meshname);
                currentMesh = Meshes.transform.Find("MeshesEmpty").Find(meshname).gameObject;
            }
                
           else return;


              if (previous_OnMesh_index ==-1)
            {
                previous_OnMesh_index = currentIndex;
                previous_OnMesh = currentMesh;
            }   
                
                else
            {
                
                if (currentIndex!= previous_OnMesh_index)
                {
                    if (previous_OnMesh_index != Selected_Mesh_index) previous_OnMesh.GetComponent<RenderBoundingBox>().SetMode("normal");
                    previous_OnMesh = currentMesh;
                    if (currentIndex != Selected_Mesh_index) previous_OnMesh.GetComponent<RenderBoundingBox>().SetMode("highlight");
                    previous_OnMesh_index = currentIndex;
                }    


            }



        }else
        {   if (previous_OnMesh_index == -1) return;
            if (previous_OnMesh_index!= Selected_Mesh_index) previous_OnMesh.GetComponent<RenderBoundingBox>().SetMode("normal");
            previous_OnMesh = null;
            previous_OnMesh_index = -1;
        }
    
    }

    public void SelectMesh(string meshname, string layername)
    {
        
        int currentIndex = -1;
        GameObject currentMeshRGB = null;
        GameObject currentMeshEmpty = null;

        if (layername == "Selectable_RGB")
        {
            currentIndex = MeshController.singleton.GetIndexByNameRGB(meshname);
            if (currentIndex == Selected_Mesh_index) return;
            currentMeshRGB = Meshes.transform.Find("MeshesRGB").Find(meshname).gameObject;
            String EmptyName = MeshController.singleton.meshName[currentIndex];
            currentMeshEmpty = Meshes.transform.Find("MeshesEmpty").Find(EmptyName).gameObject;
        }
        else if (layername == "Selectable_Empty")
        {
            currentIndex = MeshController.singleton.GetIndexByName(meshname);
            if (currentIndex == Selected_Mesh_index) return;
            currentMeshEmpty = Meshes.transform.Find("MeshesEmpty").Find(meshname).gameObject;
            String RGBName = MeshController.singleton.meshNameRGB[currentIndex];
            currentMeshRGB = Meshes.transform.Find("MeshesRGB").Find(RGBName).gameObject;
        }


        if (Selected_Mesh_index != -1)
        {
            Selected_Mesh_empty.GetComponent<RenderBoundingBox>().SetMode("normal");
            Selected_Mesh_RGB.GetComponent<RenderBoundingBox>().SetMode("normal");

        }

        Selected_Mesh_index = currentIndex;
        Selected_Mesh_RGB = currentMeshRGB;
        Selected_Mesh_empty = currentMeshEmpty;

        Selected_Mesh_empty.GetComponent<RenderBoundingBox>().SetMode("select");
        Selected_Mesh_RGB.GetComponent<RenderBoundingBox>().SetMode("select");
        
        
        MeshCursor.transform.SetParent(null);
        foreach (Transform child in HandelMeshesEmpty.transform)
        {
            if (null == child)
                continue;
            Destroy(child.gameObject);
        }
        foreach (Transform child in HandelMeshesRGB.transform)
        {
            if (null == child)
                continue;
            Destroy(child.gameObject);
        }
        //Vector3 localcenter = currentMeshEmpty.transform.position-currentMeshEmpty.transform.TransformPoint(currentMeshEmpty.GetComponent<MeshFilter>().mesh.bounds.center);
       
        Vector3 localcenter = currentMeshEmpty.GetComponent<MeshFilter>().mesh.bounds.center*transform_rate;

        Debug.Log("Mesh Center: " + localcenter);
        ControllerLeft.GetComponent<MeshControl>().setTransform(localcenter);

        GameObject MeshRGB_clone =Instantiate(currentMeshRGB, HandelMeshesRGB.transform);
        MeshRGB_clone.layer = LayerMask.NameToLayer("Teleportable");
        
        GameObject MeshEmpty_clone = Instantiate(currentMeshEmpty, HandelMeshesEmpty.transform);
        MeshEmpty_clone.layer =   LayerMask.NameToLayer("Drawable");


        cloned_Mesh_empty = MeshEmpty_clone;
        
        ControllerLeft.GetComponent<MeshControl>().MeshAttached();
       
      


    }
    public void UpdateOnOrginal()
    {
        if (cloned_Mesh_empty == null) return;


        MeshFilter CloneMeshFilter = cloned_Mesh_empty.GetComponent<MeshFilter>();


        LODMeshes lodMeshes = Selected_Mesh_empty.GetComponent<LODMeshes>();
        MeshFilter meshFilter = Selected_Mesh_empty.GetComponent<MeshFilter>();
        
        
        lodMeshes.mainColor = CloneMeshFilter.mesh.colors;
        meshFilter.mesh.colors= CloneMeshFilter.mesh.colors;
    }    


    void Update()
    {
        if (GameData.singleton.Static_unloaded)
         {
            onSceneLoad();
            GameData.singleton.Static_unloaded = false;
        }
    }

    public static void MenuButton(object sender, ClickedEventArgs e)
    {
        if (menuActive == false)
        {

            menuActive = true;
            menu.SetActive(true);
        }
        else
        {

            menuActive = false;
            menu.SetActive(false);
        }
    }


    public void SaveColorFile()
    {

        NewMenuOperation.SaveColorFile();
    }
    public void ExportMesh()
    {
        NewMenuOperation.ExportPLY();
    }

    public void LoadColorFile()
    {
        NewMenuOperation.LoadColorFile();


    }
    public void SwitchMode()
    {
        GameData.singleton.LastStaticMeshIndex = Selected_Mesh_index;
        
        Debug.Log("Switch to Dynamic");

        MeshesRGB.SetActive(false);
        MeshesEmpty.SetActive(true);
        ControllerLeft.GetComponent<ChangeVisualization>().lastscene = 1;
        ChangeLayerOfMeshes(true);
        Reset();
        Meshes.transform.localScale = new Vector3(1f, 1f, 1f);
        Meshes.transform.position = new Vector3(0f, 0f, 0f);
       
            string sceneName;
            string m_Scene;
            sceneName = "Assets/Scenes/VrAnnotationScene/UserDynamicScene.unity";
            m_Scene = "UserDynamicScene";
           
            StartCoroutine(LoadYourAsyncScene(m_Scene, sceneName));
        

    }
    IEnumerator LoadYourAsyncScene(string m_Scene, string sceneName)
    {



        AsyncOperation asyncLoad = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(sceneName, new LoadSceneParameters(LoadSceneMode.Additive));

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        
        // Wait until the last operation fully loads to return anything
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(Meshes, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(MeshController.singleton.gameObject, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(GameData.singleton.gameObject, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync("UserStatic_fix");
        

    }
    public void rescaleMesh(GameObject meshes,float mesh_x,float mesh_z)
    {
       
        float x_scale = GameObject.Find("Desk_Size_static").transform.localScale.x;
        float z_scale = GameObject.Find("Desk_Size_static").transform.localScale.z;
        float rescale_x = x_scale / (mesh_x / 10.0f);
        float rescale_z = z_scale / (mesh_z / 10.0f);
        float rescale = Math.Min(rescale_z, rescale_x);
        Vector3 center = MeshController.caculateCenter(MeshController.singleton.meshesEmpty);
        transform_rate = rescale;
        MeshControl.ScaleAround(meshes, center, new Vector3(rescale, rescale, rescale));
        //meshes.transform.localScale=new Vector3(rescale, rescale, rescale);

        meshes.transform.position = GameObject.Find("Desk_Size_static").transform.position + (GameObject.Find("Desk_Size_static").transform.position - center) * rescale;
        
    }
}
