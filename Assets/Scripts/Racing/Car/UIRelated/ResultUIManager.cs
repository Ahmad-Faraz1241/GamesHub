using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultUIManager : MonoBehaviour
{
    public Canvas resultCanvas;
    public TextMeshProUGUI resultTMP;

    void Start()
    {
        resultCanvas.gameObject.SetActive(false);
    }

    public void ShowResult(bool didWin)
    {
        resultCanvas.gameObject.SetActive(true);
        resultTMP.text = didWin ? "🎉 YOU WIN!" : "❌ YOU LOSE!";
        resultTMP.color = didWin ? Color.green : Color.red;
    }

    public void playAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void changelocation()
    {
        SceneManager.LoadScene("locationselection");
    }
}
