using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Tile Setup")]
    public GameObject defaultTilePrefab;
    public GameObject[] segmentPrefabs;

    [Tooltip("Set this to the Z-size of your tile (e.g., 20 if your tile is 1x1x20)")]
    public float segmentLength = 20f;

    [Header("Player Reference")]
    public Transform player;

    [Header("Spawning Settings")]
    public int initialSegments = 5;
    public int maxSegmentsOnScreen = 6;

    [Header("Slope Settings")]
    public float yOffsetPerTile = 0f;

    private float spawnZ = 0f;
    private float currentY = 0f;
    private List<GameObject> activeSegments = new();
    private int lastPrefabIndex = -1;
    private bool usedDefaultTile = false;

    void Start()
    {
        // Auto-detect tile length if not set
        if (segmentLength <= 0 && segmentPrefabs.Length > 0)
        {
            segmentLength = GetSegmentLength(segmentPrefabs[0]);
        }

        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player.position.z > spawnZ - (segmentLength * (maxSegmentsOnScreen - 1)))
        {
            SpawnSegment();
        }

        if (activeSegments.Count > maxSegmentsOnScreen)
        {
            GameObject oldest = activeSegments[0];
            float buffer = 15f;

            if (player.position.z > oldest.transform.position.z + segmentLength + buffer)
            {
                Destroy(oldest);
                activeSegments.RemoveAt(0);
            }
        }
    }

    void SpawnSegment()
    {
        GameObject prefabToSpawn;

        if (!usedDefaultTile && defaultTilePrefab != null)
        {
            prefabToSpawn = defaultTilePrefab;
            usedDefaultTile = true;
        }
        else
        {
            int index;
            do
            {
                index = Random.Range(0, segmentPrefabs.Length);
            } while (segmentPrefabs.Length > 1 && index == lastPrefabIndex);

            lastPrefabIndex = index;
            prefabToSpawn = segmentPrefabs[index];
        }

        // Corrected: place the tile at the start of the next Z position
        Vector3 spawnPos = new Vector3(0, currentY, spawnZ);
        GameObject newSeg = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        activeSegments.Add(newSeg);

        spawnZ += segmentLength;
        currentY -= yOffsetPerTile;

        ResetCoinsInSegment(newSeg);
    }

    void ResetCoinsInSegment(GameObject tile)
    {
        foreach (CoinSpin coin in tile.GetComponentsInChildren<CoinSpin>(true))
        {
            coin.ResetCoin();
        }
    }

    public void ResetSpawner()
    {
        foreach (GameObject tile in new List<GameObject>(activeSegments))
        {
            Destroy(tile);
        }
        activeSegments.Clear();

        spawnZ = 0f;
        currentY = 0f;
        usedDefaultTile = false;

        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    float GetSegmentLength(GameObject prefab)
    {
        MeshRenderer renderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.z;
        }
        return 20f; // fallback
    }
}
