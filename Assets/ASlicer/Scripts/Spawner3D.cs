using UnityEngine;
using System.Collections.Generic;

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
    public float horizontalForce = 3f;

    [Header("Spawn Area")]
    public float xRange = 3f;
    public float spawnY = -5f;
    public float despawnY = -7f;

    [Header("Bomb Settings")]
    public GameObject bombPrefab;
    [Range(0f, 1f)] public float bombChance = 0.1f; // 10% chance

    private List<List<GameObject>> fruitPools = new List<List<GameObject>>();
    private bool spawningActive = true;

    void Start()
    {
        // Create fruit pools
        foreach (var fruit in fruits)
        {
            List<GameObject> pool = new List<GameObject>();
            for (int i = 0; i < fruit.poolSize; i++)
            {
                GameObject obj = Instantiate(fruit.wholePrefab);
                obj.SetActive(false);
                pool.Add(obj);

                if (obj.GetComponent<FruitRecycle>() == null)
                    obj.AddComponent<FruitRecycle>().despawnY = despawnY;
            }
            fruitPools.Add(pool);
        }

        InvokeRepeating(nameof(SpawnObject), 1f, spawnInterval);
    }

    void SpawnObject()
    {
        if (!spawningActive || fruits.Length == 0) return;

        // Bomb spawn chance
        if (bombPrefab != null && Random.value < bombChance)
        {
            SpawnBomb();
            return;
        }

        // Spawn fruit
        int index = Random.Range(0, fruits.Length);
        Fruit3D fruitData = fruits[index];
        List<GameObject> pool = fruitPools[index];

        GameObject fruit = pool.Find(f => !f.activeInHierarchy);
        if (fruit == null)
        {
            fruit = Instantiate(fruitData.wholePrefab);
            pool.Add(fruit);
            if (fruit.GetComponent<FruitRecycle>() == null)
                fruit.AddComponent<FruitRecycle>().despawnY = despawnY;
        }

        // Reset slicer state
        FruitSlicer3D slicer = fruit.GetComponent<FruitSlicer3D>();
        if (slicer != null) slicer.ResetSliced();

        // Horizontal spawn with margin to keep on-screen
        float margin = 0.5f;
        float spawnX = Random.Range(-xRange + margin, xRange - margin);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        fruit.transform.position = spawnPos;
        fruit.transform.rotation = rot;
        fruit.SetActive(true);

        // Rigidbody setup
        Rigidbody rb = fruit.GetComponent<Rigidbody>();
        if (rb == null) rb = fruit.AddComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Bias horizontal force toward screen center
        float xForce;
        if (spawnX < -xRange * 0.5f)
            xForce = Random.Range(0f, horizontalForce);
        else if (spawnX > xRange * 0.5f)
            xForce = Random.Range(-horizontalForce, 0f);
        else
            xForce = Random.Range(-horizontalForce, horizontalForce);

        // Vertical force with slight boost if spawned far from center
        float yForce = Random.Range(minUpForce, maxUpForce) + Mathf.Abs(spawnX) * 0.5f;
        rb.AddForce(new Vector3(xForce, yForce, 0f), ForceMode.Impulse);
    }

    void SpawnBomb()
    {
        float margin = 0.5f;
        float spawnX = Random.Range(-xRange + margin, xRange - margin);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        GameObject bomb = Instantiate(bombPrefab, spawnPos, rot);

        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        if (rb == null) rb = bomb.AddComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        float xForce;
        if (spawnX < -xRange * 0.5f)
            xForce = Random.Range(0f, horizontalForce);
        else if (spawnX > xRange * 0.5f)
            xForce = Random.Range(-horizontalForce, 0f);
        else
            xForce = Random.Range(-horizontalForce, horizontalForce);

        float yForce = Random.Range(minUpForce, maxUpForce) + Mathf.Abs(spawnX) * 0.5f;
        rb.AddForce(new Vector3(xForce, yForce, 0f), ForceMode.Impulse);

        // Assign spawner reference to bomb
        Bomb3D bombScript = bomb.GetComponent<Bomb3D>();
        if (bombScript != null)
            bombScript.spawner = this;
    }

    // Called by bomb to stop spawning
    public void StopSpawning()
    {
        spawningActive = false;
    }
}
