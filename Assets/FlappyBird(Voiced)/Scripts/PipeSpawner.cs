using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;

    public float minY = -1f;
    public float maxY = 2f;

    public float baseSpawnRate = 2f;
    public float minSpawnRate = 0.7f;
    public float difficultyRampDuration = 60f;

    private float timer = 0f;
    private float elapsedTime = 0f;
    private bool isSpawning = false;

    void Update()
    {
        if (!isSpawning) return;

        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        float difficultyPercent = Mathf.Clamp01(elapsedTime / difficultyRampDuration);
        float currentSpawnRate = Mathf.Lerp(baseSpawnRate, minSpawnRate, difficultyPercent);

        if (timer >= currentSpawnRate)
        {
            SpawnPipe(difficultyPercent);
            timer = 0f;
        }
    }

    void SpawnPipe(float difficultyPercent)
    {
        float randomY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);
        GameObject newPipe = Instantiate(pipePrefab, spawnPos, Quaternion.identity);
        newPipe.tag = "Pipe";

        // Send difficulty % to PipeMovement
        PipeMovement pipeMovement = newPipe.GetComponent<PipeMovement>();
        if (pipeMovement != null)
        {
            pipeMovement.SetDifficulty(difficultyPercent);
        }
    }

    public void StartSpawning()
    {
        timer = 0f;
        elapsedTime = 0f;
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
}
