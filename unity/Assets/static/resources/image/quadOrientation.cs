using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class quadOrientation : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject quad;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        quad.transform.position = transform.parent.position;
        Vector3 dir = transform.parent.forward;
        dir.y = 0.0f;
        quad.transform.rotation = Quaternion.LookRotation(dir);
    }
}
