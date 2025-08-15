using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float spawnRate = 2f;
    public float minY = -1f;
    public float maxY = 2f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnPipe), 1f, spawnRate);
    }

    void SpawnPipe()
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);
        Instantiate(pipePrefab, spawnPos, Quaternion.identity);
    }
}
