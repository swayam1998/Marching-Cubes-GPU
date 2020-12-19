using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField]
    ComputeShader computeShader = default;

    [SerializeField, Range(1,200)]
    int numPointsPerAxis = 20;
    
    ComputeBuffer positionsBuffer;
    ComputeBuffer meshTriangles;
    ComputeBuffer meshPositions;

    [SerializeField]
    Mesh Mesh = default;

    [SerializeField]
    Material Material = default;

    Bounds bounds;

    private int numPoints;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        numPointsPerAxisId = Shader.PropertyToID("_numPointsPerAxis");

    void OnEnable()
    {        
        numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        positionsBuffer = new ComputeBuffer(numPoints, 3 * 4);

        bounds = new Bounds(Vector3.zero, Vector3.one * 20);

        Vector3[] positions = Mesh.vertices;
        meshPositions = new ComputeBuffer(positions.Length, sizeof(float) * 3);
        meshPositions.SetData(positions);

        int[] triangles = Mesh.triangles;
        meshTriangles = new ComputeBuffer(triangles.Length, sizeof(int));
        meshTriangles.SetData(triangles);

        Material.SetBuffer("SphereLocations", positionsBuffer);
        Material.SetBuffer("Triangles", meshTriangles);
        Material.SetBuffer("Positions", meshPositions);
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;

        meshTriangles.Release();
        meshTriangles = null;

        meshPositions.Release();
        meshTriangles = null;
    }

    // Update is called once per frame
    void Update()
    {
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        computeShader.SetInt(numPointsPerAxisId, numPointsPerAxis);
        int groups = Mathf.CeilToInt(numPointsPerAxis / 8f);
        computeShader.Dispatch(0, groups, groups, groups);

        Graphics.DrawProcedural(Material, bounds, MeshTopology.Triangles, meshTriangles.count, numPoints);
    }
}
