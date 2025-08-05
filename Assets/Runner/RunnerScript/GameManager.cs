using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject mainMenuPanel;
    public GameObject restartButton;
    public GameObject goBackButton;
    public GameObject quitButton;
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
        mainMenuPanel.SetActive(true);
        restartButton.SetActive(false);
        goBackButton.SetActive(false);
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
        coinCount = 0;
        UpdateCoinUI();

        mainMenuPanel.SetActive(false);
        restartButton.SetActive(true);
        goBackButton.SetActive(true);
        player.SetActive(true);
        tileSpawner.SetActive(true);
        gameStarted = true;

        ResetPlayerPosition();
        ResetTiles();

        if (runner != null)
            runner.ResetMovement();
    }

    public void RestartGame()
    {
        coinCount = 0;
        UpdateCoinUI();

        player.SetActive(false);
        tileSpawner.SetActive(false);

        ResetPlayerPosition();
        ResetTiles();

        player.SetActive(true);
        tileSpawner.SetActive(true);

        if (runner != null)
            runner.ResetMovement();
    }

    public void GoBackToMenu()
    {
        restartButton.SetActive(false);
        goBackButton.SetActive(false);
        mainMenuPanel.SetActive(true);

        player.SetActive(false);
        tileSpawner.SetActive(false);

        ResetPlayerPosition();
        ResetTiles();

        gameStarted = false;
    }

    public void QuitGame()
    {
        Application.Quit();
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
}
