using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRadiusOfFire : MonoBehaviour
{
    public GameObject ControllerLeft;
    private float initialRadius;
    // Start is called before the first frame update
    void Start()
    {
        initialRadius = Weapons.radiusOfFire;
        ControllerLeft.GetComponent<SteamVR_TrackedController>().Gripped += ChangeBallSize;
    }

    private void ChangeBallSize(object sender, ClickedEventArgs e)
    {
        if (Weapons.radiusOfFire > initialRadius + 5)
        {
            Weapons.radiusOfFire = initialRadius;
        }
        else
        {
            Weapons.radiusOfFire += 1;
        }
    }
}
