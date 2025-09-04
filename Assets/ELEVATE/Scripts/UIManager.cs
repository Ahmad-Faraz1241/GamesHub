using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject loadingPanel;

    [Header("Loading UI")]
    public Slider loadingSlider;
    public float loadingDuration = 1.5f; // Duration of loading animation in seconds

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowMainMenu();
    }

    public void OnPlayButtonPressed()
    {
        StartCoroutine(TransitionToGame());
    }

    public void OnRestartButtonPressed()
    {
        StartCoroutine(RestartGame());
    }

    public void OnMenuButtonPressed()
    {
        StartCoroutine(BackToMenu());
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        loadingPanel.SetActive(false);
    }

    public void ShowGamePanel()
    {
        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOverPanel()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void ShowLoadingPanel()
    {
        loadingPanel.SetActive(true);
        if (loadingSlider != null)
            loadingSlider.value = 0f;
    }

    public void HideLoadingPanel()
    {
        loadingPanel.SetActive(false);
    }

    private IEnumerator AnimateLoading(System.Action onComplete)
    {
        ShowLoadingPanel();

        float elapsed = 0f;
        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            if (loadingSlider != null)
                loadingSlider.value = Mathf.Clamp01(elapsed / loadingDuration);
            yield return null;
        }

        HideLoadingPanel();
        onComplete?.Invoke();
    }

    private IEnumerator TransitionToGame()
    {
        yield return StartCoroutine(AnimateLoading(() =>
        {
            ShowGamePanel();
            GameManager2.Instance.StartGame();
        }));
    }

    private IEnumerator RestartGame()
    {
        // Hide GameOver panel before showing loading
        gameOverPanel.SetActive(false);

        yield return StartCoroutine(AnimateLoading(() =>
        {
            ShowGamePanel();
            SpawnManager.Instance.StartGame();
        }));
    }
    private IEnumerator BackToMenu()
    {
        // Hide GameOver panel before showing loading
        gameOverPanel.SetActive(false);

        yield return StartCoroutine(AnimateLoading(() =>
        {
            ShowMainMenu();
        }));
    }

}
