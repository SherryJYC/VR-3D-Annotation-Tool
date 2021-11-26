using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControl : MonoBehaviour
{
    public Camera MiniMapCamera;
    public GameObject MiniMapMenu;
    public Material HighlightMaterial;
    private Transform currentPlayerTransform;
    private List<GameObject> areaSectionList;
    // Start is called before the first frame update
    void Start()
    {

        currentPlayerTransform = MainControl.PlayerCameraRig.transform;
        areaSectionList = new List<GameObject>(GameObject.FindGameObjectsWithTag("MiniMap"));
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
}
