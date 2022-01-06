using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    [SerializeField] private float defaultLength = 0.0f;
    [SerializeField] private GameObject dot = null;
    public LayerMask menuMask;

    public Camera Camera { get; private set; } = null;

    private LineRenderer lineRenderer = null;
    private VRInputModule inputModule = null;
    private bool onHit;
    private void Awake()
    {
        Camera = GetComponent<Camera>();
        Camera.enabled = false;

        lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.enabled = false;

        //dot.SetActive(false);
        
    }

    private void Start()
    {
        // current.currentInputModule does not work
        inputModule = EventSystem.current.gameObject.GetComponent<VRInputModule>();

    }

    private void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        // Use default or distance
        PointerEventData data = inputModule.Data;
        RaycastHit hit = CreateRaycast();


        //Debug.Log("hit  menu "+ data.pointerCurrentRaycast.distance);
        // If nothing is hit, set do default length
      
        float canvasDistance = data.pointerCurrentRaycast.distance == 0 ? defaultLength : data.pointerCurrentRaycast.distance;
        if (data.pointerCurrentRaycast.distance == 0)
           {
            dot.SetActive(false);
        }
        else
        {

            dot.SetActive(true);

        }
        // Get the closest one
        float targetLength = canvasDistance;

        // Default
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // Set position of the dot
        dot.transform.position = endPosition;

        // Set linerenderer
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);

    }

    private RaycastHit CreateRaycast()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward );
        onHit=Physics.Raycast(ray, out hit, 1000, menuMask);
        
        return hit;
    }
}