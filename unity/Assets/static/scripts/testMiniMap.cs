using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class testMiniMap : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Mesh;
    public GameObject meshName_;
    void Start()
    {
       
        Bounds meshbound = Mesh.GetComponent<MeshCollider>().bounds;
        Vector3 boundsize = meshbound.size;
        Vector3 center = Mesh.transform.InverseTransformPoint(meshbound.center);
        Debug.Log(center);

        GameObject meshName = Instantiate(meshName_);
        meshName.name = "meshName";
        RectTransform rt = meshName.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(boundsize.x, boundsize.z);
        meshName.transform.SetParent(Mesh.transform);
        meshName.transform.localPosition = center + new Vector3(0f, boundsize.y / 2, 0f);



    }  

    // Update is called once per frame
    void Update()
    {
        
    }
}
