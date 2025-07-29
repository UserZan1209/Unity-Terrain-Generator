using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
    The main controller for the terrain generation feature, the script uses some addtional components to add additonal features

    TerrainGenerator->CreateMesh - This component takes the nosiemap and some extra infomation from the mapdata to make the first iteration of the terrain.
    CreateMesh->SimpleMesh - after generating the complete mesh, it's regenerated using LOD To reduce the vertices.
 
 */

[CreateAssetMenu()]
public class ColorSettings : ScriptableObject
{
    public Color terrainColor;
    public Material m_terrain;
}

public class terrainGenerator : MonoBehaviour
{
    public static terrainGenerator Instance;

    #region seed-options
    [Header("Seed Options")]
    [SerializeField] private int mapSeed;
    [SerializeField] private int seedOffset;
    #endregion

    #region terrain-modifyers
    [Header("Terrain Generator Properties")]
    [SerializeField] public mapData mData;

    [SerializeField] public foilageData mTerrainData;

    [SerializeField] private float scale;
    [SerializeField] private float lacunarity;
    [SerializeField] private float persistance;
    [SerializeField] private int octaves;
    #endregion

    #region mesh-settings
    [Header("Mesh Settings")]



    [SerializeField] private meshQuality meshQuality;
    [SerializeField] private float meshDepth;
    [HideInInspector] private Vector3[] verts;

    [SerializeField] private AnimationCurve terrainCurve;
    #endregion

    #region terrain colours
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Renderer textureRender;
    [SerializeField] private Color[] colours;
    [HideInInspector] private Color[] colourMap;

    [SerializeField] public ColorSettings colorSettings;
    #endregion

    

    [SerializeField] public GameObject[] treePrefabs;

    #region additions-to-conditions-and-functionality
    [Header("Additional Functionality")]
    [HideInInspector] private createMesh meshCreator;
    [HideInInspector] public terrainFoilage foliageCreator;

    [SerializeField] private bool renderVerts;
    [SerializeField] private bool terrainHasLimits;
    [SerializeField] private bool createTerrain;
    [SerializeField] private bool createFoilage;
    [SerializeField] private bool isInspector;
    #endregion

    private void Start()
    {
        init();

    }

    #region terrain-funcs
    private void init()
    {
        Instance = this;
        mData = new mapData();
        mData.meshFilter = meshFilter;
        mData.terrainVerts = new Vector3[(mData.chunkSize - 1) * (mData.chunkSize - 1)];
        mData.terrainUVs = new Vector2[(mData.chunkSize -1) * (mData.chunkSize - 1)];
        mData.terrainTris = new int[(mData.chunkSize - 1) * (mData.chunkSize - 1) * 6];

        meshFilter.sharedMesh.Clear();

        meshCreator = gameObject.AddComponent<createMesh>();
        meshCreator.init(ref mData);

        #region create-foilage
        foliageCreator = gameObject.AddComponent<terrainFoilage>();
        mTerrainData = new foilageData();
        mTerrainData.trees = new GameObject[treePrefabs.Length];

        for(int i = 0; i < treePrefabs.Length; i++) 
        { 
            mTerrainData.trees[i] = treePrefabs[i];
        }
        #endregion
        Debug.Log("Settings check completed");

    }

    #region map-data-functions
    private void setMapData() 
    {
        //set map data using the inspector values, this is called whent the mesh is generated
        mData.seed = mapSeed;
        mData.seedOffset = seedOffset;
        mData.scale = scale;
        mData.lacunarity = lacunarity;
        mData.persistance = persistance;
        mData.octaves = octaves;
        mData.meshDepth = meshDepth;
    }
    public void updateMapData(ref mapData md)
    {
        mapSeed = md.seed;
        seedOffset = md.seedOffset;
        scale = md.scale;
        lacunarity = md.lacunarity;
        persistance = md.persistance;
        octaves = md.octaves;
        meshDepth = md.meshDepth;

    }
    #endregion

    private void TerrainGenerator()
    {
        meshFilter.sharedMesh.Clear();
        
        //temp - used to test in runtime by just using inspector
        mData.LOD = meshQuality;
        mData.animCurve = terrainCurve;

        //create, store and draw the noise map
        mData.mapNoiseMap = noiseGeneration(mData);
        drawNoise(mData.mapNoiseMap);

        if (createTerrain) 
            meshCreator.createTerrain(ref mData);

        //[WARNING] - very expenisive causing slowdown
        if(renderVerts)
            verts = gameObject.GetComponent<simplfyMesh>().returnNewVerticesArray();

        if(createFoilage)
            foliageCreator.terrainFoilageGenerator(ref mData, mTerrainData);

        updateTerrainUI();

        UpdateShader();
    }

