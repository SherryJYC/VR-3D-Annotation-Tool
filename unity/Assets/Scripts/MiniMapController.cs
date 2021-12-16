using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour
{
    public GameObject controllerLeft;
    private SteamVR_TrackedController controller;
    public static GameObject meshRGB;
    public static GameObject meshEmpty;
    private static int lastscene = 1; //1 is for empty 2 is for RGB
    public Camera MiniMapCamera;
    public GameObject MiniMapMenu;
    public Material HighlightMaterial;
    private Transform currentPlayerTransform;
    private List<GameObject> areaSectionList;
    // Start is called before the first frame update
    void Start()
    {
        currentPlayerTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        areaSectionList = new List<GameObject>(GameObject.FindGameObjectsWithTag("MiniMap"));
       
        meshEmpty = GameObject.Find("MeshesEmpty");
        meshRGB = GameObject.Find("MeshesRGB");
        meshEmpty.SetActive(false);
        meshRGB.SetActive(true);
        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += ChangeColorMode;
        controller.TriggerClicked += ShowMiniMap;
        controller.TriggerUnclicked += HideMiniMap;
    }

    public void TeleportPlayerToNextRoom(Vector3 targetPosition)
    {
        //Vector3 currentPosition = MiniMapCamera.ScreenToWorldPoint(menuTransform.position);
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        
        foreach (GameObject section in areaSectionList)
        {
            Vector3 directionToTarget = section.transform.position - targetPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = section;
            }
        }
        //TODO: Fade out animation and Teleport
        //currentPlayerTransform = bestTarget.transform;
        Graphics.DrawMesh(bestTarget.GetComponent<Mesh>(), bestTarget.transform.position, bestTarget.transform.rotation, HighlightMaterial, 14, MiniMapCamera);
    }
    private void ChangeColorMode(object sender, ClickedEventArgs e)
    {
        if (!MenuLaserPointer.menuActive)
        {
            if (meshRGB.activeSelf)
            {
                lastscene = 1;
                meshRGB.SetActive(false);
                meshEmpty.SetActive(true);
            }
            else
            {
                lastscene = 2;
                meshRGB.SetActive(true);
                meshEmpty.SetActive(false);
            }
        }
    }
    private void ShowMiniMap(object sender, ClickedEventArgs e)
    {
        MiniMapMenu.SetActive(true);
    }
    private void HideMiniMap(object sender, ClickedEventArgs e)
    {
        MiniMapMenu.SetActive(false);
    }
}
