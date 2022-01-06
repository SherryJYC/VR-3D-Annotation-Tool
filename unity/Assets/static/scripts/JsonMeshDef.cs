[System.Serializable]
public class MeshData
{
    public JsonData[] datalist;
}

[System.Serializable]
public class JsonMesh
{
    public int seq;
    public string name;

    public string RGBmesh;
    public string EmptyMesh;
}

[System.Serializable]
public class JsonData
{
    public string name;
    public string Description;
    public string FolderDirectory;
    public string RGBDirectory;
    public string EmptyDirectory;
    public float x_size;
    public float z_size;
    public JsonMesh[] submeshes;
}

