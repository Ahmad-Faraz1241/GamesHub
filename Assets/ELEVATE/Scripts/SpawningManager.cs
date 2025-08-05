using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public GameObject movingCubePrefab;
    public Transform lastCube;
    public Camera mainCamera;

    public float verticalSpacingMultiplier = 2f;

    private float tileHue;
    private float backgroundHue;
    private float hueStep = 0.015f;
    private float backgroundLerpSpeed = 1.5f;
    private Color targetBackgroundColor;
    private bool isGameRunning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnsureLastCube();

        tileHue = Random.Range(0f, 1f);
        SetBackgroundHueFarFromTileHue();
        targetBackgroundColor = GetPastelColor(backgroundHue);
        mainCamera.backgroundColor = targetBackgroundColor;

        if (lastCube != null)
            lastCube.gameObject.SetActive(false);

        UIManager.Instance?.ShowPlayButton();
    }

    private void Update()
    {
        mainCamera.backgroundColor = Color.Lerp(mainCamera.backgroundColor, targetBackgroundColor, Time.deltaTime * backgroundLerpSpeed);
    }

    private void EnsureLastCube()
    {
        if (lastCube == null)
        {
            MovingCube baseCube = FindObjectOfType<MovingCube>();
            if (baseCube != null)
            {
                lastCube = baseCube.transform;
                Debug.Log("SpawnManager: Found lastCube = " + lastCube.name);
            }
            else
            {
                Debug.LogWarning("SpawnManager: No lastCube found in scene!");
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("=== Starting Game ===");
        Time.timeScale = 1f; // Ensure game is unpaused
        EnsureLastCube();

        if (lastCube == null)
        {
            Debug.LogError("Cannot start game — lastCube is missing!");
            return;
        }

        isGameRunning = true;
        lastCube.localScale = movingCubePrefab.transform.localScale;
        lastCube.position = Vector3.zero;
        lastCube.gameObject.SetActive(true);

        tileHue = Random.Range(0f, 1f);
        SetBackgroundHueFarFromTileHue();

        Color baseColor = GetPastelColor(tileHue);
        Renderer baseRenderer = lastCube.GetComponent<Renderer>();
        baseRenderer.material = new Material(baseRenderer.material);
        baseRenderer.material.color = baseColor;

        targetBackgroundColor = GetPastelColor(backgroundHue, 0.15f, 0.85f);
        mainCamera.backgroundColor = targetBackgroundColor;

        ClearScene();
        SpawnNewCube();
        UIManager.Instance?.HidePlayButton();
        ScoreManager.Instance?.ResetScore();
    }

    public void GameOver()
    {
        isGameRunning = false;
        UIManager.Instance?.ShowPlayButton();
    }

    public void SpawnNewCube()
    {
        if (!isGameRunning) return;

        float cubeHeight = lastCube.localScale.y;
        Vector3 spawnPosition = lastCube.position + new Vector3(0, cubeHeight * verticalSpacingMultiplier, 0);

        GameObject cubeObject = Instantiate(movingCubePrefab, spawnPosition, Quaternion.identity);
        MovingCube cube = cubeObject.GetComponent<MovingCube>();
        cube.transform.localScale = lastCube.localScale;

        bool moveOnX = Random.value > 0.5f;
        bool spawnFromNegativeSide = Random.value > 0.5f;

        if (moveOnX)
        {
            cube.moveDirection = spawnFromNegativeSide ? Vector3.right : Vector3.left;
            float startX = spawnFromNegativeSide ? -cube.movementLimit : cube.movementLimit;
            cube.transform.position = new Vector3(startX, spawnPosition.y, lastCube.position.z);
        }
        else
        {
            cube.moveDirection = spawnFromNegativeSide ? Vector3.forward : Vector3.back;
            float startZ = spawnFromNegativeSide ? -cube.movementLimit : cube.movementLimit;
            cube.transform.position = new Vector3(lastCube.position.x, spawnPosition.y, startZ);
        }

        tileHue += hueStep;
        if (tileHue > 1f) tileHue -= 1f;

        Color tileColor = GetPastelColor(tileHue);
        Renderer rend = cube.GetComponent<Renderer>();
        rend.material = new Material(rend.material);
        rend.material.color = tileColor;

        SetBackgroundHueFarFromTileHue();
        targetBackgroundColor = GetPastelColor(backgroundHue, 0.15f, 0.85f);
    }

    public void PlaceCube(MovingCube currentCube)
    {
        lastCube = currentCube.transform;
        AudioManager.Instance?.PlayPlace();
        ScoreManager.Instance?.AddScore();
        Invoke(nameof(SpawnNewCube), 0.25f);
    }

    private void ClearScene()
    {
        Debug.Log("Clearing old cubes...");
        foreach (var cube in FindObjectsOfType<MovingCube>())
        {
            if (lastCube != null && cube != lastCube.GetComponent<MovingCube>())
                Destroy(cube.gameObject);
        }

        foreach (var rb in FindObjectsOfType<Rigidbody>())
        {
            Destroy(rb.gameObject);
        }
    }

    private void SetBackgroundHueFarFromTileHue()
    {
        float offset = Random.Range(0.4f, 0.6f);
        backgroundHue = (tileHue + offset) % 1f;
    }

    private Color GetPastelColor(float hue, float saturation = 0.25f, float value = 0.95f)
    {
        return Color.HSVToRGB(hue, saturation, value);
    }
}
