using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class Preview : MonoBehaviour
{
    public SpawnOnMouseClick spawnScript;
    private GameObject previewObject; // Das Objekt, das als Vorschau angezeigt wird
    private float hitPointx;
    private float hitPointz;
    private float hitPointy;
    public CancelDictionaryProtoType cancelScript;

    public Material normalMaterial;    // Material für gültige Position
    public Material sperrbereichMaterial; // Material für ungültige Position

    private bool terrainInScene;
    private Terrain terrainObject;

    private CancelDictionaryProtoType dictionary;

    private int tiling = 1;

    [Header("Audio")]
    public AudioClip placeSound;
    private AudioSource audioSource;

    [Header("Visuelle Effekte")]
    public GameObject placeParticlesPrefab; // Ziehe hier dein Partikel-Prefab rein

    void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        dictionary = gameManager.GetComponent<CancelDictionaryProtoType>();

        tiling = spawnScript.tiling;

        //previewObject = spawnScript.selectedPrefab;
        terrainObject = FindObjectOfType<Terrain>();
        terrainInScene = (terrainObject != null);          //prüft, ob ein Terrain in der Szene ist

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }
    // Update wird einmal pro Frame aufgerufen
    void Update()
    {
        previewObject = GameObject.FindWithTag("lastSpawned");

        // Überprüfe, ob ein Objekt gespawnt werden soll
        if (spawnScript.spawned && previewObject != null && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (cancelScript != null)
            {
                cancelScript.ShowCancelArea();
            }
            // Wenn der Raycast etwas trifft
            if (Physics.Raycast(ray, out hit))
            {
                hitPointx = (tiling * Mathf.Round(hit.point.x / tiling));    
                hitPointz = (tiling * Mathf.Round(hit.point.z / tiling));

                Vector3 placePosition = new Vector3(hitPointx, 0, hitPointz); //erstellt die Position aus den ganzen Werten der HitPoints

                if (terrainInScene)                //wenn ein Terrain in der Szene ist, wird die Höhe an dem Punkt des Terrain der placePositon hinzugefügt
                {
                    float terrainHeight = Terrain.activeTerrain.SampleHeight(new Vector3(hitPointx, 0, hitPointz));
                    placePosition += new Vector3(0, terrainHeight, 0);
                }

                if (IsPositionValidAt(new Vector3(hitPointx, 0, hitPointz)))
                {
                    //SetVisibility(previewObject, true);
                    previewObject.transform.position = placePosition;
                    ChangeRangeMaterial(normalMaterial);
                }

                // Setze die Position des Vorschau-Objekts auf den Trefferpunkt
                else
                {
                    Debug.Log("Sperrbereich");
                    //SetVisibility(previewObject, false);
                    ChangeRangeMaterial(sperrbereichMaterial);
                }

                    
            }
        }
        if (Input.GetMouseButtonUp(0) && spawnScript.spawned)
        {
            SetVisibility(previewObject, true);
            if (terrainInScene)
            {
                SetTerrainCircleHeight(previewObject.transform.position, 3f, previewObject.transform.position.y);
            }
            spawnScript.spawned = false; // Setze spawned auf false, wenn die linke Maustaste losgelassen wird

            if (placeSound != null)
            {
                audioSource.PlayOneShot(placeSound);
            }

            if (placeParticlesPrefab != null && previewObject != null)
            {
                // Erstellt das Partikelsystem genau an der Position des Turms
                GameObject particles = Instantiate(placeParticlesPrefab, previewObject.transform.position, Quaternion.identity);

                // Zerstört das Partikel-Objekt nach 2 Sekunden, um Speicherplatz zu sparen
                Destroy(particles, 2f);
            }

            if (previewObject != null)
            {
                previewObject.layer = LayerMask.NameToLayer("Default");
                previewObject.tag = "Tower";
            }
            if (cancelScript != null)
            {
                cancelScript.HideCancelArea();
            }

        }
        
    }
    public bool IsPositionValidAt(Vector3 position)
    {
        int x = (int)Mathf.Round(position.x);
        int y = (int)Mathf.Round(position.z);

        Vector2Int pos = new Vector2Int(x, y);

        return dictionary.CanSpawnAt(pos);

        /*Tower towerPrefab = spawnScript.prefabsToSpawn[spawnScript.selectedPrefabIndex].GetComponent<Tower>();
        float ownCancelRadius = towerPrefab.spawnCancelRadius;
        // Suche nach Türmen in der Szene
        Tower[] towers = FindObjectsOfType<Tower>();
        foreach (Tower tower in towers)
        {
            if (tower.CompareTag("lastSpawned"))
            {
                continue; // Gehe zum nächsten Turm
            }
            float distance = Vector3.Distance(new Vector3(position.x, 0, position.z), tower.transform.position);
            if (distance < tower.spawnCancelRadius || distance < ownCancelRadius)
            {
                return false; // Position ist innerhalb des No-Tower-Bereichs
            }
        }
        return true; // Position ist gültig
        */
    }
    void SetVisibility(GameObject obj, bool visible)
    {
        if (obj == null)
        {
            return;
        }
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }
    void ChangeRangeMaterial(Material material)
    {
        if (previewObject == null) return;

        Transform rangeChild = previewObject.transform.Find("Range"); // Suche nach dem Child-Objekt "Range"
        if (rangeChild != null)
        {
            Renderer rangeRenderer = rangeChild.GetComponent<Renderer>();
            if (rangeRenderer != null)
            {
                rangeRenderer.material = material; // Material ändern
            }
        }
    }
    void SetTerrainCircleHeight(Vector3 worldPos, float radius, float targetWorldHeight)
    {
        Terrain terrain = Terrain.activeTerrain;
        if (terrain == null) return;

        TerrainData data = terrain.terrainData;

        int resolution = data.heightmapResolution;

        // Position relativ zum Terrain
        Vector3 terrainPos = worldPos - terrain.transform.position;

        int centerX = Mathf.RoundToInt((terrainPos.x / data.size.x) * resolution);
        int centerZ = Mathf.RoundToInt((terrainPos.z / data.size.z) * resolution);

        int radiusInSamples = Mathf.RoundToInt((radius / data.size.x) * resolution);

        int size = radiusInSamples * 2;

        // Clamp gegen Randfehler
        int startX = Mathf.Clamp(centerX - radiusInSamples, 0, resolution - 1);
        int startZ = Mathf.Clamp(centerZ - radiusInSamples, 0, resolution - 1);

        size = Mathf.Clamp(size, 0, resolution - startX);
        size = Mathf.Clamp(size, 0, resolution - startZ);

        float[,] heights = data.GetHeights(startX, startZ, size, size);

        // Zielhöhe normalisieren (0–1)
        float normalizedHeight =
            (targetWorldHeight - terrain.transform.position.y) / data.size.y;

        normalizedHeight = Mathf.Clamp01(normalizedHeight);

        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float dist = Vector2.Distance(
                    new Vector2(x, z),
                    new Vector2(radiusInSamples, radiusInSamples));

                if (dist < radiusInSamples)
                {
                    heights[z, x] = normalizedHeight;
                }
            }
        }

        data.SetHeights(startX, startZ, heights);
    }
}
