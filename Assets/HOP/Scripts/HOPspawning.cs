using System.Collections.Generic;
using UnityEngine;

public class HOPspawning : MonoBehaviour
{
    [Header("Tile Setup")]
    public GameObject tilePrefab;
    public GameObject fakeTilePrefab;
    public Transform player;

    [Header("Tile Spacing")]
    public float tileSpacingX = 2f;
    public float tileSpacingZ = 3.2f;
    public float maxHorizontalRange = 6f;

    [Header("Spawning Logic")]
    public float maxTilesAhead = 1f;
    public float minTilesAhead = -0.3f;
    public int fakeTileStartScore = 20;
    public int fakeTileStepScore = 10;
    public float minXSpacing = 1.5f;
    public float tileWidth = 2f;

    [Header("Pastel Color Timing")]
    public float colorChangeInterval = 5f;

    private Vector3 lastTilePos;
    private int currentTileZ = 0;

    private Queue<GameObject> activeTiles = new Queue<GameObject>();
    private Queue<GameObject> tilePool = new Queue<GameObject>();
    private Queue<GameObject> fakeTilePool = new Queue<GameObject>();

    private Color currentFakeColor;
    private float lastColorChangeTime = 0f;

    void Start()
    {
        lastTilePos = new Vector3(
            Mathf.Round(player.position.x / tileSpacingX) * tileSpacingX,
            0f,
            Mathf.Round(player.position.z / tileSpacingZ) * tileSpacingZ
        );

        currentTileZ = Mathf.RoundToInt(lastTilePos.z / tileSpacingZ);
        currentFakeColor = GeneratePastelNonBlueColor();
        lastColorChangeTime = Time.time;

        SpawnNextTile(0);
    }

    void Update()
    {
        // 🔹 Stop spawning if game is paused
        if (HOPUIManager.Instance != null && Time.timeScale == 0f) return;

        int score = HOPScoreManager.Instance.GetScore();
        float dynamicTilesAhead = Mathf.Clamp(
            maxTilesAhead - (score * 0.02f),
            minTilesAhead,
            maxTilesAhead
        );

        float playerZIndex = player.position.z / tileSpacingZ;
        float tileZIndex = currentTileZ;

        if (playerZIndex >= tileZIndex - dynamicTilesAhead)
        {
            SpawnNextTile(score);
        }
    }

    void SpawnNextTile(int score)
    {
        if (Time.time - lastColorChangeTime > colorChangeInterval)
        {
            currentFakeColor = GeneratePastelNonBlueColor();
            lastColorChangeTime = Time.time;
        }

        List<float> usedXPositions = new List<float>();

        // Real Tile
        int dir = Random.value < 0.5f ? -1 : 1;
        Vector3 nextPos = lastTilePos + new Vector3(dir * tileSpacingX, 0f, tileSpacingZ);
        nextPos.x = Mathf.Clamp(nextPos.x, -maxHorizontalRange, maxHorizontalRange);

        GameObject realTile = GetPooledTile(tilePrefab, tilePool);
        realTile.transform.position = nextPos;
        realTile.transform.rotation = Quaternion.identity;
        realTile.tag = "Tile";

        Renderer tileRenderer = realTile.GetComponent<Renderer>();
        if (tileRenderer != null)
            tileRenderer.material.color = new Color(0.5f, 0.7f, 0.9f); // light blue

        usedXPositions.Add(nextPos.x);
        activeTiles.Enqueue(realTile);
        lastTilePos = nextPos;
        currentTileZ++;

        // Fake Tiles
        if (score >= fakeTileStartScore)
        {
            float range = Mathf.Lerp(3f, maxHorizontalRange, Mathf.InverseLerp(20f, 100f, score));

            List<float> validXPositions = new List<float>();
            for (float x = -range; x <= range; x += tileWidth)
            {
                bool tooClose = false;
                foreach (float usedX in usedXPositions)
                {
                    if (Mathf.Abs(x - usedX) < tileWidth * 0.95f)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (!tooClose)
                    validXPositions.Add(x);
            }

            int maxFakesAllowed = Mathf.Clamp(validXPositions.Count, 0, 10);
            int targetFakeCount = Mathf.Min(
                (score - fakeTileStartScore) / fakeTileStepScore + 1,
                maxFakesAllowed
            );

            if (validXPositions.Count > 0 && targetFakeCount > 0)
            {
                Shuffle(validXPositions);

                for (int i = 0; i < targetFakeCount; i++)
                {
                    Vector3 fakePos = nextPos + new Vector3(validXPositions[i] - nextPos.x, 0f, 0f);
                    fakePos.x = Mathf.Clamp(fakePos.x, -maxHorizontalRange, maxHorizontalRange);

                    GameObject fakeTile = GetPooledTile(fakeTilePrefab, fakeTilePool);
                    fakeTile.transform.position = fakePos;
                    fakeTile.transform.rotation = Quaternion.identity;
                    fakeTile.tag = "FakeTile";

                    Renderer fakeRenderer = fakeTile.GetComponent<Renderer>();
                    if (fakeRenderer != null)
                        fakeRenderer.material.color = currentFakeColor;

                    activeTiles.Enqueue(fakeTile);
                    usedXPositions.Add(validXPositions[i]);
                }
            }
        }

        // Cleanup
        while (activeTiles.Count > 30)
        {
            GameObject tileToRemove = activeTiles.Dequeue();
            if (tileToRemove != null)
            {
                tileToRemove.SetActive(false);
                Collider col = tileToRemove.GetComponent<Collider>();
                if (col != null) col.enabled = false;

                if (tileToRemove.CompareTag("Tile"))
                    tilePool.Enqueue(tileToRemove);
                else
                    fakeTilePool.Enqueue(tileToRemove);
            }
        }
    }

    GameObject GetPooledTile(GameObject prefab, Queue<GameObject> pool)
    {
        GameObject tile = pool.Count > 0 ? pool.Dequeue() : Instantiate(prefab);
        tile.SetActive(true);
        Collider col = tile.GetComponent<Collider>();
        if (col != null) col.enabled = true;
        return tile;
    }

    Color GeneratePastelNonBlueColor()
    {
        float hue;
        do
        {
            hue = Random.value;
        } while (hue > 0.55f && hue < 0.7f); // skip blue hues

        float saturation = Random.Range(0.2f, 0.35f); // pastel
        float value = Random.Range(0.9f, 1f);         // light

        return Color.HSVToRGB(hue, saturation, value);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }



    // Add this at the bottom of HOPspawning.cs
    

}
