using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HOPScoreManager : MonoBehaviour
{
    public static HOPScoreManager Instance;

    private int currentScore = 0;

    [Header("UI")]
    public TMP_Text scoreText; // Assign this in the Unity Inspector

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddPoint()
    {
        currentScore++;
        UpdateScoreText();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreText();
    }

    public int GetScore()
    {
        return currentScore;
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = " " + currentScore.ToString();
        }
    }
}
