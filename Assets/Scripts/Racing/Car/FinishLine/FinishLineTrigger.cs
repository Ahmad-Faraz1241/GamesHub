using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    public TimerManager timerManager;         // Assign in Inspector
    public ResultUIManager resultUIManager;   // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("🏁 Triggered by: " + other.name);

            // Stop timer and logic
            timerManager.Win();

            // Show result UI as Win
            resultUIManager.ShowResult(true);
        }
    }
}
