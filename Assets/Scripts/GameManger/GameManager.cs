using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int credits = 0;

    private Terrain terrain;
    private TerrainData terrainData;
    private float[,] originalHeights;

    void Awake()
    {
        terrain = Terrain.activeTerrain;

        terrain.terrainData = Instantiate(terrain.terrainData);

        terrainData = terrain.terrainData;

        originalHeights = terrainData.GetHeights(
            0, 0,
            terrainData.heightmapResolution,
            terrainData.heightmapResolution);
    }


    public bool SpendCredits(int amount, bool spend = true)
    {
        if(credits >= amount)
        {
            if (spend)
            {
                credits -= amount; //zieht amount von credits ab
            }
            return true;
        }
        return false;
    }

    public void AddCredits(int amount)
    {
        credits += amount;
    }

    public void RemoveCredits(int amount)
    {
        credits -= amount;
    }
}
