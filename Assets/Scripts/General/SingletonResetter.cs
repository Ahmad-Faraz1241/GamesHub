using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonResetter : MonoBehaviour
{
    // Runs automatically before any scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ClearSingletons()
    {
        AudioManager.Instance = null;
        UIManager.Instance = null;
        GameManager2.Instance = null;
        SpawnManager.Instance = null;
        ScoreManager.Instance = null;
    }
}
