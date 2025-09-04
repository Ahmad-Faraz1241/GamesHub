using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject restartButton;     // Optional
    public GameObject goBackButton;      // Optional
    public GameObject loadingPanel;
    public GameObject crashPanel;
    public Slider loadingSlider;
    public TMP_Text coinText;

    [Header("Game Objects")]
    public GameObject player;
    public GameObject tileSpawner;

    private int coinCount = 0;
    private bool gameStarted = false;

    private Vector3 initialPlayerPos;
    private Quaternion initialPlayerRot;
    private CharacterController characterController;
    private RunnerMovement runner;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;

        mainMenuPanel.SetActive(true);
        loadingPanel.SetActive(false);
        crashPanel.SetActive(false);

        restartButton?.SetActive(false);
        goBackButton?.SetActive(false);

        player.SetActive(false);
        tileSpawner.SetActive(false);

        initialPlayerPos = player.transform.position;
        initialPlayerRot = player.transform.rotation;
        characterController = player.GetComponent<CharacterController>();
        runner = player.GetComponent<RunnerMovement>();

        UpdateCoinUI();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        loadingPanel.SetActive(true);
        crashPanel.SetActive(false);

        if (loadingSlider != null)
            loadingSlider.value = 0f;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            if (loadingSlider != null)
                loadingSlider.value = progress;

            yield return null;
        }

        coinCount = 0;
        UpdateCoinUI();

        mainMenuPanel.SetActive(false);
        restartButton?.SetActive(false);
        goBackButton?.SetActive(false);
        loadingPanel.SetActive(false);

        player.SetActive(true);
        tileSpawner.SetActive(true);
        gameStarted = true;

        ResetPlayerPosition();
        ResetTiles();

        runner?.ResetMovement();
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameRoutine());
    }

    private IEnumerator RestartGameRoutine()
    {
        loadingPanel.SetActive(true);
        crashPanel.SetActive(false);

        if (loadingSlider != null)
            loadingSlider.value = 0f;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);

            if (loadingSlider != null)
                loadingSlider.value = progress;

            yield return null;
        }

        coinCount = 0;
        UpdateCoinUI();

        player.SetActive(false);
        tileSpawner.SetActive(false);

        ResetPlayerPosition();
        ResetTiles();

        player.SetActive(true);
        tileSpawner.SetActive(true);

        runner?.ResetMovement();

        loadingPanel.SetActive(false);
    }

    public void GoBackToMenu()
    {
        restartButton?.SetActive(false);
        goBackButton?.SetActive(false);
        crashPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        player.SetActive(false);
        tileSpawner.SetActive(false);

        ResetPlayerPosition();
        ResetTiles();

        gameStarted = false;
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateCoinUI();

        if (coinCount % 50 == 0 && runner != null)
        {
            runner.IncreaseSpeed(0.2f, 0.04f);
        }
    }

    void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coinCount;
    }

    void ResetPlayerPosition()
    {
        if (characterController != null)
            characterController.enabled = false;

        player.transform.position = initialPlayerPos;
        player.transform.rotation = initialPlayerRot;

        if (characterController != null)
            characterController.enabled = true;
    }

    void ResetTiles()
    {
        TileSpawner spawner = tileSpawner.GetComponent<TileSpawner>();
        if (spawner != null)
            spawner.ResetSpawner();
    }

    // Show crash panel after delay
    public void ShowCrashPanel()
    {
        StartCoroutine(ShowCrashPanelWithDelay());
    }

    private IEnumerator ShowCrashPanelWithDelay()
    {
        yield return new WaitForSeconds(3f);  // Delay before showing crash panel

        crashPanel.SetActive(true);

        if (restartButton != null)
            restartButton.SetActive(true);

        if (goBackButton != null)
            goBackButton.SetActive(true);
    }

}
