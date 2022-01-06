using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshControl : MonoBehaviour
{

    public GameObject controllerLeft;
    public GameObject mesh;
    public static GameObject pivot;
    public static GameObject rotation_indicator;
    private SteamVR_TrackedController controller;


    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device device;
    private Vector2 previous_touchpad;




    Vector2 touchpad;

    private float sensitivity = 3.5f;

    private Vector2 padScrolling;


    private float xtemp = float.MaxValue;  //variable for the function ModifiyRadius  

    private float magnification = 1.0F;

    private float deltaX = 0F;

    private float rescale = 1;
    private Vector3 mesh_scale;


    void Start()
    {
        mesh = GameObject.Find("HandleMesh");

        setTransform();
        pivot = gameObject.transform.Find("pivot_point").gameObject;
        pivot.GetComponent<MeshRenderer>().enabled = false;

        rotation_indicator = gameObject.transform.Find("pivot_point").transform.Find("rotation_indicator").gameObject;
        rotation_indicator.GetComponent<MeshRenderer>().enabled = false;
        trackedObj = controllerLeft.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObj.index);
        controller = controllerLeft.GetComponent<SteamVR_TrackedController>();
        controller.MenuButtonClicked += VisualizeRGB;



    }
    public void MeshAttached()
    {
        int currentStatus = gameObject.GetComponent<ChangeVisualization>().lastscene;
        foreach (Transform child in meshRGB.transform)
        {
            if (null == child)
                continue;
            child.gameObject.SetActive(currentStatus!=1);
        }
        foreach (Transform child in meshEmpty.transform)
        {
            if (null == child)
                continue;
            child.gameObject.SetActive(currentStatus==1);
        }

    }

    public void setTransform()
    {


        mesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        float initial_rate = GameObject.Find("ControlManager").GetComponent<MainControl>().transform_rate;
        mesh.transform.localScale = new Vector3(initial_rate, initial_rate, initial_rate);


    }
    public void setTransform(Vector3 meshcenter)
    {


        mesh.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        float initial_rate = GameObject.Find("ControlManager").GetComponent<MainControl>().transform_rate;
        mesh.transform.localScale = new Vector3(initial_rate, initial_rate, initial_rate);
        mesh.transform.localPosition -= meshcenter;
        mesh.transform.localPosition += new Vector3(0.0f, 0.0f, 0.2f);

    }
    private void Awake()
    {


        previous_touchpad = new Vector2(.0f, .0f);
        
        meshEmpty = GameObject.Find("HandelMeshesEmpty");
        meshRGB = GameObject.Find("HandleMeshesRGB");

    }

    private GameObject FindObject(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
    private bool PadIsPressing()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
    }

    protected virtual bool TriggerIsPressed()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Trigger);
    }

    protected virtual bool RightGripIsPressed()
    {
        return GameObject.Find("ControlManager").GetComponent<MainControl>().right_controller_state.gripped();
    }
    protected virtual bool TriggerIsUp()
    {
        return device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger);
    }


    private bool PadIsUp()
    {
        return (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad));
    }
    private bool PadIsTouching()
    {
        return device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
    }
    // Update is called once per frame
    void Update()
    {
        bool ischanging_rotation;
        bool ischanging_pivot;
        ischanging_rotation = changeRotation();
        ChangeScale();
        ischanging_pivot = setPivot();

        if (!(ischanging_rotation || ischanging_pivot))
        {
            pivot.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    bool changeRotation()
    {
        if (PadIsPressing())
        {
            pivot.GetComponent<MeshRenderer>().enabled = true;
            Vector2 touchpad = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
            float tiltAroundX = touchpad.x;
            float tiltAroundY = touchpad.y;
            //print("Pressing Touchpad");
            print(touchpad);

            Vector2 rot_axis2d = new Vector2(tiltAroundX, tiltAroundY);
            float rot_magnitude = rot_axis2d.magnitude;
            rot_axis2d = rot_axis2d.normalized;

            Vector3 blueaxis = pivot.transform.forward; //y
            Vector3 redaxis = pivot.transform.right; //x

            Vector3 rot_axis3d = rot_axis2d.y * redaxis - rot_axis2d.x * blueaxis;

            rot_axis3d = rot_axis3d.normalized;
            rotation_indicator.transform.forward = rot_axis3d;
            rotation_indicator.GetComponent<MeshRenderer>().enabled = true;
            mesh.transform.RotateAround(pivot.transform.position, rot_axis3d, rot_magnitude * sensitivity);

            return true;
        }
        else
        {


            rotation_indicator.GetComponent<MeshRenderer>().enabled = false;
            return false;
        }
    }
    public Vector3 previous_pose;
    public Vector3 previous_rot;
    bool setPivot()
    {
        if (TriggerIsPressed())
        {
        
            
            pivot.GetComponent<MeshRenderer>().enabled = true;
            mesh.transform.SetParent(null, true);


            //mesh.transform.position = previous_pose;
            //mesh.transform.eulerAngles = previous_rot;
            return true;
        }

        else
        {

            mesh.transform.SetParent(transform, true);
            //previous_pose = mesh.transform.position;
            //previous_rot = mesh.transform.eulerAngles;
            return false;

        }


    }


    void ChangeScale()
    {
        if (!PadIsPressing() && PadIsTouching() && RightGripIsPressed())
        {


            padScrolling = device.GetAxis();

            if (xtemp == float.MaxValue)
            {
                xtemp = padScrolling.x;

            }
            else
            {


                deltaX = xtemp - padScrolling.x;
                xtemp = padScrolling.x;
                if (deltaX >= -0.1 && deltaX <= 0.1)
                {

                    rescale += deltaX * magnification;


                    if (rescale < 0.5) rescale = 0.5F;
                    if (rescale > 5) rescale = 5f;

                    GameObject.Find("ControlManager").GetComponent<MainControl>().controller_rescale(rescale);
                    //mesh.transform.localScale = mesh_scale * rescale;
                    mesh_scale = new Vector3(1f, 1f, 1f) * GameObject.Find("ControlManager").GetComponent<MainControl>().TransformRate();
                    ScaleAround(mesh, pivot.transform.position, mesh_scale * rescale);
                }

            }
        }
    }
    public static void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
    {
        Vector3 A = target.transform.position;
        Vector3 B = pivot;

        Vector3 C = A - B; // diff from object pivot to desired pivot/origin

        float RS = newScale.x / target.transform.localScale.x; // relataive scale factor

        // calc final position post-scale
        Vector3 FP = B + C * RS;

        // finally, actually perform the scale/translation
        target.transform.localScale = newScale;
        target.transform.position = FP;
    }
    public static GameObject meshRGB;
    public static GameObject meshEmpty;
    //1 is for empty 2 is for RGB

    // Use this for initialization

  

    private void VisualizeRGB(object sender, ClickedEventArgs e)
    {
        int lastscene = gameObject.GetComponent<ChangeVisualization>().lastscene;
        if (!MenuLaserPointer.menuActive)
        {
            if (lastscene == 2)
            {

                foreach (Transform child in meshRGB.transform)
                {
                    if (null == child)
                        continue;
                   child.gameObject.SetActive(true);
                }
                foreach (Transform child in meshEmpty.transform)
                {
                    if (null == child)
                        continue;
                    child.gameObject.SetActive(false);
                }
                

            }
            else
            {
               
                foreach (Transform child in meshRGB.transform)
                {
                    if (null == child)
                        continue;
                    child.gameObject.SetActive(false);
                }
                foreach (Transform child in meshEmpty.transform)
                {
                    if (null == child)
                        continue;
                    child.gameObject.SetActive(true);
                }
            }
        }
    }

}
