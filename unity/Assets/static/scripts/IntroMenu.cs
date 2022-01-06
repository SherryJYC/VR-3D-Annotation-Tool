using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.IO;
using UnityEngine.SceneManagement;

public class IntroMenu : MonoBehaviour
{
    // Start is called before the first frame update

    public List<MeshData_Unity> data = new List<MeshData_Unity>();

    public GameObject MeshButtonPrefab;
    public GameObject panelToAttachButtonsTo;
    public GameObject MeshInfo;
    private int selected_meshset = -1;
    public static int numberofsaves = 0;

    public GameObject meshes;
    public string current_active_panel = "load";
    public bool NewSession = true;
    public bool entry_static = true;

    public void setActivePannel(string panel)
    {
        current_active_panel = panel;
    }
    void Start()
    {
        string path = "Config/data";

        load_json(path);


        meshes = GameObject.Find("Meshes");
        add_menu_button();

        MeshInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "No Mesh Selected !";
        var directoryInfo = new DirectoryInfo("Assets/Data/Saves");
        numberofsaves = directoryInfo.GetDirectories().Length;
        MeshController.singleton.meshesEmpty.SetActive(false);
        MeshController.singleton.meshesRGB.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (current_active_panel == "load")
            checkMeshSelected();


    }
    void add_menu_button()
    {
        RectTransform rt = panelToAttachButtonsTo.GetComponent<RectTransform>();
        float ButtonWidth = MeshButtonPrefab.GetComponent<RectTransform>().rect.height;
        for (int i = 0; i < data.Count; i++)
        {
            GameObject button = (GameObject)Instantiate(MeshButtonPrefab, panelToAttachButtonsTo.transform, false);
            //button.transform.SetParent(panelToAttachButtonsTo.transform);
            int x = i;
            button.GetComponent<Button>().onClick.AddListener(delegate { MeshButtonClicked(x); });                                                                                     //Next line assumes button has child with text as first gameobject like button created from GameObject->UI->Butto
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data[i].meshset.name + "\n" + "No. " + data[i].Seq + "\n" + data[i].meshset.FolderDirectory;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, rt.sizeDelta.y + ButtonWidth);

        }

    }



    public void rescaleMesh()
    {
        float mesh_x = data[selected_meshset].meshset.x_size;
        float mesh_z = data[selected_meshset].meshset.z_size;
        float x_scale = GameObject.Find("Desk_Size").transform.localScale.x;
        float z_scale = GameObject.Find("Desk_Size").transform.localScale.z;
        float rescale_x = x_scale / (mesh_x / 10.0f);
        float rescale_z = z_scale / (mesh_z / 10.0f);
        float rescale = Math.Min(rescale_z, rescale_x);
        Vector3 center = MeshController.caculateCenter(MeshController.singleton.meshesRGB);

        ScaleAround(meshes, center, new Vector3(rescale, rescale, rescale));
        //meshes.transform.localScale=new Vector3(rescale, rescale, rescale);

        meshes.transform.position = GameObject.Find("Desk_Size").transform.position + (GameObject.Find("Desk_Size").transform.position - center) * rescale;

    }
    public void LoadButtonClicked()
    {
        ProcessMeshSet();
        MeshinfoUpdate();
        rescaleMesh();



    }
    void MeshButtonClicked(int MeshNo)
    {

        Debug.Log("Button clicked = " + MeshNo);
        if (MeshNo != selected_meshset) MeshController.singleton.ResetMesh();
        selected_meshset = MeshNo;
        MeshinfoUpdate();


    }
    void MeshinfoUpdate()
    {
        string meshloaded = data[selected_meshset].loaded ? " (Loaded)" : " (Not Loaded)";
        MeshInfo.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data[selected_meshset].meshset.name + meshloaded + "\n" + "Description: " + data[selected_meshset].meshset.Description + "\n" + "Location: " + data[selected_meshset].meshset.FolderDirectory + "\n" + "Number of Submesh: " + data[selected_meshset].MeshNum + "\n" + "Dimension(m): " + data[selected_meshset].meshset.x_size + "x" + data[selected_meshset].meshset.z_size;

    }
    void load_json(string path)
    {
        TextAsset textAsset = Resources.Load(path) as TextAsset;
        MeshData json_data = (MeshData)JsonConvert.DeserializeObject(textAsset.text, typeof(MeshData));
        int datacount = 0;
        foreach (JsonData element in json_data.datalist)
        {

            Debug.Log(element);
            MeshData_Unity tempo = new MeshData_Unity(element);
            tempo.Seq = datacount;
            data.Add(tempo);
            datacount++;
        }

    }
    public class MeshData_Unity
    {
        public JsonData meshset;
        public int Seq;
        public int MeshNum;
        public bool loaded = false;
        public MeshData_Unity(JsonData meshset_)
        {

            meshset = meshset_;
            MeshNum = meshset.submeshes.Length;
        }

    }
    private void checkMeshSelected()
    {
        if (selected_meshset == -1)
        {
            GameObject.Find("Start").GetComponent<Button>().interactable = false;
            GameObject.Find("LoadMesh").GetComponent<Button>().interactable = false;


        }
        else
        {


            GameObject.Find("LoadMesh").GetComponent<Button>().interactable = true;

            if (data[selected_meshset].loaded)
            { GameObject.Find("Start").GetComponent<Button>().interactable = true; }

        }


    }
    private void ProcessMeshSet()
    {
        if (selected_meshset != -1 && !data[selected_meshset].loaded)
        {
            PrecessNewMesh(data[selected_meshset].meshset);
            data[selected_meshset].loaded = true;
        }
    }

    private void PrecessNewMesh(JsonData selected_mesh)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Resources/Materials/Test 1.mat");
        Material linemat = AssetDatabase.LoadAssetAtPath<Material>("Assets/static/UI/LineMat.mat");

        string folderPath = Application.dataPath + selected_mesh.FolderDirectory + selected_mesh.EmptyDirectory;
        string folderPathRGB = Application.dataPath + selected_mesh.FolderDirectory + selected_mesh.RGBDirectory;
        string PrefabPath = Application.dataPath + selected_mesh.FolderDirectory + "/PrefabEmpty/";
        string PrefabPathRGB = Application.dataPath + selected_mesh.FolderDirectory + "/PrefabRGB/";


        if (!Directory.Exists(PrefabPathRGB)) Directory.CreateDirectory(PrefabPathRGB);

        if (!Directory.Exists(PrefabPath)) Directory.CreateDirectory(PrefabPath);


        int filecount = 0;

        //BLANK
        foreach (JsonMesh file in selected_mesh.submeshes)
        {
            string fileName = file.EmptyMesh;
            //string prefabPath = "Assets" +selected_mesh.FolderDirectory + "/PrefabEmpty/" + fileName + ".prefab";
            string localPath = "Assets" + selected_mesh.FolderDirectory + selected_mesh.EmptyDirectory + fileName + ".blend";

            //Check if mesh contains LOD

            //Empty Mesh
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(localPath);
            GameObject prefabInstatiated = Instantiate(prefab);
            prefabInstatiated.name = fileName;
            Mesh meshOfPrefab = AssetDatabase.LoadAssetAtPath<Mesh>(localPath);
            SplitMesh(meshOfPrefab);
            if (prefab != null && meshOfPrefab != null)
            {
                //Add Material to mesh renderer

                prefabInstatiated.GetComponent<Renderer>().material = mat;

                //Add LODMehses

                LODMeshes lodMehses = prefabInstatiated.AddComponent<LODMeshes>() as LODMeshes;
                lodMehses.highPolyMesh = meshOfPrefab;
                lodMehses.mediumPolyMesh = meshOfPrefab;
                lodMehses.lowPolyMesh = meshOfPrefab;
                lodMehses.distanceLOD1 = 50f;
                lodMehses.distanceLOD2 = 100f;

                //Add VisualizationFacesHidden
                //prefabInstatiated.AddComponent<VisualizationFacesHidden>();

                //Add Collider
                MeshCollider meshCollider = prefabInstatiated.AddComponent<MeshCollider>();
                meshCollider.sharedMesh = meshOfPrefab;
               

                //Add Bounding Box Renderer
                RenderBoundingBox BBRenderer = prefabInstatiated.AddComponent<RenderBoundingBox>();
                BBRenderer.linecolor = Color.red;
                BBRenderer.lineMaterial = linemat;

                //Change Layer to Drawable
                prefabInstatiated.layer = LayerMask.NameToLayer("Drawable");


                
                MeshController.singleton.LoadPrefab(filecount, prefabInstatiated, false);
                MeshController.singleton.meshName.Add(fileName);
               
                //UnityEngine.Object prefabObject = PrefabUtility.CreatePrefab(prefabPath, prefabInstatiated);
                //Destroy(prefabInstatiated);

                //RGB

                string fileNameRGB = file.RGBmesh;
                // string prefabPathRGB = "Assets" + selected_mesh.FolderDirectory + "/PrefabRGB/" + fileNameRGB + ".prefab";
                string localPathRGB = "Assets" + selected_mesh.FolderDirectory + selected_mesh.RGBDirectory + fileNameRGB + ".blend";

                GameObject prefabRGB = AssetDatabase.LoadAssetAtPath<GameObject>(localPathRGB);
                //prefabRGB.GetComponent<Renderer>().material = mat;

                GameObject prefabInstatiatedRGB = Instantiate(prefabRGB);
                prefabInstatiatedRGB.name = fileNameRGB;
                Mesh meshOfPrefabRGB = AssetDatabase.LoadAssetAtPath<Mesh>(localPathRGB);
                if (prefabRGB != null && meshOfPrefabRGB != null)
                {
                    //Add Material to mesh renderer                
                    //prefabInstatiatedRGB.GetComponent<Renderer>().material = mat;

                    //Add LODMehses



                    //Add Collider
                    MeshCollider meshColliderRGB = prefabInstatiatedRGB.AddComponent<MeshCollider>();
                    // use empty mesh as colider mesh instead of chunk
                    meshColliderRGB.sharedMesh = meshOfPrefab;
                    
                    //Add Bounding Box Renderer
                    RenderBoundingBox BBRendererRGB = prefabInstatiatedRGB.AddComponent<RenderBoundingBox>();
                    BBRendererRGB.linecolor = Color.green;
                    BBRendererRGB.lineMaterial = linemat;

                    //meshColliderRGB.sharedMesh = meshOfPrefabRGB;

                    ////Add VisualizationFacesHidden
                    //prefabInstatiatedRGB.AddComponent<VisualizationFacesHidden>();

                    //Change Layer to Drawable
                    prefabInstatiatedRGB.layer = LayerMask.NameToLayer("Teleportable");
                   
                    MeshController.singleton.LoadPrefab(filecount, prefabInstatiatedRGB, true);
                    MeshController.singleton.meshNameRGB.Add(fileNameRGB);
                    AddMeshName(prefabInstatiatedRGB, file.name);
                    // UnityEngine.Object prefabObjectRGB = PrefabUtility.CreatePrefab(prefabPathRGB, prefabInstatiatedRGB);
                    // 
                    // Destroy(prefabInstatiatedRGB);
                }
            }

            filecount++;
        }





    }
    public GameObject MeshNamePrefab;
    
    void AddMeshName(GameObject Mesh,string Name)
      {

        Bounds meshbound = Mesh.GetComponent<MeshCollider>().bounds;
        Vector3 boundsize = meshbound.size;
        Vector3 center = Mesh.transform.InverseTransformPoint(meshbound.center);
        Debug.Log(center);
        
        GameObject meshName = Instantiate(MeshNamePrefab);
        meshName.layer = LayerMask.NameToLayer("MeshName");
        meshName.GetComponent<TextMeshPro>().text = Name;
        meshName.name = "meshName";
        RectTransform rt = meshName.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(boundsize.x, boundsize.z);
        meshName.transform.SetParent(Mesh.transform);
        meshName.transform.localPosition = center + new Vector3(0f, boundsize.y / 2, 0f);

      }


        void SplitMesh(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        Vector3[] newVerts;
        Vector3[] newNormals;
        Vector2[] newUvs;

        int n = triangles.Length;
        newVerts = new Vector3[n];
        newNormals = new Vector3[n];
        newUvs = new Vector2[n];

        for (int i = 0; i < n; i++)
        {
            newVerts[i] = verts[triangles[i]];
            newNormals[i] = normals[triangles[i]];
            if (uvs.Length > 0)
            {
                newUvs[i] = uvs[triangles[i]];
            }
            triangles[i] = i;
        }
        mesh.vertices = newVerts;
        mesh.normals = newNormals;
        mesh.uv = newUvs;
        mesh.triangles = triangles;
    }

    public void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.position;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relataive scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.position = FP;
    }

    public void start()
    {
       
        meshes.transform.localScale = new Vector3(1f, 1f, 1f);
        meshes.transform.position = new Vector3(0f, 0f, 0f);
        //DontDestroyOnLoad(meshes);
        string sceneName;
        string m_Scene;
        
        if (!entry_static)
        {
             sceneName = "Assets/Scenes/VrAnnotationScene/UserDynamicScene.unity";
             m_Scene = "UserDynamicScene";
             SetGameData();
        }
        else
        {
             sceneName = "Assets/Scenes/VrAnnotationScene/UserStatic_fix.unity";
             m_Scene = "UserStatic_fix";
             SetGameData();
        }
        
        StartCoroutine(LoadYourAsyncScene(m_Scene, sceneName));
    }
    public void SetGameData()
    {
        GameData gamedata=GameData.singleton.gameObject.GetComponent<GameData>();
        gamedata.numberofsaves = numberofsaves;
        gamedata.MeshData = data[selected_meshset].meshset;
        gamedata.NewSession = NewSession;
        gamedata.Static_unloaded = true;
        gamedata.CurrentOnMeshlayer = "NULL";
        gamedata.CurrentOnMeshName  = "NULL";
        gamedata.LastStaticMeshIndex = -1;




    }
    public void setnextMode()
    {

        entry_static = !entry_static;

    }

    IEnumerator LoadYourAsyncScene(string m_Scene, string sceneName)
    {



        AsyncOperation asyncLoad = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(sceneName, new LoadSceneParameters(LoadSceneMode.Additive));

        while (!asyncLoad.isDone)
        {
            yield return null;
        }


        // Wait until the last operation fully loads to return anything

        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(meshes, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(MeshController.singleton.gameObject, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(GameData.singleton.gameObject, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync("menu");
    }
}
