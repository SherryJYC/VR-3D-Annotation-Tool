using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderBoundingBox : MonoBehaviour
{
    [SerializeField]
    private List<Vector3> boundingBoxPoints;

    [SerializeField]
    private Mesh boundsMesh;

    [SerializeField]
    private Bounds bounds;

    public Material lineMaterial;
    public Color linecolor;
    public Color HighLightcolor =Color.blue;
    public Color Selectedcolor = Color.white;
    private Color DrawColor;
    /// <summary>
    /// Ordered points of the bounding box
    /// </summary>
    public List<Vector3> BoundingBoxPoints
    {
        get
        {
            return boundingBoxPoints;
        }

        set
        {
            boundingBoxPoints = value;
        }
    }

    /// <summary>
    ///  <paramref name="mode"/> allows "select" "highlight" "normal"
    /// </summary>
    public void SetMode( string mode) 
    {  if (mode =="select")
            DrawColor = Selectedcolor;
        if (mode == "highlight")
            DrawColor = HighLightcolor;
        if (mode == "normal")
            DrawColor = linecolor;
    }

    private void Start()
    {
         DrawColor = linecolor ;
    }
    private void Awake()
    {
        // Get the mesh and the mesh's bounds object
        //boundsMesh = GetComponent<MeshFilter>().mesh; 
       
        boundsMesh = GetComponent<MeshCollider>().sharedMesh;
        bounds = boundsMesh.bounds;
        BoundingBoxPoints = CalculateBoundingBoxCorners(bounds);

        // Transform all the points into world space
        for (int i = 0; i < BoundingBoxPoints.Count; i++)
        {
            BoundingBoxPoints[i] = transform.TransformPoint(BoundingBoxPoints[i]);
        }
    }

    private void Update()
    {
       
        // Get the bounding box points
        BoundingBoxPoints = CalculateBoundingBoxCorners(bounds);

        // Transform all the points into world space
        for (int i = 0; i < BoundingBoxPoints.Count; i++)
        {
            BoundingBoxPoints[i] = transform.TransformPoint(BoundingBoxPoints[i]);
        }
      
    }

    void OnRenderObject()
    {   if (Camera.current.gameObject.layer == LayerMask.NameToLayer("Dynamic_Cam"))return;
        if (!lineMaterial)
        {
            Debug.Log("Unable to render lines");
            return;
        }
        
        lineMaterial.SetPass(0);
       
        GL.Begin(GL.LINES);
        GL.Color(DrawColor);

        // Points from the first point
        GL.Vertex3(BoundingBoxPoints[0].x, BoundingBoxPoints[0].y, BoundingBoxPoints[0].z);
        GL.Vertex3(BoundingBoxPoints[1].x, BoundingBoxPoints[1].y, BoundingBoxPoints[1].z);

        GL.Vertex3(BoundingBoxPoints[0].x, BoundingBoxPoints[0].y, BoundingBoxPoints[0].z);
        GL.Vertex3(BoundingBoxPoints[3].x, BoundingBoxPoints[3].y, BoundingBoxPoints[3].z);

        GL.Vertex3(BoundingBoxPoints[0].x, BoundingBoxPoints[0].y, BoundingBoxPoints[0].z);
        GL.Vertex3(BoundingBoxPoints[4].x, BoundingBoxPoints[4].y, BoundingBoxPoints[4].z);

        // Points from the second point
        GL.Vertex3(BoundingBoxPoints[1].x, BoundingBoxPoints[1].y, BoundingBoxPoints[1].z);
        GL.Vertex3(BoundingBoxPoints[5].x, BoundingBoxPoints[5].y, BoundingBoxPoints[5].z);

        GL.Vertex3(BoundingBoxPoints[1].x, BoundingBoxPoints[1].y, BoundingBoxPoints[1].z);
        GL.Vertex3(BoundingBoxPoints[2].x, BoundingBoxPoints[2].y, BoundingBoxPoints[2].z);

        // Points from the third point
        GL.Vertex3(BoundingBoxPoints[3].x, BoundingBoxPoints[3].y, BoundingBoxPoints[3].z);
        GL.Vertex3(BoundingBoxPoints[7].x, BoundingBoxPoints[7].y, BoundingBoxPoints[7].z);

        GL.Vertex3(BoundingBoxPoints[3].x, BoundingBoxPoints[3].y, BoundingBoxPoints[3].z);
        GL.Vertex3(BoundingBoxPoints[2].x, BoundingBoxPoints[2].y, BoundingBoxPoints[2].z);

        // Points from the fourth point
        GL.Vertex3(BoundingBoxPoints[4].x, BoundingBoxPoints[4].y, BoundingBoxPoints[4].z);
        GL.Vertex3(BoundingBoxPoints[7].x, BoundingBoxPoints[7].y, BoundingBoxPoints[7].z);

        GL.Vertex3(BoundingBoxPoints[4].x, BoundingBoxPoints[4].y, BoundingBoxPoints[4].z);
        GL.Vertex3(BoundingBoxPoints[5].x, BoundingBoxPoints[5].y, BoundingBoxPoints[5].z);

        // Points from the fifth point
        GL.Vertex3(BoundingBoxPoints[5].x, BoundingBoxPoints[5].y, BoundingBoxPoints[5].z);
        GL.Vertex3(BoundingBoxPoints[6].x, BoundingBoxPoints[6].y, BoundingBoxPoints[6].z);

        // Points from the sixth point
        GL.Vertex3(BoundingBoxPoints[6].x, BoundingBoxPoints[6].y, BoundingBoxPoints[6].z);
        GL.Vertex3(BoundingBoxPoints[2].x, BoundingBoxPoints[2].y, BoundingBoxPoints[2].z);

        GL.Vertex3(BoundingBoxPoints[6].x, BoundingBoxPoints[6].y, BoundingBoxPoints[6].z);
        GL.Vertex3(BoundingBoxPoints[7].x, BoundingBoxPoints[7].y, BoundingBoxPoints[7].z);
        
        GL.End(); 
        


    }

    /// <summary>
    /// Calculates the corners of the bounding box given by the <paramref name="bounds"/>
    /// </summary>
    /// <param name="bounds">This is the bounds to use for the calculation</param>
    /// <returns>Returns a list of points that represent the corners.</returns>
    public static List<Vector3> CalculateBoundingBoxCorners(Bounds bounds)
    {
        //POSITIVE Z

        //1,1,1
        Vector3 v1 = new Vector3(bounds.center.x + bounds.extents.x,
                                bounds.center.y + bounds.extents.y,
                                bounds.center.z + bounds.extents.z);

        //-1,1,1
        Vector3 v2 = new Vector3(bounds.center.x - bounds.extents.x,
                                bounds.center.y + bounds.extents.y,
                                bounds.center.z + bounds.extents.z);

        //-1,1,1
        Vector3 v3 = new Vector3(bounds.center.x - bounds.extents.x,
                                bounds.center.y - bounds.extents.y,
                                bounds.center.z + bounds.extents.z);

        //1,-1,1
        Vector3 v4 = new Vector3(bounds.center.x + bounds.extents.x,
                                bounds.center.y - bounds.extents.y,
                                bounds.center.z + bounds.extents.z);

        //NEGATIVE Z

        //1,1,-1
        Vector3 v5 = new Vector3(bounds.center.x + bounds.extents.x,
                                bounds.center.y + bounds.extents.y,
                                bounds.center.z - bounds.extents.z);

        //-1,1,-1
        Vector3 v6 = new Vector3(bounds.center.x - bounds.extents.x,
                                bounds.center.y + bounds.extents.y,
                                bounds.center.z - bounds.extents.z);

        //-1,-1,-1
        Vector3 v7 = new Vector3(bounds.center.x - bounds.extents.x,
                                bounds.center.y - bounds.extents.y,
                                bounds.center.z - bounds.extents.z);

        //1,-1,-1
        Vector3 v8 = new Vector3(bounds.center.x + bounds.extents.x,
                                bounds.center.y - bounds.extents.y,
                                bounds.center.z - bounds.extents.z);

        List<Vector3> points = new List<Vector3>();

        points.Add(v1);
        points.Add(v2);
        points.Add(v3);
        points.Add(v4);
        points.Add(v5);
        points.Add(v6);
        points.Add(v7);
        points.Add(v8);

        return points;
    }
}