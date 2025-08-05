using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TwoFingerSwipeSceneSwitcher_AdditiveSafe : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 100f;
    public string game1Scene = "Game1"; // Must match exactly in Build Settings
    public string game2Scene = "Game2"; // Must match exactly in Build Settings

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private string activeScene;

    void Start()
    {
        StartCoroutine(LoadBothScenes());
    }

    IEnumerator LoadBothScenes()
    {
        Debug.Log("Loading Game 1...");
        AsyncOperation loadGame1 = SceneManager.LoadSceneAsync(game1Scene, LoadSceneMode.Additive);
        while (!loadGame1.isDone) yield return null;

        Debug.Log("Loading Game 2...");
        AsyncOperation loadGame2 = SceneManager.LoadSceneAsync(game2Scene, LoadSceneMode.Additive);
        while (!loadGame2.isDone) yield return null;

        // Start with Game1 active
        SetActiveScene(game1Scene);
    }

    void Update()
    {
#if UNITY_EDITOR
        // Test in Editor with arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            SwitchScenes();
        }
#else
        // Mobile swipe detection
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

    void DetectSwipe()
    {
        float swipeDistance = Vector2.Distance(startTouchPos, endTouchPos);

        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDir = endTouchPos - startTouchPos;

            // Only vertical swipes
            if (Mathf.Abs(swipeDir.y) > Mathf.Abs(swipeDir.x))
            {
                SwitchScenes();
            }
        }
    }

    void SwitchScenes()
    {
        string targetScene = (activeScene == game1Scene) ? game2Scene : game1Scene;
        SetActiveScene(targetScene);
    }

    void SetActiveScene(string sceneName)
    {
        activeScene = sceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));

        foreach (var scene in new[] { game1Scene, game2Scene })
        {
            bool isActive = (scene == activeScene);
            Scene unityScene = SceneManager.GetSceneByName(scene);

            if (unityScene.IsValid())
            {
                foreach (GameObject go in unityScene.GetRootGameObjects())
                {
                    go.SetActive(isActive);

                    // Enable/disable all scripts so inactive game doesn't run
                    foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true))
                    {
                        mb.enabled = isActive;
                    }
                }
            }
        }

        Debug.Log("Active scene is now: " + activeScene);
    }
}
