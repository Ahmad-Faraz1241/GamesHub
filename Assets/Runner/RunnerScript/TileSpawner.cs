using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public GameObject defaultTilePrefab;
    public GameObject[] easyTiles;  // Tiles for first minute
    public GameObject[] hardTiles;  // Tiles after first minute

    [Header("Spawn Settings")]
    public float segmentLength = 20f;
    public Transform player;
    public int initialSegments = 5;
    public int maxSegmentsOnScreen = 6;
    public float yOffsetPerTile = 0f;

    [Header("Difficulty Settings")]
    public float easyDuration = 60f;  // time in seconds to spawn easy tiles

    private float spawnZ = 0f;
    private float currentY = 0f;
    private List<GameObject> activeSegments = new();
    private int lastPrefabIndex = -1;
    private bool usedDefaultTile = false;
    private float elapsedTime = 0f;

    void Start()
    {
        if (segmentLength <= 0 && easyTiles.Length > 0)
        {
            segmentLength = GetSegmentLength(easyTiles[0]);
        }

        for (int i = 0; i < initialSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

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

        // First tile is always default
        if (!usedDefaultTile && defaultTilePrefab != null)
        {
            prefabToSpawn = defaultTilePrefab;
            usedDefaultTile = true;
        }
        else
        {
            // Check elapsed time to decide difficulty
            if (elapsedTime < easyDuration)
                prefabToSpawn = GetRandomTile(easyTiles);
            else
                prefabToSpawn = GetRandomTile(hardTiles);
        }

        Vector3 spawnPos = new Vector3(0, currentY, spawnZ);
        GameObject newSeg = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
        activeSegments.Add(newSeg);

        spawnZ += segmentLength;
        currentY -= yOffsetPerTile;

        ResetCoinsInSegment(newSeg);
    }

    GameObject GetRandomTile(GameObject[] prefabs)
    {
        int index;
        do
        {
            index = Random.Range(0, prefabs.Length);
        } while (prefabs.Length > 1 && index == lastPrefabIndex);

        lastPrefabIndex = index;
        return prefabs[index];
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
        elapsedTime = 0f;

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
        return 20f;
    }
}
