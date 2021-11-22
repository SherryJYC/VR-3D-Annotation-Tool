using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZEffects;

public class ControllerManager : Weapons
{
    public EffectTracer TracerEffect;
    public GameObject laserPrefab; // The laser prefab
    public GameObject targetPrefab;

    private GameObject rayOfFire; // A reference to the spawned laser
    private GameObject targetOfFire;
    private Transform fireTransform; // The transform component of the laser for ease of use

    // Use this for initialization
    void Start()
    {
        Initialize();
        targetOfFire = Instantiate(targetPrefab);
        targetOfFire.transform.parent = transform;
        rayOfFire = Instantiate(laserPrefab);
        rayOfFire.transform.parent = transform;
        fireTransform = rayOfFire.transform;

        radiusOfFire = 1f;
        factorOfScale = 0.001f;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit costantRay;
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out costantRay, distanceOfShoot, shootableMask))
        {
            ShowLaserTarget(costantRay);
            if (TriggerIsPressed())
            {
                Animation();
                Fire();
            }
        }
        else
        {
            rayOfFire.SetActive(false);
            targetOfFire.SetActive(false);
        }
    }

    private void Animation()
    {
        device.TriggerHapticPulse(750);
        TracerEffect.ShowTracerEffect(muzzleTrasform.position, muzzleTrasform.forward, 250f);

    }

    private void Fire()
    {
        if (ShootRay() && !MenuLaserPointer.menuActive && !MenuLaserPointer.otherMenu)
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            GameObject meshHits;
            meshHits = GameObject.Find(hit.collider.name);
            MeshFilter meshFilter = meshHits.GetComponent<MeshFilter>();
            Mesh highPoly = meshFilter.mesh;

            if (meshCollider != null || meshCollider.sharedMesh != null)
            {
                ColorMesh(highPoly, hit, current_color, meshHits, false);
                SaveColorTemporary(meshHits);
            }

        }
    }

    private void ShowLaserTarget(RaycastHit target)
    {
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(muzzleTrasform.transform.position, target.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(target.point); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + radiusOfFire * 0.001F, 0.0F + radiusOfFire * 0.001F,
            target.distance);

        targetOfFire.SetActive(true);
        targetOfFire.transform.position = target.point;
        //targetOfFire.transform.LookAt(trackedObj.transform.position); // Rotate laser facing the hit point
        targetOfFire.transform.rotation = Quaternion.FromToRotation(Vector3.forward, target.normal);
        targetOfFire.transform.localScale = new Vector3(0.01f * radiusOfFire, 0.01f * radiusOfFire, targetOfFire.transform.localScale.z);
        //Change color of the laser
        rayOfFire.GetComponent<Renderer>().material.SetColor("_Color",Weapons.current_color); 
        targetOfFire.GetComponent<Renderer>().material.SetColor("_Color", Weapons.current_color);
    }

    protected override bool TriggerIsPressed()
    {
        // Single shot weapon.
        return device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
    }
}