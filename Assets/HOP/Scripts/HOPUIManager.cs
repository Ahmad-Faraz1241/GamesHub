using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HOPUIManager : MonoBehaviour
{
    public static HOPUIManager Instance;

    public GameObject playButton;
    public float showButtonDelay = 1.5f; // Delay before showing Play again after game over

    private bool isGameOver = false;

    private HOPmovement playerMovement;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        playerMovement = FindObjectOfType<HOPmovement>();
        ShowPlayButton();
        PauseGame();
    }

    public void OnPlayButtonClicked()
    {
        if (isGameOver)
        {
            // Restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        // Start game
        HidePlayButton();
        ResumeGame();
        HOPmovement.isGameActive = true;
    }

    public void ShowPlayButton()
    {
        playButton.SetActive(true);
    }

    public void HidePlayButton()
    {
        playButton.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public void OnGameOver()
    {
        isGameOver = true;
        StartCoroutine(ShowButtonAfterDelay(showButtonDelay));
        PauseGame();
    }

    private IEnumerator ShowButtonAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ShowPlayButton();
    }
}
