using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField]
    ComputeShader computeShader = default;

    [SerializeField, Range(1,200)]
    int numPointsPerAxis = 20;

    [SerializeField]
    float isoLevel = 0;

    [SerializeField]
    SphereDensity densityGenerator;    
    ComputeBuffer positionsBuffer;
    ComputeBuffer meshVertices;
    ComputeBuffer meshTriangles;

    [SerializeField]
    Material Material = default;

    Bounds bounds;

    private int numPoints;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        verticesId = Shader.PropertyToID("_Vertices"),
        trianglesId = Shader.PropertyToID("_Triangles"),
        isoLevelId = Shader.PropertyToID("isoLevel"),
        numPointsPerAxisId = Shader.PropertyToID("_numPointsPerAxis");

    void OnEnable()
    {        
        numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        positionsBuffer = new ComputeBuffer(numPoints, 3 * 4);

        bounds = new Bounds(Vector3.zero, Vector3.one * 20);

        densityGenerator.Generate(positionsBuffer, numPointsPerAxis);
        computeShader.SetBuffer(0, positionsId, positionsBuffer);
        computeShader.SetBuffer(0, verticesId, meshVertices);
        //computeShader.SetBuffer(0, trianglesId, meshTriangles);

        int[] tris = new int[meshVertices.count];
        for(int i=0;i<meshVertices.count;i++)
        {
            tris[i] = i;
        }
        meshTriangles.SetData(tris);

        computeShader.SetInt(numPointsPerAxisId, numPointsPerAxis);
        computeShader.SetFloat(isoLevelId, isoLevel);

        int groups = Mathf.CeilToInt(numPointsPerAxis / 8f);
        computeShader.Dispatch(0, groups, groups, groups);

        Material.SetBuffer("SphereLocations", positionsBuffer);
        Material.SetBuffer("Positions", meshVertices);
        Material.SetBuffer("Triangles", meshTriangles);
        
    }

    void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
        
        meshVertices.Release();
        meshTriangles = null;

        meshTriangles.Release();
        meshTriangles = null;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(meshTriangles.count);
        Graphics.DrawProcedural(Material, bounds, MeshTopology.Triangles, meshTriangles.count, meshVertices.count);
    }
}
