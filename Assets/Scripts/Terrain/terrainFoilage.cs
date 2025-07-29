using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class terrainFoilage : MonoBehaviour
{
    [SerializeField] private const int OBJECT_LIMIT = 3000;
    [SerializeField] private int currantObjects = 0;

    [SerializeField] private GameObject[] objectPool = new GameObject[OBJECT_LIMIT];

    public void terrainFoilageGenerator(ref mapData mD, foilageData fD)
    {
        #region refresh-object-pool
        //if there are active objects they are deleted before generating new foilage data
        if (currantObjects > 0)
        {
            for(int i = 0; i < currantObjects; i++)
            {
                Destroy(objectPool[i].gameObject);
            }
            currantObjects = 0;
        }
        #endregion

        objectPool = new GameObject[OBJECT_LIMIT];

        for (int i = 0; i < mD.terrainVerts.Length && currantObjects <= OBJECT_LIMIT; i++)
        {
            #region tree-placement   
            int r = Random.Range(0, fD.trees.Length-1);
            int chance = Random.Range(1,100);

            // and is above sea level
            if(chance <= 8 && currantObjects < OBJECT_LIMIT) 
            {
                Debug.Log("Attempt: " + (currantObjects+1).ToString());
              
                Vector3 spawnPos = new Vector3(mD.terrainVerts[i].x, mD.terrainVerts[i].y, mD.terrainVerts[i].z);
                //add random rotation
                GameObject t = Instantiate(fD.trees[r], spawnPos, Quaternion.identity);
                objectPool[currantObjects] = t;
                currantObjects++;

                float scale = Random.Range(1.2f, 2.5f);

                t.transform.localScale = t.transform.localScale * scale;

 

            }
            #endregion
        }
    }
}
