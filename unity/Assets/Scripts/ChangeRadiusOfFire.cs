using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRadiusOfFire : MonoBehaviour
{
    public GameObject ControllerLeft;
    private float initialRadius;
    private float incrementFactor = 5;
    private float maxRadius = 10.0f; 
   
    // Start is called before the first frame update
    void Start()
    {
        initialRadius = Weapons.radiusOfFire;
        ControllerLeft.GetComponent<SteamVR_TrackedController>().Gripped += ChangeBallSize;
    }

    private void ChangeBallSize(object sender, ClickedEventArgs e)
    {
        if (Weapons.radiusOfFire > maxRadius)
        {
            Weapons.radiusOfFire = initialRadius;
        }
        else
        {
            Weapons.radiusOfFire += incrementFactor;
        }
    }
}
