using UnityEngine;
using TMPro;

public class ScoreManager3D : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Assign in Canvas
    private int score = 0;

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    public int GetScore() => score;
}
