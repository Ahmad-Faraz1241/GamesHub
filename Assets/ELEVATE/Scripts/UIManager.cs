using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject playButton;

    private void Awake()
    {
        Instance = this; // Always overwrite
    }

    public void ShowPlayButton()
    {
        playButton.SetActive(true);
    }

    public void HidePlayButton()
    {
        playButton.SetActive(false);
    }

    public void OnPlayButtonPressed()
    {
        HidePlayButton();
        SpawnManager.Instance.StartGame();
    }
}
