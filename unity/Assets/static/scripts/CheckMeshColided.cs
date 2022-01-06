using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMeshColided : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected" + other.name);
        string layername = LayerMask.LayerToName(other.gameObject.layer);

        if (layername == "Teleportable")
        {

            GameData.singleton.CurrentOnMeshlayer = "Selectable_RGB";


        }
        else if (layername == "Drawable")
        {
            GameData.singleton.CurrentOnMeshlayer = "Selectable_Empty";


        }
        else return;
        GameData.singleton.CurrentOnMeshName = other.name;
    }
}
