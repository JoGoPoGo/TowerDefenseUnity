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

        // 1. Sicherheitscheck: Ist überhaupt ein Terrain in der Szene?
        if (terrain == null)
        {
            Debug.Log("FEHLER im Awake: Es gibt kein aktives Terrain in dieser Szene!");
            return; // Bricht die Methode hier sofort ab, um den Absturz zu verhindern
        }

        // 2. Sicherheitscheck: Hat das Terrain auch wirklich Terrain-Daten zugewiesen?
        if (terrain.terrainData == null)
        {
            Debug.Log("FEHLER im Awake: Das Terrain hat keine TerrainData!");
            return;
        }

        // Wenn beide Checks bestanden sind, machen wir sicher weiter:
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
