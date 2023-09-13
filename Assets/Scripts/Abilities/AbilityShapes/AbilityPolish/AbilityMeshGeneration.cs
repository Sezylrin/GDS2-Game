using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMeshGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    private Mesh dynamicMesh;
    public MeshRenderer rend;
    public MeshFilter meshFilter;

    private Vector3[] vertices;
    private Vector2[] uv;
    void Start()
    {
        dynamicMesh = new Mesh();

        uv = new Vector2[4];
        vertices = new Vector3[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

        int[] triangles = { 0, 2, 1, 0, 3, 2 };

        dynamicMesh.vertices = vertices;
        dynamicMesh.uv = uv;
        dynamicMesh.triangles = triangles;
        meshFilter.mesh = dynamicMesh;

    }
    public void SetMaterial(Material colour)
    {
        rend.material = colour;
    }
    // Update is called once per frame
    public void SetVertex(Vector2[] vertices)
    {
        if (!dynamicMesh)
            return;
        Vector3[] vertice = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            vertice[i] = vertices[i];
        }
        dynamicMesh.vertices = vertice;
        meshFilter.mesh = dynamicMesh;
    }
}
