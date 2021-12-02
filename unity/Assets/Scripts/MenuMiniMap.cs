using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMiniMap : MonoBehaviour
{
    public GameObject laserPrefab;
    private GameObject laser; // A reference to the spawned laser
    public RectTransform ImageRendererMap;
    public LayerMask menuMask; // Mask to filter out areas where menu button 
    public GameObject controllerRight;
    private SteamVR_TrackedObject trackedObjRight;

    private Camera TopCamera;
    private Camera PlayerCamera;
    private Vector3 cursorPositionWorldSpace;
    private Transform laserTransform; // The transform component of the laser for ease of use
    private Vector3 hitPoint; // Point where the raycast hits
    void Awake()
    {
        TopCamera = GameObject.FindGameObjectWithTag("MiniMapCamera").GetComponent<Camera>();
        PlayerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        GameObject controllerRight =  GameObject.Find("Controller (right)");
        trackedObjRight = controllerRight.GetComponent<SteamVR_TrackedObject>();
        laser = Instantiate(laserPrefab);
        laser.transform.parent = transform;
        laserTransform = laser.transform;

    }

    void Update()
    {
        RaycastHit hit;
        Debug.Log(Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 1000, menuMask));
        Debug.Log(hit.point);
        if (Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 1000, menuMask)) //Layer MiniMap = 13
        {
            ShowLaser(hit);
            Debug.Log(gameObject.GetComponent<MenuMiniMap>().TransformeCursorHitToWorldCoordinates(hit.point));
        }
        else
        {
            laser.SetActive(false);
        }

    }

    public Vector3 TransformeCursorHitToWorldCoordinates(Vector3 hitWorldPosition)
    {
        Vector2 screenPoint = PlayerCamera.WorldToScreenPoint(hitWorldPosition);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
           ImageRendererMap,
           screenPoint,
           PlayerCamera,
           out localPoint
        );
        var rect = ImageRendererMap.rect;
        localPoint.x /= rect.width;
        localPoint.y /= rect.height;
        Ray ray = TopCamera.ViewportPointToRay(localPoint);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float d;
        plane.Raycast(ray, out d);
        Vector3 hitPlane = ray.GetPoint(d);
        //PlayerCamera.transform.position = new Vector3(hit.x, .transform.position.y, hit.z);


        // Ccorrect the offset f the pivot
        //localPoint.x = (localPoint.x / rect.width) + map.rectTransform.pivot.x;
        //localPoint.y = (localPoint.y / rect.height) + map.rectTransform.pivot.y;

        return hitPlane; 
    }

    private void ShowLaser(RaycastHit hit)
    {
        laser.SetActive(true); //Show the laser

        laserTransform.position = Vector3.Lerp(trackedObjRight.transform.position, hit.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hit.point); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }

    //private void TriggerClicked()
    //{

    //    RaycastHit hit;

    //    // Send out a raycast from the controller
    //    if ((Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 100, menuMask)) && menuActive)
    //    {
    //        hitPoint = hit.point;
    //        GameObject buttonMenu = GameObject.Find(hit.transform.name);
    //        //Debug.Log(buttonMenu.name);


    //        menuActive = false;
    //        menu.SetActive(false);
    //        MenuController.OpenMenu(buttonMenu.name, hit, otherMenu);

    //        otherMenu = true;
    //        ShowLaser(hit);

    //    }
    //    else if ((Physics.Raycast(trackedObjRight.transform.position, trackedObjRight.transform.forward, out hit, 100, menuMask)) && otherMenu)
    //    {

    //        menuActive = true;
    //        menu.SetActive(true);
    //        MenuController.OpenMenu(hit.transform.name, hit, otherMenu);
    //        otherMenu = false;
    //    }
    //    else
    //    {
    //        laser.SetActive(false);
    //    }

    //}
}
