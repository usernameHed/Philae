using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using hedCommon.extension.runtime;

/// <summary>
/// Generate Description
/// </summary>
public abstract class Generate : MonoBehaviour
{
    public enum TypeMesh
    {
        Plane, 
        Box,   
        Cone,  
        Tube,
        Torus,
        Sphere,
        IcoSphere,
    }

    [FoldoutGroup("GamePlay"), Tooltip("type"), SerializeField]
    protected TypeMesh typeMesh;

    public Material DefaultMaterial;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;

    protected Vector3[] verticesObject;           //verticle of object
    protected Vector3[] normalesObject;           //normals of all verticles
    protected Vector2[] uvsObject;                //uvs of points;
    protected int[] trianglesObject;              //then save triangle of objects

    protected Mesh meshObject;


    private void Start()
    {
        InitMesh();
    }

    /// <summary>
    /// génère le mesh
    /// </summary>
    [Button("GeneratePlease")]
    private void GeneratePlease()
    {
        meshFilter = gameObject.transform.GetOrAddComponent<MeshFilter>();
        meshRenderer = gameObject.transform.GetOrAddComponent<MeshRenderer>();

        if (meshFilter.sharedMesh == null)
        {
            meshFilter.sharedMesh = new Mesh();
            meshFilter.sharedMesh.name = "Procedural Mesh";
        }
        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = DefaultMaterial;
        }

        meshObject = meshFilter.sharedMesh;

        meshObject.Clear();

        GenerateMesh();

        meshObject.vertices = verticesObject;
        meshObject.normals = normalesObject;
        meshObject.uv = uvsObject;
        meshObject.triangles = trianglesObject;

        meshObject.RecalculateBounds();
        //mesh.Optimize();

        //MeshUtility.Optimize(mesh);
    }

    [Button("Save")]
    private void SaveMesh()
    {
        ExtMesh.SaveSelectedMeshObj(gameObject, true);
    }

    abstract protected void InitMesh(); //appelé à l'initialisation
    abstract protected void GenerateMesh(); //appelé à l'initialisation
}
