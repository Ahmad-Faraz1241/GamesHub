using UnityEngine;

public class HOPDifficultyManager : MonoBehaviour
{
    private void Update()
    {
        if (HOPUIManager.Instance != null && Time.timeScale == 0f) return;

        int score = HOPScoreManager.Instance.GetScore();

        if (score >= 60)
            Time.timeScale = 1.65f;
        else if (score >= 50)
            Time.timeScale = 1.50f;
        else if (score >= 30)
            Time.timeScale = 1.30f;
        else
            Time.timeScale = 1.15f;
    }
}
