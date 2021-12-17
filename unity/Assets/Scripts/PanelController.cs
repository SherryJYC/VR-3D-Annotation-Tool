using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    private Vector3 hitPoint; // Point where the raycast hits
    public static bool menuActive;
    public GameObject PrefabMenu;
    public static GameObject menu;
    // Use this for initialization
    void Start()
    {
        menu = PrefabMenu;
        menuActive = false;
        menu.SetActive(false);
    }
    public static void MenuButton(object sender, ClickedEventArgs e)
    {
        Camera playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        if (menuActive == false)
        {
            playerCamera.cullingMask = ~(1 << LayerMask.NameToLayer("Drawable"));
            menuActive = true;
            menu.SetActive(true);
        }
        else
        {
            playerCamera.cullingMask = -1;
            menuActive = false;
            menu.SetActive(false);
        }
    }
}