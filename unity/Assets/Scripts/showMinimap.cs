using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowMinimap : MonoBehaviour
{
    public GameObject MiniMapPanel;
    public GameObject LeftController;
    private Vector3 offset;
    private GameObject playerCamera;
    float damp = 4.0f; // we can change the slerp velocity here
    static public bool isMiniMapActive = false; 

    // Called once when the gameobject wher the script is attached is activated
    void Awake()
    {
        
        //offset = new Vector3(0.0f, 0.6f, 0.4f);
        offset = new Vector3(0.0f, 0.6f, 0.0f);
    }

    private void Update()
    {
        if (LeftController.GetComponent<SteamVR_TrackedController>().triggerPressed)
        {
            isMiniMapActive = true;
            MiniMapPanel.SetActive(true);
            activateMiniMap();

        }
        else
        {
            MiniMapPanel.SetActive(false);
            isMiniMapActive = false;
        }
    }


    //private void switchScene(object sender, ClickedEventArgs e)
    //{
    //    Scene scene = SceneManager.GetActiveScene();

    //    if (scene.name == "UserDynamicScene")
    //    {
    //        SceneManager.LoadScene("UserStatic_fixed", LoadSceneMode.Single);

    //    }

    //}


    // Update is called once per frame
    private void activateMiniMap()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        MiniMapPanel.transform.position = LeftController.transform.position + offset;
        var rotationAngle = Quaternion.LookRotation(-1 * (playerCamera.transform.position - MiniMapPanel.transform.position)); // we get the angle has to be rotated
        MiniMapPanel.transform.rotation = Quaternion.Slerp(MiniMapPanel.transform.rotation, rotationAngle, Time.deltaTime * damp); // we rotate the rotationAngle 
    }
}
