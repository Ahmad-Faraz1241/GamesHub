using UnityEngine;
using UnityEngine.UI;

public class FlappyGameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject bird;           // Assign Bird GameObject
    public GameObject pipeSpawner;    // Assign PipeSpawner GameObject
    public Button startButton;        // Assign UI Start Button

    void Start()
    {
        // Ensure gameplay runs after scene activation/preload
        Time.timeScale = 1f;

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(StartGame);
            startButton.gameObject.SetActive(true);
        }

        // Hide gameplay objects until Start is pressed
        if (bird != null) bird.SetActive(false);
        if (pipeSpawner != null) pipeSpawner.SetActive(false);
    }

    public void StartGame()
    {
        // Safety: make sure time is running when starting gameplay
        Time.timeScale = 1f;

        if (bird != null) bird.SetActive(true);
        if (pipeSpawner != null) pipeSpawner.SetActive(true);
        if (startButton != null) startButton.gameObject.SetActive(false);
    }
}