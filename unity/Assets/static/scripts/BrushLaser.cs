using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class BrushLaser : MonoBehaviour
{

    public GameObject laserPrefab; // The laser prefab
  
    public Transform cameraRigTransform;
    public Transform eyeTransform;
    public Transform headTransform; // The camera rig's head
    public GameObject controllerRight;
    public LayerMask teleportMask; // Mask to filter out areas where teleports are allowed
    public Vector3 teleportReticleOffset; // Offset from the floor for the reticle to avoid z-fighting
    public Transform muzzleTransform;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private GameObject laserTeleport; // A reference to the spawned laser
    private Transform laserTransform; // The transform component of the laser for ease of use
    private Transform teleportReticleTransform; // Stores a reference to the teleport reticle transform for ease of use
    private GameObject reticle; // A reference to an instance of the reticle
    private RaycastHit hit;
    private float distanceOfTeleport;
    private Vector3 hitPoint;
    private bool shouldTeleport; // True if there's a valid teleport target
    private float scale_x;
    private float scale_y;
    private void Start()
    {
        laserTeleport = Instantiate(laserPrefab);
        laserTeleport.transform.parent = transform;
        laserTransform = laserTeleport.transform;
        
        distanceOfTeleport = 100;
       

        trackedObj = controllerRight.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        scale_x = laserTransform.localScale.x *0.3f;
        scale_y = laserTransform.localScale.y*0.3f;
    }

    private void Update()
    {
        Teleport();
    }



    private void Teleport()
    {
        if (ShootRay())
        {
            hitPoint = hit.point;

            ShowLaser(hit);


        }
        else { 
            laserTeleport.SetActive(false); 
        }
      
    }


    private void ShowLaser(RaycastHit hit)
    {
        laserTeleport.SetActive(true); //Show the laser

        laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        laserTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        laserTransform.localScale = new Vector3(scale_x, scale_y,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point
    }

    private bool ShootRay()
    {
        return Physics.Raycast(muzzleTransform.position, muzzleTransform.forward, out hit, distanceOfTeleport, teleportMask);
    }
}
