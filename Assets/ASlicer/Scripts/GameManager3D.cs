using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager3D : MonoBehaviour
{
    [Header("References")]
    public Spawner3D spawner;
    public ScoreManager3D scoreManager;

    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject loadingPanel;
    public GameObject gameOverPanel;

    [Header("Loading UI")]
    public Slider loadingSlider;

    [Header("Loading Settings")]
    public float loadingDuration = 2f;

    void Start()
    {
        Time.timeScale = 1f;
        ShowPanel(mainMenuPanel);
        HidePanel(loadingPanel);
        HidePanel(gameOverPanel);
        spawner?.StopSpawning();
    }

    public void OnPlayPressed()
    {
        StartCoroutine(ShowLoadingThen(StartGame));
    }

    private void StartGame()
    {
        scoreManager?.ResetScore();
        spawner?.StartSpawning();
        HidePanel(mainMenuPanel);
        HidePanel(gameOverPanel);
    }

    public void OnBombSliced()
    {
        spawner?.StopSpawning();
        ShowPanel(gameOverPanel);
    }

    public void OnRestartPressed()
    {
        scoreManager?.ResetScore();
        spawner?.StartSpawning();
        HidePanel(gameOverPanel);
        HidePanel(mainMenuPanel);
    }

    public void OnGoBackPressed()
    {
        spawner?.StopSpawning();
        ShowPanel(mainMenuPanel);
        HidePanel(gameOverPanel);
    }

    private IEnumerator ShowLoadingThen(System.Action onComplete)
    {
        ShowPanel(loadingPanel);
        loadingSlider.value = 0f;

        float t = 0f;
        while (t < loadingDuration)
        {
            t += Time.deltaTime;
            loadingSlider.value = Mathf.Clamp01(t / loadingDuration);
            yield return null;
        }

        HidePanel(loadingPanel);
        onComplete?.Invoke();
    }

    private void ShowPanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(true);
    }

    private void HidePanel(GameObject panel)
    {
        if (panel != null) panel.SetActive(false);
    }
}
