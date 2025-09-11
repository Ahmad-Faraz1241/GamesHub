using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;

    public float minY = -2f;          // Lower limit for pipe height
    public float maxY = 3f;           // Upper limit for pipe height
    public float verticalGap = 1.5f;  // Minimum vertical gap between consecutive pipes

    public float baseSpawnRate = 2f;  // Maximum spawn interval (start of game)
    public float minSpawnRate = 0.7f; // Minimum spawn interval (highest difficulty)
    public float difficultyRampDuration = 60f;

    public float randomSpawnVariance = 0.5f; // Randomize spawn interval a bit
    private float timer = 0f;
    private float elapsedTime = 0f;
    private bool isSpawning = false;
    private float lastPipeY = 0f;

    void Update()
    {
        if (!isSpawning) return;

        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        float difficultyPercent = Mathf.Clamp01(elapsedTime / difficultyRampDuration);
        float targetSpawnRate = Mathf.Lerp(baseSpawnRate, minSpawnRate, difficultyPercent);

        // Add some random variation to spawn interval
        float currentSpawnRate = targetSpawnRate + Random.Range(0f, randomSpawnVariance);

        if (timer >= currentSpawnRate)
        {
            SpawnPipe(difficultyPercent);
            timer = 0f;
        }
    }

    void SpawnPipe(float difficultyPercent)
    {
        float randomY;

        // Ensure the new pipe has enough vertical distance from the previous one
        int attempts = 0;
        do
        {
            randomY = Random.Range(minY, maxY);
            attempts++;
        } while (Mathf.Abs(randomY - lastPipeY) < verticalGap && attempts < 10);

        lastPipeY = randomY;

        Vector3 spawnPos = new Vector3(transform.position.x, randomY, 0);
        GameObject newPipe = Instantiate(pipePrefab, spawnPos, Quaternion.identity);
        newPipe.tag = "Pipe";

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
        lastPipeY = Random.Range(minY, maxY); // first pipe starts randomly
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }
}
