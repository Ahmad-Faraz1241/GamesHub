using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public float totalTime = 60f; // Total time to complete
    private float timeRemaining;
    private bool timerRunning = false;
    private bool resultShown = false;

    public TextMeshProUGUI timerText;
    public BasicCarController carController; // Assign in Inspector
    public ResultUIManager resultUIManager;  // ✅ Assign in Inspector

    void Start()
    {
        timeRemaining = totalTime;
        timerRunning = true;
        resultShown = false;
    }

    void Update()
    {
        if (!timerRunning || resultShown) return;

        timeRemaining -= Time.deltaTime;
        timeRemaining = Mathf.Max(timeRemaining, 0f);
        timerText.text = Mathf.CeilToInt(timeRemaining).ToString();

        if (timeRemaining <= 0f)
        {
            timerRunning = false;
            resultShown = true;

            carController.LockControlsAndStopCar(false);  // Lose
            resultUIManager.ShowResult(false);            // ❌ YOU LOSE!
        }
    }

    public void Win()
    {
        if (!resultShown)
        {
            timerRunning = false;
            resultShown = true;

            carController.LockControlsAndStopCar(true);   // Win
            resultUIManager.ShowResult(true);             // 🎉 YOU WIN!
        }
    }
}
