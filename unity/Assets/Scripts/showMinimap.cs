using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class showMinimap : MonoBehaviour
{
    public GameObject ControllerRight;
    // Start is called before the first frame update
    void Start()
    {
        ControllerRight.GetComponent<SteamVR_TrackedController>().Gripped += switchScene;

    }

    private void switchScene(object sender, ClickedEventArgs e)
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == "UserDynamicScene")
        {
            SceneManager.LoadScene("MiniMapScene", LoadSceneMode.Single);

        }

    }

        // Update is called once per frame
        void Update()
    {
        
    }
}
