using UnityEngine;
using UnityEngine.UI;

public class FlappyGameManager : MonoBehaviour
{
    [Header("References")]
    public BirdController bird;      // Drag BirdController script here
    public PipeSpawner pipeSpawner;  // Drag PipeSpawner GameObject here
    public Button startButton;       // Drag Start Button here

    void Start()
    {
        Time.timeScale = 1f;

        if (startButton != null)
        {
            startButton.onClick.RemoveAllListeners();
            startButton.onClick.AddListener(() =>
            {
                bird.StartGame();
            });
            startButton.gameObject.SetActive(true);
        }
    }
}
