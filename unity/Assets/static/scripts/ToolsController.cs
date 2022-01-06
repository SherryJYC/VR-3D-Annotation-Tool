using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class ToolsController : Weapons
{

    public float adjust_rate = 0.1f;
    public GameObject PowerText;

    private TextMesh powerText;
    public GameObject laserPrefab; // The laser prefab
    public GameObject targetPrefab;

    private GameObject rayOfFire; // A reference to the spawned laser
    private GameObject targetOfFire;
    private Transform fireTransform; // The transform component of the laser for ease of use

    private Vector2 padScrolling;
    public GameObject meshCursorPrefab;


    public static GameObject MeshCursor;
    private Mesh selectedMesh;

    private float xtemp = float.MaxValue;  //variable for the function ModifiyRadius
    private float ytemp = float.MaxValue;  //variable for the function ModifiyRadius  
    private double angle = 0F;
    private float magnification = 1.5F;
    private float deltaX = 0F;
    private float deltaY = 0F;

    public int mode;// 1 for spraygun, 0 for airbrush
    public LineRenderer lineRenderer;

    public LayerMask MenuMask;
    public LayerMask SelectableMeshMask;
    public bool updateHandleMeshOnOriginal=false;


    // Use this for initialization
    void Start()
    {
        Initialize();

        powerText = PowerText.GetComponent<TextMesh>();
        targetOfFire = Instantiate(targetPrefab);
        targetOfFire.transform.parent = transform;
        rayOfFire = Instantiate(laserPrefab);
        rayOfFire.transform.parent = transform;
        fireTransform = rayOfFire.transform;

        radiusOfFire = 1;
        factorOfScale = 0.001f;

        padScrolling = Vector2.zero;

        MeshCursor = meshCursorPrefab;
        selectedMesh = MeshCursor.GetComponent<MeshFilter>().mesh;
        lineRenderer = GetComponent<LineRenderer>();
    }

    void ToolOnEnable()
    {
        colorSelected = gameObject.transform.Find("ColorSelected").Find("ImageColorSelected").gameObject;

        GameObject.FindWithTag("ColorRadialMenu").transform.localScale = gameObject.transform.Find("RadialMenu_pose").localScale;
        GameObject.FindWithTag("ColorRadialMenu").transform.SetParent(gameObject.transform.Find("RadialMenu_pose"));
        GameObject.FindWithTag("ColorRadialMenu").transform.localPosition = new Vector3(0, 0, 0);
        GameObject.FindWithTag("ColorRadialMenu").transform.localEulerAngles = new Vector3(0, 0, 0);

    }
    void ToolOnDisable()
    {
        GameObject.FindWithTag("ColorRadialMenu").transform.SetParent(GameObject.Find("Controller (right)").transform);
    }

    // Update is called once per frame
    void Update()
    {
        ModifiyRadius();
        RaycastHit constantRay;


        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, MenuMask)|| Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, SelectableMeshMask))
        {
            lineRenderer.enabled = true;
            MeshCursor.SetActive(false);
            targetOfFire.SetActive(false);
            rayOfFire.SetActive(false);

            float targetLength = constantRay.distance;
            Vector3 endPosition = transform.position + (transform.forward * targetLength);

            // Set linerenderer
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, endPosition);
            if (updateHandleMeshOnOriginal)
            {
                updateHandleMeshOnOriginal = false;
                GameObject.Find("ControlManager").GetComponent<MainControl>().UpdateOnOrginal();

            }
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, SelectableMeshMask))
                {
                

                
                string layername = LayerMask.LayerToName(constantRay.collider.gameObject.layer);

                 GameObject.Find("ControlManager").GetComponent<MainControl>().onSelectableMesh(constantRay.collider.name,layername);


                if (TriggerIsPressed())
                {
                   
                GameObject.Find("ControlManager").GetComponent<MainControl>().SelectMesh(constantRay.collider.name, layername);
                }

                }

        }
        else if (Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, shootableMask))
        {
            GameObject.Find("ControlManager").GetComponent<MainControl>().onSelectableMesh("Null", "Null");
            lineRenderer.enabled = false;
            Renderer fire_render = rayOfFire.GetComponent<Renderer>();
            fire_render.material.SetColor("_Color", current_color);
            if (mode == 0)
            {
                MeshCursor.transform.SetParent(constantRay.collider.transform);
                MeshCursor.transform.localPosition = Vector3.zero;
                MeshCursor.transform.localRotation = Quaternion.identity;
                MeshCursor.transform.localScale = Vector3.one;

                MeshCursor.SetActive(true);
                HighlightMeshTarget(constantRay, current_color, 0);
            }
            ShowLaserTarget(constantRay);
          
            if (TriggerIsPressed())
            {

                Fire();
            }
            updateHandleMeshOnOriginal = true;
        }

        else
        {
            GameObject.Find("ControlManager").GetComponent<MainControl>().onSelectableMesh("Null", "Null");

            lineRenderer.enabled = false;
            MeshCursor.SetActive(false);
            rayOfFire.SetActive(false);
            targetOfFire.SetActive(false);
            
            if (updateHandleMeshOnOriginal)
            {
                updateHandleMeshOnOriginal = false;
                GameObject.Find("ControlManager").GetComponent<MainControl>().UpdateOnOrginal();

            }
        }

    }

    public void Fire()
    {   
        

      if (ShootRay() && !MenuLaserPointer.menuActive && !MenuLaserPointer.otherMenu)
        {
            {
                hitPoint = hit.point;

                MeshCollider meshCollider = hit.collider as MeshCollider;
                GameObject meshHits;
                meshHits = GameObject.Find(hit.collider.name);
                MeshFilter meshFilter = meshHits.GetComponent<MeshFilter>();
                Mesh highPoly = meshFilter.mesh;

                if (meshCollider != null || meshCollider.sharedMesh != null)
                {
                    ChangeFactorOfScale(highPoly);
                    
                    ColorMesh(highPoly, hit, current_color, meshHits, radiusOfFire > 1);
                    SaveColorTemporary(meshHits);
                    ShowFire(hit);
                }
            }
        }
    }

    private void RegisterAction(Mesh meshToRegister)
    {
        //Save the precedent situation to perform Undo action
        DataHit.Instance.CurrentIndex = 0;

        if (DataHit.Instance.OccupyASpace())
        {
            if (!DataHit.Instance.MeshName.Contains(hit.collider.name))
            {
                DataHit.Instance.MeshName.Add(hit.collider.name);
                DataHit.Instance.ColorPrec.Add(meshToRegister.colors);
                DataHit.Instance.TimeOrder.Add(DataHit.Instance.CurrentTime);
                DataHit.Instance.CurrentTime++;
                DataHit.Instance.FreeSpace--;
            }
            else
            {
                DataHit.Instance.UpdateArrayColor(hit.collider.name, meshToRegister.colors);
            }

        }
        else
        {
            DataHit.Instance.Remove(0);
            //Shift left
            ShiftLeft(DataHit.Instance.MeshName, 1);
            ShiftLeft(DataHit.Instance.ColorPrec, 1);

            if (!DataHit.Instance.MeshName.Contains(hit.collider.name))
            {
                DataHit.Instance.MeshName.Add(hit.collider.name);
                DataHit.Instance.ColorPrec.Add(meshToRegister.colors);
                DataHit.Instance.TimeOrder.Add(DataHit.Instance.CurrentTime);
                DataHit.Instance.CurrentTime++;
                DataHit.Instance.FreeSpace--;
            }
            else
            {
                DataHit.Instance.UpdateArrayColor(hit.collider.name, meshToRegister.colors);
            }

        }

    }

    private void ShiftLeft<T>(List<T> lst, int shifts)
    {
        for (int i = shifts; i < lst.Count; i++)
        {
            lst[i - shifts] = lst[i];
        }
    }

    private void ShowFire(RaycastHit hit)
    {
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(hitPoint); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + radiusOfFire * 0.001F, 0.0F + radiusOfFire * 0.001F,
            hit.distance); // Scale laser so it fits exactly between the controller & the hit point

    }

    private void ShowLaserTarget(RaycastHit target)
    {
        float rate = GameObject.Find("ControlManager").GetComponent<MainControl>().TransformRate();

        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(trackedObj.transform.position, target.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(target.point); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + radiusOfFire * 0.001F, 0.0F + radiusOfFire * 0.001F, target.distance);

        if (mode == 1)
        {



            targetOfFire.SetActive(true);
            targetOfFire.GetComponent<SpriteRenderer>().color = current_color;
            targetOfFire.transform.position = target.point;
            targetOfFire.transform.LookAt(trackedObj.transform.position); // Rotate laser facing the hit point
            targetOfFire.transform.localScale = new Vector3(adjust_rate * radiusOfFire * rate, adjust_rate * radiusOfFire * rate, targetOfFire.transform.localScale.z);
        }
        else
        {

            targetOfFire.SetActive(false);
        }

    }

    protected override bool ShootRay()
    {
        return Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, distanceOfShoot, shootableMask);
    }

    private bool PadIsPressing()
    {
        return device.GetPress(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private bool PadIsRelasing()
    {
        return device.GetTouchUp(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private bool PadIsTouching()
    {
        return device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad);
    }

    private void ModifiyRadius()
    {
        if (mode == 0)
        {
            radiusOfFire = 1.0f;

        }
        else
        {


            if (!PadIsPressing() && PadIsTouching())
            {


                padScrolling = device.GetAxis();

                if (xtemp == float.MaxValue && ytemp == float.MaxValue)
                {
                    xtemp = padScrolling.x;
                    ytemp = padScrolling.y;
                }
                else
                {

                    angle = Math.Atan2(padScrolling.y, padScrolling.x);
                    deltaX = xtemp - padScrolling.x;
                    deltaY = ytemp - padScrolling.y;

                    if (angle < Math.PI / 8 && angle >= -Math.PI / 8)
                    {
                        if (deltaY > 0)
                        {
                            radiusOfFire += deltaY * magnification;
                        }
                        if (deltaY < 0) radiusOfFire -= Math.Abs(deltaY) * magnification;
                    }
                    if (angle < 3 * Math.PI / 8 && angle >= Math.PI / 8)
                    {
                        if ((deltaX < 0) && (deltaY > 0))
                        {
                            radiusOfFire += deltaY * magnification;
                        }
                        if ((deltaX > 0) && (deltaY < 0)) radiusOfFire -= Math.Abs(deltaY) * magnification;
                    }
                    if (angle < 5 * Math.PI / 8 && angle >= 3 * Math.PI / 8)
                    {
                        if (deltaX < 0)
                        {
                            radiusOfFire += Math.Abs(deltaX) * magnification;
                        }
                        if (deltaX > 0) radiusOfFire -= deltaX * magnification;
                    }
                    if (angle < 7 * Math.PI / 8 && angle >= 5 * Math.PI / 8)
                    {
                        if ((deltaX < 0) && (deltaY < 0))
                        {
                            radiusOfFire += Math.Abs(deltaY) * magnification;
                        }
                        if ((deltaX > 0) && (deltaY > 0)) radiusOfFire -= deltaY * magnification;
                    }
                    if (angle < -7 * Math.PI / 8 || angle >= 7 * Math.PI / 8)
                    {
                        if (deltaY < 0)
                        {
                            radiusOfFire += Math.Abs(deltaY) * magnification;
                        }
                        if (deltaY > 0) radiusOfFire -= (deltaY) * magnification;
                    }
                    if (angle < -5 * Math.PI / 8 && angle >= -7 * Math.PI / 8)
                    {
                        if ((deltaX > 0) && (deltaY < 0))
                        {
                            radiusOfFire += Math.Abs(deltaX) * magnification;
                        }
                        if ((deltaX < 0) && (deltaY > 0)) radiusOfFire -= Math.Abs(deltaX) * magnification;
                    }
                    if (angle < -3 * Math.PI / 8 && angle >= -5 * Math.PI / 8)
                    {
                        if (deltaX > 0)
                        {
                            radiusOfFire += deltaX * magnification;
                        }
                        if (deltaX < 0) radiusOfFire -= Math.Abs(deltaX) * magnification;
                    }
                    if (angle < -Math.PI / 8 && angle >= -3 * Math.PI / 8)
                    {
                        if ((deltaX > 0) && (deltaY > 0))
                        {
                            radiusOfFire += deltaX * magnification;
                        }
                        if ((deltaX < 0) && (deltaY < 0)) radiusOfFire -= Math.Abs(deltaX) * magnification;
                    }

                    if (radiusOfFire < 1.0) radiusOfFire = 1.0F;
                    if (radiusOfFire > 20) radiusOfFire = 20.0f;

                    if (powerText != null)
                    {

                        powerText.text = String.Format("{0:0.0}", radiusOfFire);
                    }
                }
            }

            xtemp = padScrolling.x;
            ytemp = padScrolling.y;
        }
    }


    

    private void ShowTriangleTarget(RaycastHit hit)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1, Color.red, 10000f);
        Debug.DrawLine(p1, p2, Color.red, 10000f);
        Debug.DrawLine(p2, p0, Color.red, 10000f);

    }

    protected void HighlightMeshTarget(RaycastHit hit, Color col, int mode)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Color[] colors = mesh.colors;
        Vector3[] selectedVertices;
        int[] selectedTriangles;
        //int[] faces = null;
        int indexTriangleMesh = hit.triangleIndex * 3;
        int v0 = triangles[indexTriangleMesh + 0];
        int v1 = triangles[indexTriangleMesh + 1];
        int v2 = triangles[indexTriangleMesh + 2];
        List<int> listVeticesHit = new List<int>() { v0, v1, v2 };
        Vector3 p0 = vertices[v0];
        Vector3 p1 = vertices[v1];
        Vector3 p2 = vertices[v2];
        Vector3[] hitTriangle = new Vector3[] { p0, p1, p2 };

        Dictionary<int, Vector3> quadIndex = new Dictionary<int, Vector3>();

        switch (mode)
        {
            case 0:
                //Quad Selection
                List<int> closestEdgeIndex = closestEdgeToCursor(hitTriangle, hit.point);
                List<int> listClosestVerticesHit = new List<int>() {
                                                    triangles[indexTriangleMesh + closestEdgeIndex[0]],
                                                    triangles[indexTriangleMesh + closestEdgeIndex[1]] };
                Dictionary<int, int> dictionaryQuad = new Dictionary<int, int>(4);
                dictionaryQuad.Add(0, v0); //Starting point 

                if (closestEdgeIndex.Contains(0))
                {
                    if (closestEdgeIndex.Contains(1))
                    {
                        dictionaryQuad.Add(2, v1);
                        dictionaryQuad.Add(3, v2);
                    }
                    else
                    {
                        dictionaryQuad.Add(1, v1);
                        dictionaryQuad.Add(2, v2);
                    }
                }
                else
                {
                    dictionaryQuad.Add(1, v1);
                    dictionaryQuad.Add(3, v2);
                }


                for (int index = 0; index < triangles.Length / 3; index++)
                {
                    int t0 = triangles[index * 3 + 0];
                    int t1 = triangles[index * 3 + 1];
                    int t2 = triangles[index * 3 + 2];

                    if (listClosestVerticesHit.Contains(t0) && listClosestVerticesHit.Contains(t1) && !listVeticesHit.Contains(t2))
                    {
                        AddLastVerticeQuad(ref dictionaryQuad, t2);
                        break;
                    }

                    if (listClosestVerticesHit.Contains(t0) && listClosestVerticesHit.Contains(t2) && !listVeticesHit.Contains(t1))
                    {
                        AddLastVerticeQuad(ref dictionaryQuad, t1);
                        break;
                    }

                    if (listClosestVerticesHit.Contains(t1) && listClosestVerticesHit.Contains(t2) && !listVeticesHit.Contains(t0))
                    {
                        AddLastVerticeQuad(ref dictionaryQuad, t0);
                        break;
                    }

                }

                if (dictionaryQuad.Count == 4)
                {
                    selectedVertices = new Vector3[4];
                    foreach (var item in dictionaryQuad)
                    {
                        selectedVertices[item.Key] = vertices[item.Value];
                    }
                    selectedTriangles = new int[] { 0, 1, 2, 2, 3, 0 };
                }
                else
                {
                    selectedVertices = new Vector3[] { p0, p1, p2 };
                    selectedTriangles = new int[] { 0, 1, 2 };
                }
                break;
            case 1:
                //Polygon Selection
                int closestCornerIndex = closestVeticeToCursor(hitTriangle, hit.point);
                int closestCornerToHit = triangles[indexTriangleMesh + closestCornerIndex];
                List<Vector3> listVerticesDisk = new List<Vector3>() { vertices[closestCornerToHit] };
                int numTriangle = 0;

                for (int index = 0; index < triangles.Length / 3; index++)
                {
                    int t0 = triangles[index * 3 + 0];
                    int t1 = triangles[index * 3 + 1];
                    int t2 = triangles[index * 3 + 2];

                    if (closestCornerToHit == t0)
                    {
                        listVerticesDisk.Add(vertices[t1]);
                        listVerticesDisk.Add(vertices[t2]);
                        numTriangle++;
                    }
                    if (closestCornerToHit == t1)
                    {
                        listVerticesDisk.Add(vertices[t2]);
                        listVerticesDisk.Add(vertices[t0]);
                        numTriangle++;
                    }
                    if (closestCornerToHit == t2)
                    {
                        listVerticesDisk.Add(vertices[t0]);
                        listVerticesDisk.Add(vertices[t1]);
                        numTriangle++;
                    }
                }

                List<int> listTrianglesDisk = new List<int>();
                for (int index = 1; index < numTriangle; index++)
                {
                    listTrianglesDisk.Add(0);
                    listTrianglesDisk.Add(index);
                    listTrianglesDisk.Add(index + 1);

                }

                //Close the disk
                listTrianglesDisk.Add(0);
                listTrianglesDisk.Add(numTriangle);
                listTrianglesDisk.Add(1);

                Debug.Log(numTriangle);
                selectedVertices = listVerticesDisk.ToArray();
                selectedTriangles = listTrianglesDisk.ToArray();
                break;
            default:
                //Triangle selection
                selectedVertices = new Vector3[] { p0, p1, p2 };
                selectedTriangles = new int[] { 0, 1, 2 };
                break;

        }

        selectedMesh.Clear();
        selectedMesh.vertices = selectedVertices;
        selectedMesh.triangles = selectedTriangles;
        selectedMesh.RecalculateNormals();
    }



    private int closestVeticeToCursor(Vector3[] vertices, Vector3 hitPosition)
    {
        float distance = (hitPosition - vertices[0]).magnitude;
        float minDistance = distance;
        int closestVertice = 0;


        for (int index = 1; index < vertices.Length; index++)
        {
            distance = (hitPosition - vertices[index]).magnitude;
            if (distance < minDistance)
            {
                closestVertice = index;
                minDistance = distance;
            }
        }
        return closestVertice;
    }

    private List<int> closestEdgeToCursor(Vector3[] vertices, Vector3 hitPosition)
    {
        float distance = Vector3.Cross(vertices[0] - vertices[2], hitPosition - vertices[2]).magnitude;
        float minDistance = distance;
        int[] closestEdgeTriangle = new int[] { 2, 0 };


        for (int index = 0; index < vertices.Length - 1; index++)
        {
            distance = Vector3.Cross(vertices[index + 1] - vertices[index], hitPosition - vertices[index]).magnitude;
            if (distance < minDistance)
            {
                closestEdgeTriangle[0] = index;
                closestEdgeTriangle[1] = index + 1;
                minDistance = distance;
            }
        }
        return closestEdgeTriangle.ToList<int>();
    }

    private void AddLastVerticeQuad(ref Dictionary<int, int> quadTmp, int vertexIndex)
    {

        for (int index = 1; index <= quadTmp.Count; index++)
        {
            if (!quadTmp.ContainsKey(index))
            {
                quadTmp.Add(index, vertexIndex);
                break;
            }
        }
    }

}
