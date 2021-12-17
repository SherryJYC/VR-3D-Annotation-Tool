using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZEffects;

public class PaintballController : Weapons
{
    public GameObject LabelText;
    public static TextMesh labelText;
    public EffectTracer TracerEffect;
    public GameObject laserPrefab; // The laser prefab
    public GameObject targetPrefab;
    public GameObject MeshCursor;

    private GameObject rayOfFire; // A reference to the spawned laser
    private GameObject targetOfFire;
    private Mesh selectedMesh;
    private Transform fireTransform; // The transform component of the laser for ease of use

    public LayerMask MiniMapMask;
    public LayerMask MenuMask;
    // Use this for initialization
    void Start()
    {
        Initialize();
        targetOfFire = Instantiate(targetPrefab);
        targetOfFire.transform.parent = transform;
        rayOfFire = Instantiate(laserPrefab);
        rayOfFire.transform.parent = transform;
        fireTransform = rayOfFire.transform;

        //TODO: Tune these parameters later on
        radiusOfFire = 1f;
        factorOfScale = 0.001f;
        labelText = LabelText.GetComponent<TextMesh>();
        selectedMesh = MeshCursor.GetComponent<MeshFilter>().mesh;
        GetComponentInParent<SteamVR_TrackedController>().MenuButtonClicked += PanelController.MenuButton;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit constantRay;
        if (PanelController.menuActive)
        {
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, MenuMask))
            {
                rayOfFire.SetActive(false);
                targetOfFire.SetActive(false);
                MeshCursor.SetActive(false);
                ShowLaser(constantRay);
                if (TriggerIsPressed())
                {
                    hitPoint = constantRay.point;
                    GameObject buttonMenu = GameObject.Find(constantRay.transform.name);
                    PanelController.menuActive = false;
                    PanelController.menu.SetActive(false);
                    MenuController.OpenMenu(buttonMenu.name, constantRay, false);
                }

            }
        }
        else if (ShowMinimap.isMiniMapActive)
        {
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, MiniMapMask))
            {
                targetOfFire.SetActive(false);
                MeshCursor.SetActive(false);
                ShowLaser(constantRay);
                var collider = constantRay.collider.gameObject;
                if (collider.GetComponent<Button>() != null)
                {
                    if (TriggerIsPressed())
                    {
                        collider.GetComponent<Button>().onClick.Invoke();
                    }
                    else
                    {
                        collider.GetComponent<Button>().Select();
                    }
                }
            }
        }
        else
        {
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out constantRay, distanceOfShoot, shootableMask))
            {
               
                ShowLaserTarget(constantRay);
                HighlightMeshTarget(constantRay, Color.cyan, 0);
                if (TriggerIsPressed())
                {
                    Animation();
                    Fire();
                }
            }
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
                ChangeFactorOfScale(highPoly);
                SplitMesh(highPoly);
                ColorMesh(highPoly, hit, current_color, meshHits, radiusOfFire > 1);
                SaveColorTemporary(meshHits);
            }

        }
    }

    void SplitMesh(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Vector3[] verts = mesh.vertices;
        Vector3[] normals = mesh.normals;
        Vector2[] uvs = mesh.uv;

        Vector3[] newVerts;
        Vector3[] newNormals;
        Vector2[] newUvs;

        int n = triangles.Length;
        newVerts = new Vector3[n];
        newNormals = new Vector3[n];
        newUvs = new Vector2[n];

        for (int i = 0; i < n; i++)
        {
            newVerts[i] = verts[triangles[i]];
            newNormals[i] = normals[triangles[i]];
            if (uvs.Length > 0)
            {
                newUvs[i] = uvs[triangles[i]];
            }
            triangles[i] = i;
        }
        mesh.vertices = newVerts;
        mesh.normals = newNormals;
        mesh.uv = newUvs;
        mesh.triangles = triangles;
    }

    private void ShowLaser(RaycastHit target)
    {
        float brushSize = 1.0f;
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(muzzleTrasform.transform.position, target.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(target.point); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + brushSize * 0.001F, 0.0F + brushSize * 0.001F,
           target.distance);

        //Change color of the laser
        rayOfFire.GetComponent<Renderer>().material.SetColor("_Color", Color.red);

    }

    private void ShowLaserTarget(RaycastHit target)
    {
        int brushSize = 1;
        if (radiusOfFire >= 5)
        {
           brushSize += (int)radiusOfFire / 5;
        }
        rayOfFire.SetActive(true); //Show the laser
        fireTransform.position = Vector3.Lerp(muzzleTrasform.transform.position, target.point, .5f); // Move laser to the middle between the controller and the position the raycast hit
        fireTransform.LookAt(target.point); // Rotate laser facing the hit point
        fireTransform.localScale = new Vector3(0.0F + brushSize * 0.001F, 0.0F + brushSize * 0.001F,
            target.distance);

        MeshCursor.SetActive(true);
        targetOfFire.SetActive(true);
        targetOfFire.transform.position = target.point;
        targetOfFire.transform.rotation = Quaternion.FromToRotation(Vector3.forward, target.normal);
        targetOfFire.transform.localScale = new Vector3(0.01f * brushSize, 0.01f * brushSize, targetOfFire.transform.localScale.z);

        //Change color of the laser
        rayOfFire.GetComponent<Renderer>().material.SetColor("_Color", Weapons.current_color);
        targetOfFire.GetComponent<Renderer>().material.SetColor("_Color", Weapons.current_color);
        muzzleTrasform.gameObject.GetComponent<Renderer>().material.SetColor("_Color", Weapons.current_color);
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
            case 1:
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
                    selectedTriangles = new int[] { 0, 1, 2, 2, 3, 0};
                }
                else
                {
                    selectedVertices = new Vector3[] { p0, p1, p2 };
                    selectedTriangles = new int[] { 0, 1, 2 };
                }
                break;
            case 2:
                //Polygon Selection
                int closestCornerIndex = closestVeticeToCursor(hitTriangle, hit.point);
                int closestCornerToHit = triangles[indexTriangleMesh + closestCornerIndex];
                List<Vector3> listVerticesDisk = new List<Vector3>() { vertices[closestCornerToHit]};
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
                selectedVertices = new Vector3[] {p0, p1, p2 };
                selectedTriangles = new int[] { 0, 1, 2 };
                break;

        }
        
        selectedMesh.Clear();
        selectedMesh.vertices = selectedVertices;
        selectedMesh.triangles = selectedTriangles;
        selectedMesh.RecalculateNormals();
    }

    protected override bool TriggerIsPressed()
    {
        // Single shot weapon.
        return device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger);
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
        int[] closestEdgeTriangle = new int[] {2, 0};
        
        
        for (int index = 0; index < vertices.Length-1; index++)
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