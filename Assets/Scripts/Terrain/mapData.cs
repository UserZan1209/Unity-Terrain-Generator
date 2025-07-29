using UnityEngine;
/*

   Contains all the data for the terrain data, to help reduce size of needed parameters in some functions  

 */
public class mapData 
{
    public int chunkSize = 241;
    public MeshFilter meshFilter;

    public meshQuality LOD;

    public int seed;
    public int seedOffset;

    public float scale;
    public float lacunarity;
    public float persistance;
    public int octaves;

    public float meshDepth;
    public AnimationCurve animCurve;

    public float[,] mapNoiseMap;

    public Vector3[] terrainVerts;
    public int[] terrainTris;
    public Vector2[] terrainUVs;

    public void randomize()
    {
        seed = Random.Range(-999999, 999999) + seedOffset;
        scale = Random.Range(-0.0999f, 0.0999f);
        octaves = Random.Range(2, 16);
        meshDepth = Random.Range(4.999f, 10.999f);
    }

    public void displayData()
    {
        Debug.Log("[Mapdata] Seed: " + seed);
        Debug.Log("[Mapdata] offset: " + seedOffset);
        Debug.Log("[Mapdata] scale: " + scale);
        Debug.Log("[Mapdata] lacunarity: " + lacunarity);
        Debug.Log("[Mapdata] persistance: " + persistance);
        Debug.Log("[Mapdata] mesh depth: " + meshDepth);
    }
}
