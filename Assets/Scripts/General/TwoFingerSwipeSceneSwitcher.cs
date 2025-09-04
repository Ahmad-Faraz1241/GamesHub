using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Switches to the next scene on a two-finger vertical swipe.
/// All scenes preload for faster switching, except ShapeSlicer which reloads fresh.
/// </summary>
public class TwoFingerSwipeSceneSwitcher : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 100f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private static int nextSceneIndex;
    private static AsyncOperation preloadOperation;
    private static float switchStartTime = -1f;

    private const string reloadSceneName = "ShapeSlicer"; // Scene that should always reload fresh

    void Start()
    {
        // Log duration if returning from a scene switch
        if (switchStartTime > 0)
        {
            float duration = Time.time - switchStartTime;
           
            switchStartTime = -1f;
        }

        CacheNextSceneIndex();
        PreloadScene();
    }

    void Update()
    {
#if UNITY_EDITOR
        // Editor test with arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ActivateScene();
        }
#else
        // On device: detect two-finger swipe
        if (Input.touchCount == 2)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPos = touch.position;
                DetectSwipe();
            }
        }
#endif
    }

    /// <summary>
    /// Detects if the gesture is a vertical two-finger swipe.
    /// </summary>
    void DetectSwipe()
    {
        float swipeDistance = Vector2.Distance(startTouchPos, endTouchPos);

        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDir = endTouchPos - startTouchPos;

            // Only trigger on vertical swipe
            if (Mathf.Abs(swipeDir.y) > Mathf.Abs(swipeDir.x))
            {
                ActivateScene();
            }
        }
    }

    /// <summary>
    /// Finds the next scene index (loops around).
    /// </summary>
    void CacheNextSceneIndex()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount <= 1)
        {
            return;
        }

        nextSceneIndex = (currentSceneIndex + 1) % sceneCount;
    }

    /// <summary>
    /// Preloads the next scene (including ShapeSlicer).
    /// </summary>
    void PreloadScene()
    {
        if (SceneManager.sceneCountInBuildSettings <= 1) return;

        string nextSceneName = GetSceneName(nextSceneIndex);

        // Preload all scenes including ShapeSlicer
        preloadOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        preloadOperation.allowSceneActivation = false;
       
    }

    /// <summary>
    /// Activates the next scene (all scenes use preloading now).
    /// </summary>
    void ActivateScene()
    {
        string nextSceneName = GetSceneName(nextSceneIndex);
        switchStartTime = Time.time;

        if (preloadOperation != null && !preloadOperation.allowSceneActivation)
        {
            Debug.Log($"Activating preloaded scene: {nextSceneName}");
            preloadOperation.allowSceneActivation = true; // activate preloaded
        }
        else
        {
            // Fallback if preload failed
           
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    /// <summary>
    /// Helper: Get scene name by build index.
    /// </summary>
    private string GetSceneName(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }
}
