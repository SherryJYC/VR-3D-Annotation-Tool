using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DynamicController : MonoBehaviour
{
    public GameObject controller;

    public static bool menuActive = false;
    public GameObject PrefabMenu;
    public static GameObject menu;

    public GameObject MeshesRGB;
    public GameObject MeshesEmpty;
    public GameObject ControllerLeft;
    // Use this for initialization

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        

    }
    void Start()
    {
        controller.GetComponent<SteamVR_TrackedController>().MenuButtonClicked += MenuButton;
        menu = PrefabMenu;
        menuActive = false;
        menu.SetActive(false);
        GameObject Meshes = GameObject.Find("Meshes");
        MeshesRGB = Meshes.transform.Find("MeshesRGB").gameObject;
        MeshesEmpty = Meshes.transform.Find("MeshesEmpty").gameObject;
        toLastStaticMeshCenter();
        ShiftMiniMapCamera();

    }

    private void ShiftMiniMapCamera()
    {
        var MiniCam=GameObject.Find("MiniMapCamera");
        var pose=MeshController.caculateCenter(MeshesEmpty);
        MiniCam.transform.position =new Vector3(pose.x,MiniCam.transform.position.y,pose.z);
        float size=Mathf.Max(GameData.singleton.MeshData.x_size, GameData.singleton.MeshData.z_size)/2*1.2f;
        MiniCam.GetComponent<Camera>().orthographicSize = size;
    }
    private void toLastStaticMeshCenter()
    { int last_index = GameData.singleton.LastStaticMeshIndex;
        if (last_index != -1)
        {
             string LastMeshName = MeshController.singleton.meshName[last_index];
            GameObject LastMesh = GameObject.Find(LastMeshName);
            Bounds meshbound = LastMesh.GetComponent<MeshCollider>().bounds;
            Vector3 pose=LastMesh.transform.TransformPoint(meshbound.center-new Vector3(0f, meshbound.size.y/2, 0f));
            GameObject.Find("PaintballGun").GetComponent<TeleportController>().Teleport(pose);
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
        MeshesRGB.SetActive(false);
        MeshesEmpty.SetActive(true);
        ControllerLeft.GetComponent<ChangeVisualization>().lastscene = 1;
        Debug.Log("Switch Mode");

        

        string sceneName;
        string m_Scene;
        sceneName = "Assets/Scenes/VrAnnotationScene/UserStatic_fix.unity";
        m_Scene = "UserStatic_fix";

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
        GameObject Meshes = GameObject.Find("Meshes");
        Meshes.transform.localScale = new Vector3(1f, 1f, 1f);
        Meshes.transform.position = new Vector3(0f, 0f, 0f);
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(Meshes, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(MeshController.singleton.gameObject, SceneManager.GetSceneByName(m_Scene));
        UnityEditor.SceneManagement.EditorSceneManager.MoveGameObjectToScene(GameData.singleton.gameObject, SceneManager.GetSceneByName(m_Scene));
        GameData.singleton.Static_unloaded = true;
        Debug.Log(GameData.singleton.Static_unloaded);
        UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync("UserDynamicScene");
    }

}
