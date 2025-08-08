using UnityEngine;
using UnityEngine.SceneManagement;

public class TwoFingerSwipeSceneSwitcher : MonoBehaviour
{
    public float minSwipeDistance = 100f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private static string otherScene;
    private static AsyncOperation preloadOperation;

    // Time tracking
    private static float switchStartTime = -1f;

    void Start()
    {
        // If coming from another scene, calculate and log duration
        if (switchStartTime > 0)
        {
            float switchDuration = Time.time - switchStartTime;
            Debug.Log("Scene switch duration: " + switchDuration.ToString("F3") + " seconds");
            switchStartTime = -1f; // reset
        }

        CacheOtherSceneName();
        PreloadScene();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ActivatePreloadedScene();
        }
#else
        if (Input.touchCount == 2)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                startTouchPos = touch.position;
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPos = touch.position;
                DetectSwipe();
            }
        }
#endif
    }

    void DetectSwipe()
    {
        float swipeDistance = Vector2.Distance(startTouchPos, endTouchPos);
        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDir = endTouchPos - startTouchPos;

            if (Mathf.Abs(swipeDir.y) > Mathf.Abs(swipeDir.x))
            {
                ActivatePreloadedScene();
            }
        }
    }

    void CacheOtherSceneName()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name != currentScene)
            {
                otherScene = name;
                break;
            }
        }
    }

    void PreloadScene()
    {
        if (string.IsNullOrEmpty(otherScene)) return;

        preloadOperation = SceneManager.LoadSceneAsync(otherScene);
        preloadOperation.allowSceneActivation = false;
    }

    void ActivatePreloadedScene()
    {
        if (preloadOperation != null && !preloadOperation.allowSceneActivation)
        {
            switchStartTime = Time.time; // Record time before activation
            preloadOperation.allowSceneActivation = true;
        }
    }
}
