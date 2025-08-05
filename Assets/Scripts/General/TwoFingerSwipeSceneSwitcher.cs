using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TwoFingerSwipeSceneSwitcher : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 100f;
    public float overlayDuration = 0.1f; // Time to keep overlay before revealing

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private bool isSwitching = false;
    private string otherScene;
    private Image overlayImage;

    void Start()
    {
        // Cache the other scene name
        string currentScene = SceneManager.GetActiveScene().name;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (sceneName != currentScene)
            {
                otherScene = sceneName;
                break;
            }
        }

        CreateOverlay();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!isSwitching)
                StartCoroutine(SwitchToOtherScene());
        }
#else
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
        if (isSwitching) return;

        float swipeDistance = Vector2.Distance(startTouchPos, endTouchPos);

        if (swipeDistance >= minSwipeDistance)
        {
            Vector2 swipeDir = endTouchPos - startTouchPos;

            if (Mathf.Abs(swipeDir.y) > Mathf.Abs(swipeDir.x))
            {
                StartCoroutine(SwitchToOtherScene());
            }
        }
    }

    IEnumerator SwitchToOtherScene()
    {
        if (string.IsNullOrEmpty(otherScene)) yield break;
        isSwitching = true;

        // Show black overlay instantly
        overlayImage.color = Color.black;
        overlayImage.enabled = true;

        // Wait short duration to hide load
        yield return new WaitForSeconds(overlayDuration);

        // Load other scene in background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(otherScene, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = true;
        while (!asyncLoad.isDone)
            yield return null;
    }

    void CreateOverlay()
    {
        GameObject overlayObj = new GameObject("BlackOverlay");
        Canvas canvas = overlayObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayObj.AddComponent<CanvasScaler>();
        overlayObj.AddComponent<GraphicRaycaster>();

        overlayImage = overlayObj.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0); // start transparent

        RectTransform rt = overlayImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        DontDestroyOnLoad(overlayObj); // persist between scenes
        overlayImage.enabled = false;
    }
}
