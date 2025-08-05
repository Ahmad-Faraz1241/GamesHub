using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;
    public bool isGameRunning = false;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        Debug.Log("GameManager2: Starting Game");
        Time.timeScale = 1f; // Always resume game
        isGameRunning = true;
        SpawnManager.Instance.StartGame();
    }

    public void GameOver()
    {
        Debug.Log("GameManager2: Game Over");
        isGameRunning = false;
        UIManager.Instance?.ShowPlayButton();
    }
}
