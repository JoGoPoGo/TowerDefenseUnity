using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelDictionary : MonoBehaviour
{
    // Key = Gitterposition, Value = 0 (frei) oder 1 (besetzt)
    private Dictionary<Vector2Int, int> spawnGrid = new Dictionary<Vector2Int, int>();

    // Prüfen, ob an einer Position gespawnt werden darf
    public bool CanSpawnAt(Vector2Int pos)
    {
        return !spawnGrid.ContainsKey(pos) || spawnGrid[pos] == 0;
    }

    // Platz als besetzt markieren
    public void OccupyPosition(Vector2Int pos)
    {
        spawnGrid[pos] = 1;
    }

    // Platz wieder freigeben
    public void FreePosition(Vector2Int pos)
    {
        spawnGrid[pos] = 0;
    }
}
