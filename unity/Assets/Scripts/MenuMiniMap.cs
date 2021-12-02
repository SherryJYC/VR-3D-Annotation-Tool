using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMiniMap : MonoBehaviour
{
    public GameObject LeftController;
    private Vector3 offset;
    private GameObject playerCamera;
    float damp = 5.0f; // we can change the slerp velocity here
    

    void Awake()
    {
        LeftController = GameObject.Find("Controller (left)");
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
        offset = new Vector3(0.0f,0.6f,0.6f);
    }

    void Update()
    {
        transform.position = LeftController.transform.position + offset;
        var rotationAngle = Quaternion.LookRotation(-1*(playerCamera.transform.position - transform.position)); // we get the angle has to be rotated
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, Time.deltaTime * damp); // we rotate the rotationAngle 
    }

    //public Vector3 TransformeCursorHitToWorldCoordinates(Vector3 hitWorldPosition)
    //{
        //Vector2 screenPoint = PlayerCamera.WorldToScreenPoint(hitWorldPosition);
        //Vector2 localPoint;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //   ImageRendererMap,
        //   screenPoint,
        //   PlayerCamera,
        //   out localPoint
        //);
        //var rect = ImageRendererMap.rect;
        //localPoint.x /= rect.width;
        //localPoint.y /= rect.height;
        //Ray ray = TopCamera.ViewportPointToRay(localPoint);
        //Plane plane = new Plane(Vector3.up, Vector3.zero);
        //float d;
        //plane.Raycast(ray, out d);
        //Vector3 hitPlane = ray.GetPoint(d);
        //PlayerCamera.transform.position = new Vector3(hit.x, .transform.position.y, hit.z);


        // Ccorrect the offset f the pivot
        //localPoint.x = (localPoint.x / rect.width) + map.rectTransform.pivot.x;
        //localPoint.y = (localPoint.y / rect.height) + map.rectTransform.pivot.y;

        //return hitPlane;
    //}

}
