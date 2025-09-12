using UnityEngine;
using UnityEngine.SceneManagement;


public class TwoFingerSwipeSceneSwitcher : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 100f;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    private static int nextSceneIndex;

    void Start()
    {
        CacheNextSceneIndex();
    }

    void Update()
    {
#if UNITY_EDITOR
        
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

    void DetectSwipe()
    {
        float swipeDistance = Vector2.Distance(startTouchPos, endTouchPos);

        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDir = endTouchPos - startTouchPos;

            
            if (Mathf.Abs(swipeDir.y) > Mathf.Abs(swipeDir.x))
            {
                ActivateScene();
            }
        }
    }

    void CacheNextSceneIndex()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount <= 1)
        {
            nextSceneIndex = currentSceneIndex;
            return;
        }

        nextSceneIndex = (currentSceneIndex + 1) % sceneCount;
    }

    void ActivateScene()
    {
        SceneManager.LoadScene(nextSceneIndex);
    }
}
