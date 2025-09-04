// Spawner3D.cs
using System.Collections.Generic;
using UnityEngine;

public class Spawner3D : MonoBehaviour
{
    [System.Serializable]
    public class Fruit3D
    {
        public GameObject wholePrefab;
        public int poolSize = 10;
    }

    [Header("Fruit Settings")]
    public Fruit3D[] fruits;
    public float spawnInterval = 1.5f;
    public float minUpForce = 8f;
    public float maxUpForce = 12f;
    public float horizontalForce = 1.5f;

    [Header("Spawn Area")]
    public float xRange = 2.5f;
    public float spawnY = -5f;
    public float despawnY = -7f;

    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    public int bombPoolSize = 20;

    [Header("Difficulty Curve")]
    public float difficultyIncreaseRate = 0.05f;
    public float maxBombChance = 0.3f;
    public float maxFruitCount = 3;

    [HideInInspector] public bool spawningActive = false;

    private float bombChance = 0.05f;
    private float elapsedTime = 0f;

    private List<List<GameObject>> fruitPools = new List<List<GameObject>>();
    private List<GameObject> bombPool = new List<GameObject>();

    void Start()
    {
        Debug.Log("Spawner3D.Start() called - creating pools");
        CreatePools();
    }

    void Update()
    {
        if (!spawningActive) return;

        elapsedTime += Time.deltaTime;
        bombChance = Mathf.Min(maxBombChance, bombChance + difficultyIncreaseRate * Time.deltaTime);
        maxUpForce = Mathf.Min(16f, 12f + elapsedTime / 30f);
    }

    public void StartSpawning()
    {
        Debug.Log("Spawner3D -> StartSpawning");

        elapsedTime = 0f;
        bombChance = 0.05f;
        spawningActive = true;

        CancelInvoke(nameof(SpawnWave));
        InvokeRepeating(nameof(SpawnWave), 0.5f, spawnInterval);
    }

    public void StopSpawning()
    {
        Debug.Log("Spawner3D -> StopSpawning");

        spawningActive = false;
        CancelInvoke(nameof(SpawnWave));
    }

    private void CreatePools()
    {
        // Fruits
        foreach (var fruit in fruits)
        {
            List<GameObject> pool = new List<GameObject>();
            for (int i = 0; i < fruit.poolSize; i++)
            {
                GameObject obj = Instantiate(fruit.wholePrefab, transform);
                obj.SetActive(false);

                if (obj.GetComponent<FruitRecycle>() == null)
                    obj.AddComponent<FruitRecycle>().despawnY = despawnY;

                pool.Add(obj);
            }
            fruitPools.Add(pool);
        }

        // Bombs
        if (bombPrefab != null)
        {
            for (int i = 0; i < bombPoolSize; i++)
            {
                GameObject bomb = Instantiate(bombPrefab, transform);
                bomb.SetActive(false);
                bombPool.Add(bomb);
            }
        }
    }

    private void SpawnWave()
    {
        if (!spawningActive || fruits.Length == 0) return;

        int maxCount = Mathf.Clamp(Mathf.FloorToInt(1 + elapsedTime / 15f), 1, (int)maxFruitCount);
        int count = Random.Range(1, maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            float xPos = Random.Range(-xRange, xRange);

            if (bombPrefab != null && Random.value < bombChance)
                SpawnBomb(xPos);
            else
            {
                int index = Random.Range(0, fruits.Length);
                SpawnFruit(index, xPos);
            }
        }
    }

    private void SpawnFruit(int index, float xPos)
    {
        List<GameObject> pool = fruitPools[index];
        GameObject fruit = pool.Find(f => !f.activeInHierarchy);

        if (fruit == null)
        {
            fruit = Instantiate(fruits[index].wholePrefab, transform);
            pool.Add(fruit);

            if (fruit.GetComponent<FruitRecycle>() == null)
                fruit.AddComponent<FruitRecycle>().despawnY = despawnY;
        }

        fruit.transform.position = new Vector3(xPos, spawnY, 0f);
        fruit.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        fruit.SetActive(true);

        FruitSlicer3D slicer = fruit.GetComponent<FruitSlicer3D>();
        if (slicer != null) slicer.ResetSliced();

        Rigidbody rb = fruit.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(new Vector3(
                Random.Range(-horizontalForce, horizontalForce),
                Random.Range(minUpForce, maxUpForce),
                0f
            ), ForceMode.Impulse);
        }
    }

    private void SpawnBomb(float xPos)
    {
        GameObject bomb = bombPool.Find(b => !b.activeInHierarchy);

        if (bomb == null)
        {
            bomb = Instantiate(bombPrefab, transform);
            bomb.SetActive(false);
            bombPool.Add(bomb);
        }

        bomb.transform.position = new Vector3(xPos, spawnY, 0f);
        bomb.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        bomb.SetActive(true);

        Bomb3D bombScript = bomb.GetComponent<Bomb3D>();
        if (bombScript != null) bombScript.spawner = this;

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(new Vector3(
                Random.Range(-horizontalForce, horizontalForce),
                Random.Range(minUpForce, maxUpForce),
                0f
            ), ForceMode.Impulse);
        }
    }
}
