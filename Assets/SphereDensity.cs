using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDensity : MonoBehaviour
{
    [SerializeField]
    ComputeShader densityShader;

    [SerializeField]
    float radius = 1;

    public ComputeBuffer Generate(ComputeBuffer pointsBuffer, int numPointsPerAxis)
    {
        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        int numThreadsPerAxis = Mathf.CeilToInt(numPointsPerAxis / 8f);
        // Points buffer is populated inside shader with pos (xyz) + density (w).
        // Set paramaters
        densityShader.SetBuffer(0, "points", pointsBuffer);
        densityShader.SetInt("numPointsPerAxis", numPointsPerAxis);
        densityShader.SetFloat("radius", radius);
        // densityShader.SetFloat("boundsSize", boundsSize);
        // densityShader.SetVector("centre", new Vector4(centre.x, centre.y, centre.z));
        // densityShader.SetVector("offset", new Vector4(offset.x, offset.y, offset.z));
        // densityShader.SetFloat("spacing", spacing);
        // densityShader.SetVector("worldSize", worldBounds);

        // Dispatch shader
        densityShader.Dispatch(0, numThreadsPerAxis, numThreadsPerAxis, numThreadsPerAxis);

        // Return voxel data buffer so it can be used to generate mesh
        return pointsBuffer;
    }
}
