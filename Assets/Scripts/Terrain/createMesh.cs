using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createMesh : MonoBehaviour
{
    [Header("Create Mesh Details")]
    #region create-mesh-references
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private int chunkSize;

    [HideInInspector] private bool drawMesh = true;

    //Stores the posiiton of the vertices
    [SerializeField] private Vector3[] vertices;
    [SerializeField] private Vector2[] uvs;
    [SerializeField] int[] triangles;

    [HideInInspector]private int[] tris;
    //[HideInInspector]private const int CHUNKSIZE = 241;
    #endregion

    [HideInInspector] private simplfyMesh simpleMesh;

    public void init(ref mapData mD)
    {
        chunkSize = mD.chunkSize;
        meshFilter = mD.meshFilter;

        simpleMesh = gameObject.AddComponent<simplfyMesh>();

        Debug.Log("mesh creator made");
    }


    public void createTerrain(ref mapData mD)
    {
        // initialse arrays
        vertices = new Vector3[(chunkSize - 1) * (chunkSize -1)];
        uvs = new Vector2[(chunkSize - 1) * (chunkSize - 1)];

        triangles = new int[(chunkSize - 1) * (chunkSize - 1) * 6];

        int vertexIndex = 0;
        int trisIndex = 0;


        #region create-vertices
        for (int w = 0; w < chunkSize-1; w++)
        {
            for (int h = 0; h < chunkSize-1; h++)
            {

                vertices[vertexIndex] = new Vector3(w, mD.meshDepth * mD.animCurve.Evaluate(mD.mapNoiseMap[w,h]), h);

                //normalise uv vector
                uvs[vertexIndex] = new Vector2((float)w / chunkSize, (float)h / chunkSize);

                vertexIndex++;
            }
        }

        #endregion

        #region draw-triangles
        vertexIndex = 0;
        trisIndex = 0;

        for (int x = 0; x < chunkSize-1; x++)
        {
            for (int y = 0; y < chunkSize-1; y++)
            {
                if (x < chunkSize - 1)
                {
                    if (y < chunkSize - 1)
                    {
                        triangles[trisIndex + 0] = vertexIndex;
                        triangles[trisIndex + 1] = vertexIndex + 1;
                        triangles[trisIndex + 2] = vertexIndex + chunkSize;
                        triangles[trisIndex + 3] = vertexIndex + 1;
                        triangles[trisIndex + 4] = vertexIndex + 1 + chunkSize;
                        triangles[trisIndex + 5] = vertexIndex + chunkSize;
                        trisIndex += 6;
                    }
                }
                vertexIndex++;
            }
        }
        #endregion

        #region apply-lod
        simpleMesh.initSimpleMesh(ref mD);
        meshFilter.sharedMesh.Clear();
        meshFilter = simpleMesh.simplifyMesh(meshFilter, ref mD);

        updateMesh(mD);
        #endregion

    }

    //Apply the new settings to the meshfilter
    public void updateMesh(mapData mD)
    {
        meshFilter = mD.meshFilter;
        meshFilter.gameObject.GetComponent<MeshRenderer>().enabled = true;

        vertices = mD.terrainVerts;
        meshFilter.sharedMesh.vertices = vertices;
        triangles = mD.terrainTris;
        meshFilter.sharedMesh.triangles = triangles; 
        uvs = mD.terrainUVs;
        meshFilter.sharedMesh.uv = uvs;
        meshFilter.sharedMesh.RecalculateNormals();

    }

    //used by terrainGenerator.cs to render the verts for debugging
    public Vector3[] returnVerticesArray() 
    {
        Vector3[] vert;
        vert = new Vector3[(chunkSize - 1) * (chunkSize - 1)];

        for(int i = 0; i < vertices.Length; i++)
        {
           vert[i] = vertices[i];
        }

        return vert;
    }


}
