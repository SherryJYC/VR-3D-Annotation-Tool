using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMiniMap : MonoBehaviour
{
    public GameObject LaserCursor;
    public GameObject PanelMap;
    public RectTransform ImageRendererMap;

    private Camera TopCamera;
    private Camera PlayerCamera;
    private Vector3 cursorPositionWorldSpace;  
    void Start()
    {
        TopCamera = MainControl.TopCameraMiniMap.GetComponent<Camera>();
        PlayerCamera = MainControl.PlayerCameraRig.GetComponent<Camera>();
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
}
