using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum meshQuality 
{
    Low,
    Medium,
    High,
    Ultra,
}


public class simplfyMesh : MonoBehaviour
{
    [HideInInspector] private int chunkSize;
    [HideInInspector] private int lod;
    [HideInInspector] private meshQuality quality;
    [HideInInspector] private float terrainDepth;
    [HideInInspector] private AnimationCurve terraoinCurve;
    [HideInInspector] float[,] noiseMap;
    [HideInInspector] int[] newTriangle;
    [HideInInspector] mapData md;

    [SerializeField] private Vector3[] newVerts;


    public void initSimpleMesh(ref mapData mD)
    { 
        md = mD;
        chunkSize = mD.chunkSize;
        quality = mD.LOD;
        noiseMap = mD.mapNoiseMap;
        terrainDepth = mD.meshDepth;
        terraoinCurve = mD.animCurve;


    }

    public MeshFilter simplifyMesh(MeshFilter mf, ref mapData mD) 
    {
        int iter = 0;
        Debug.Log("lod->" + quality);
        switch (quality)
        {
            case meshQuality.Low:
                iter = 10;
                break;
            case meshQuality.Medium:
                iter = 6;
                break;
            case meshQuality.High:
                iter = 3;
                break;
            case meshQuality.Ultra:
                iter = 1;
                break;
            default:
                Debug.Log("LOD Error");
                break;
           
        }

        //Verts = new Vector3[(chunkSize - 1) / iter * (chunkSize - 1) / iter];

        mD.terrainVerts = new Vector3[(chunkSize - 1) / iter * (chunkSize - 1) / iter];
        mD.terrainUVs = new Vector2[(chunkSize - 1) / iter * (chunkSize - 1) / iter];
        mD.terrainTris = new int[(chunkSize - 1) / iter * (chunkSize - 1) / iter * 6];

        Debug.Log("tris:" + mD.terrainTris.Length);

        int vertexIndex = 0;
        int trisIndex = 0;

        int meshSimplifactionFactor = iter*2;
        int vertsPerLine = ((chunkSize - 1) / meshSimplifactionFactor) + 1;

        newVerts = new Vector3[vertsPerLine * vertsPerLine];
        Vector2[] newUV = new Vector2[vertsPerLine * vertsPerLine];

        #region create-vertices
        for (int w = 0; w < chunkSize; w+=meshSimplifactionFactor)
        {
            for (int h = 0; h < chunkSize; h+=meshSimplifactionFactor)
            {

                newVerts[vertexIndex] = new Vector3(w, terrainDepth * terraoinCurve.Evaluate(noiseMap[w, h]) * md.meshDepth, h);

                if(newVerts[vertexIndex] == Vector3.zero)
                    Debug.Log("Vert index: " + vertexIndex);

                newUV[vertexIndex] = new Vector2((float)w / chunkSize, (float)h / chunkSize);
/*                if (newVerts[vertexIndex] == new Vector3(0, 0, 0))
                {
                    break;
                }*/
                vertexIndex++;
            }
        }
        Debug.Log("New verts added");
        #endregion

        #region draw-triangles
        vertexIndex = 0;
        trisIndex = 0;

        newTriangle = new int[vertsPerLine * vertsPerLine * 6];

        for (int x = 0; x < chunkSize; x+=meshSimplifactionFactor)
        {
            for (int y = 0; y < chunkSize; y+=meshSimplifactionFactor)
            {
                if (x < chunkSize - 1)
                {
                    if (y < chunkSize - 1)
                    {
                        newTriangle[trisIndex + 0] = vertexIndex;
                        newTriangle[trisIndex + 1] = vertexIndex + 1;
                        newTriangle[trisIndex + 2] = vertexIndex + vertsPerLine;
                        newTriangle[trisIndex + 3] = vertexIndex + 1;
                        newTriangle[trisIndex + 4] = vertexIndex + vertsPerLine + 1;
                        newTriangle[trisIndex + 5] = vertexIndex + vertsPerLine;
                        trisIndex += 6;
                    }
                }

                vertexIndex++;
            }
        }

        Debug.Log("New Tris added");
        #endregion
        mD.terrainVerts = newVerts;
        mD.terrainUVs = newUV;
        mD.terrainTris = newTriangle;

        return mf; 
    }

    public Vector3[] returnNewVerticesArray()
    {
        Vector3[] vert;
        vert = new Vector3[(chunkSize - 1) * (chunkSize - 1)];

        for (int i = 0; i < md.terrainVerts.Length; i++)
        {
            vert[i] = md.terrainVerts[i];
        }

        return vert;
    }
}