    #region noisemap-generator
    private float[,] noiseGeneration(mapData mD)
    {
        float[,] noiseMap = new float[mD.chunkSize, mD.chunkSize];
        
        float maxHeight = float.MinValue;
        float minHeight = float.MaxValue;

        //float seedVal = generateSeedVal(seed, seedOffset); // Removed so regenerating with the same seed makes the same outcome
        print(mD.seedOffset);
        for (int w = 0; w < mD.chunkSize; w++)
        {
            for (int h = 0; h < mD.chunkSize; h++)
            {
                //algorithm multiplyers
                float val = 0.0f;
                float frequency = 1.0f;
                float amplitude = 1.0f;

                // Using the number presented with octaves several noisemaps are created, overlayed and smoothed out to created terrain like enviroments
                #region fractal-noise-map
                for (int i = 0; i < octaves; i++)
                {
                    float wSample = (w + (mD.seed + mD.seedOffset)) * scale;
                    float hSample = (h + (mD.seed + mD.seedOffset)) * scale;


                    float perlinValue = Mathf.PerlinNoise(wSample, hSample) * 2 - 1;
                    val += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (val > maxHeight)
                {
                    maxHeight = val;
                }
                else if (val < minHeight)
                {
                    minHeight = val;
                }
                #endregion

                noiseMap[w, h] = val;
            }
        }

        //interpolate each noisemap create a fractal noisemap
        #region interpolate-noisemaps
        for (int h = 0; h < mD.chunkSize; h++)
        {
            for (int w = 0; w < mD.chunkSize; w++)
            {
                noiseMap[w, h] = Mathf.InverseLerp(minHeight, maxHeight, noiseMap[w, h]);
            }
        }
        #endregion

        return noiseMap;
    }

    private void drawNoise(float[,] noiseMap)
    {
        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D tex = new Texture2D(mapWidth, mapHeight);
         
        colourMap = new Color[mapWidth * mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                colourMap[y * mapWidth + x] = Color.Lerp(Color.white, Color.black, noiseMap[x, y]); // greyscale
                #region color texture
                /*
                                if (noiseMap[x, y] < 1f)
                                {
                                    colourMap[y * mapWidth + x] = colours[9];
                                }
                                if (noiseMap[x, y] < 0.95f)
                                {
                                    colourMap[y * mapWidth + x] = colours[8];
                                }
                                if (noiseMap[x, y] < 0.8f)
                                {
                                    colourMap[y * mapWidth + x] = colours[7];
                                }
                                if (noiseMap[x, y] < 0.7f)
                                {
                                    colourMap[y * mapWidth + x] = colours[6];
                                }
                                if (noiseMap[x, y] <= 0.6f)
                                {
                                    colourMap[y * mapWidth + x] = colours[5];
                                }
                                if (noiseMap[x, y] <= 0.5f)
                                {
                                    colourMap[y * mapWidth + x] = colours[4];
                                }
                                if (noiseMap[x, y] <= 0.4f)
                                {
                                    colourMap[y * mapWidth + x] = colours[3];
                                }
                                if (noiseMap[x, y] <= 0.3f)
                                {
                                    colourMap[y * mapWidth + x] = colours[2];
                                }
                                if (noiseMap[x, y] <= 0.15f)
                                {
                                    colourMap[y * mapWidth + x] = colours[1];
                                }
                                if (noiseMap[x, y] <= 0.1f)
                                {
                                    colourMap[y * mapWidth + x] = colours[0];
                                }
                */



                #endregion
            }
        }


        //Additonal texture settings
        #region texture-filters
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        tex.SetPixels(colourMap);
        tex.Apply();



        //colorSettings.m_terrain.SetTexture("tex", tex);

        textureRender.material.mainTexture = tex;

        debugUI.instance.updateNoiseMapOnUI(mData.mapNoiseMap);
        #endregion
    }
    #endregion

    public void InputGeneration()
    {
        mapData mD = debugUI.instance.ReadMapData();

        mData.seed = mD.seed;
        mData.seedOffset = mD.seedOffset;

        mData.octaves = mD.octaves;
        mData.scale = mD.scale;
        mData.lacunarity = mD.lacunarity;
        mData.persistance = mD.persistance;
        mData.meshDepth = mD.meshDepth;

        updateMapData(ref mData);
        updateTerrainUI();

        TerrainGenerator();

    }

    #region randomise-terrain
    public void randomTerrain()
    {
        mData.randomize();

        updateMapData(ref mData);
        updateTerrainUI();

        TerrainGenerator();
    }
    #endregion
    #endregion

    private void UpdateShader()
    {

    }

    #region UI-related-funcs

    public void setMapSeed(int s)
    {
        mapSeed = s;
    }
    public void setOctaves(int o) 
    {
        octaves = o;
    }
    public void setScale(float s)
    {
        float newScale = s;

        scale = newScale;
    }
    public void setPersistance(float p)
    {
        float newPersistance = p;

        persistance = newPersistance;
    }
    public void setLacunarity(float l)
    {
        float newLacunarity = l;

        lacunarity = newLacunarity;
    }
    public void setDepthScale(float dS)
    {
        float newDepth = dS;

        meshDepth = newDepth;
    }

    public void updateTerrainUI() 
    {
        debugUI.instance.setSeed(mData.seed);
        debugUI.instance.setOctaves(mData.octaves);
        debugUI.instance.setScale(mData.scale);
        debugUI.instance.setLacunarity(mData.lacunarity);
        debugUI.instance.setPersistance(mData.persistance);
        debugUI.instance.setDepthScale(mData.meshDepth);
    }


    #endregion

    #region debug-functions
    public void OnDrawGizmos()
    {
        if(verts != null)
        {
            for (int i = 0; i < verts.Length; i++)
            {
                Gizmos.DrawSphere(verts[i], .1f);
            }
        }
    }
    #endregion



}

