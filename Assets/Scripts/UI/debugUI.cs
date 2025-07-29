using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class debugUI : MonoBehaviour
{
    [HideInInspector] public static debugUI instance;

    [SerializeField] private GameObject player;
    [HideInInspector] private GameObject playerRef;

    [SerializeField] private GameObject TerrainMenu;
    [SerializeField] private GameObject changeTerrainMenu;
    [SerializeField] private GameObject loadingScreen;

    [SerializeField] private RawImage noiseMapDisplay;

    #region terrain-details
    [Header("Terrain Details")]
    [SerializeField] private int seed;
    [SerializeField] private TextMeshProUGUI seedVal;

    [SerializeField] private int seedOffset;
    [SerializeField] private TextMeshProUGUI seedOffsetVal;

    [SerializeField] private int octaves;
    [SerializeField] private TextMeshProUGUI octaveVal;

    [SerializeField] private float scale;
    [SerializeField] private TextMeshProUGUI scaleVal;

    [SerializeField] private float lacuncarity;
    [SerializeField] private TextMeshProUGUI lacuncarityVal;

    [SerializeField] private float persistance;
    [SerializeField] private TextMeshProUGUI persistanceVal;

    [SerializeField] private float depthScale;
    [SerializeField] private TextMeshProUGUI depthScaleVal;
    #endregion

    #region input-fields

    [SerializeField] private TMP_InputField iFSeed;
    [SerializeField] private TMP_InputField iFSeedOffset;
    [SerializeField] private TMP_InputField iFOctave;
    [SerializeField] private TMP_InputField iFScale;
    [SerializeField] private TMP_InputField iFlacunarity;
    [SerializeField] private TMP_InputField iFPersistance;
    [SerializeField] private TMP_InputField iFDepthScale;

    #endregion

    void Start()
    {
        instance = this;
        changeTerrainMenu.SetActive(false);
        loadingScreen.SetActive(false);
    }

    private void FixedUpdate()
    {
        //updateNoiseMapOnUI();
    }

    public void updateNoiseMapOnUI(float[,] noiseMap)
    {

        int mapWidth = noiseMap.GetLength(0);
        int mapHeight = noiseMap.GetLength(1);

        Texture2D tex = new Texture2D(mapWidth, mapHeight);

        Color[] colourMap = new Color[mapWidth * mapHeight];

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                colourMap[y * mapWidth + x] = Color.Lerp(Color.white, Color.black, noiseMap[x, y]); // greyscale
            }
        }


        //Additonal texture settings
        #region texture-filters
        tex.SetPixels(colourMap);
        tex.Apply();
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;

        noiseMapDisplay.GetComponent<RawImage>().texture = tex;
        #endregion
    }

    #region set-terrain-details
    public void setSeed(int s)
    {
        seed = s;
        seedVal.text = seed.ToString();
    }

    public void setOctaves(int o)
    {
        octaves = o;
        octaveVal.text = octaves.ToString();
    }

    public void setScale(float s)
    {
        scale = s;
        scaleVal.text = scale.ToString();
    }

    public void setLacunarity(float l)
    {
        lacuncarity = l;
        lacuncarityVal.text = lacuncarity.ToString();
    }

    public void setPersistance(float p)
    {
        persistance = p;
        persistanceVal.text = persistance.ToString();
    }

    public void setDepthScale(float d)
    {
        depthScale = d;
        depthScaleVal.text = depthScale.ToString();
    }

    public void updateUI(mapData md)
    {
        setSeed(md.seed);
        setOctaves(md.octaves);
        setScale(md.scale);
        setLacunarity(md.lacunarity);
        setPersistance(md.persistance);
        setDepthScale(md.meshDepth);
    }

    #region menu-controller-funcs
    public void toggleLoadScreen(int t)
    {
        //add fade over time
        Debug.Log("load");
        loadingScreen.SetActive(!loadingScreen.activeInHierarchy);
    }

    public void toggleTerrainMenu()
    {
        TerrainMenu.SetActive(!TerrainMenu.activeInHierarchy);
    }

    public void toggleChangeTerrainMenu()
    {
        changeTerrainMenu.SetActive(!changeTerrainMenu.activeInHierarchy);
    }
    #endregion

    #endregion

    #region read-UI-inputfields
    public void readIFSeed() 
    {
        string s = iFSeed.text.ToString();

        if(int.TryParse(s, out int newSeed))
        {
            seed = newSeed;
        }
        else 
        {
            Debug.Log("[WARNING] "+s+"is not a valid seed");
        }
    }

    public void readIFSeedOffset()
    {
        string s = iFSeedOffset.text.ToString();

        if (int.TryParse(s, out int newOffset))
        {
            seedOffset = newOffset;
        }
        else
        {
            Debug.Log("[WARNING] " + s + "is not a valid seed");
        }
    }

    public void readIFOctaves()
    {
        string o = iFOctave.text.ToString();

        if (int.TryParse(o, out int newOctaves))
        {
            octaves = newOctaves;
        }
        else
        {
           // Debug.Log("[WARNING] " + o + "is not a valid for octaves");
        }
    }

    public void readIFScale()
    {
        string sc = iFScale.text.ToString();

        if (float.TryParse(sc, out float newScale))
        {
            scale = newScale;
        }
        else
        {
            Debug.Log("[WARNING] " + sc + "is not a valid scale");
        }
    }

    public void readIFPersistance()
    {
        string p = iFPersistance.text.ToString();

        if (float.TryParse(p, out float newPersistance))
        {
            persistance = newPersistance;
        }
        else
        {
            Debug.Log("[WARNING] " + p + "is not a valid for persistance");
        }
    }

    public void readIFLacunarity()
    {
        string l = iFlacunarity.text.ToString();

        if (float.TryParse(l, out float newLacunrity))
        {
            lacuncarity = newLacunrity;
        }
        else
        {
            Debug.Log("[WARNING] " + l + "is not a valid for lacunarity");
        }
    }

    public void readIFDepthScale()
    {
        string dS = iFDepthScale.text.ToString();

        if (float.TryParse(dS, out float newDepth))
        {
            depthScale = newDepth;
        }
        else
        {
            Debug.Log("[WARNING] " + dS + "is not a valid for lacunarity");
        }
    }

    public mapData ReadMapData() 
    {
        readIFSeed();
        readIFSeedOffset();
        readIFOctaves();
        readIFScale();
        readIFPersistance();
        readIFLacunarity();
        readIFDepthScale();

        mapData mD = new mapData();
        mD.seed = seed;
        mD.seedOffset = seedOffset;
        mD.octaves = octaves; 
        mD.scale = scale;
        mD.lacunarity = lacuncarity;
        mD.persistance = persistance;
        mD.meshDepth = depthScale;

        return mD;
    }

    public void randomize()
    {
        terrainGenerator.Instance.randomTerrain();
    }
    #endregion

    #region debug-funcs
    public void spawnPlayer()
    {
        playerRef = Instantiate(player, new Vector3(terrainGenerator.Instance.mData.chunkSize / 2, 25, terrainGenerator.Instance.mData.chunkSize / 2), Quaternion.identity);

        toggleChangeTerrainMenu();
        toggleTerrainMenu();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        //will be deleted
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Destroy(playerRef);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            toggleTerrainMenu();
        }
    }
    #endregion
}
