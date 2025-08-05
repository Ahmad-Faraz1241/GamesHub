using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    private Vector2 startTouch;
    private Vector2 swipeDelta;
    private bool isSwiping = false;

    public bool SwipeLeft { get; private set; }
    public bool SwipeRight { get; private set; }
    public bool SwipeUp { get; private set; }
    public bool SwipeDown { get; private set; }  // ✅ NEW

    [SerializeField]
    private float swipeThreshold = 30f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        SwipeLeft = SwipeRight = SwipeUp = SwipeDown = false;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Keyboard input for testing
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwipeLeft = true;
        if (Input.GetKeyDown(KeyCode.RightArrow)) SwipeRight = true;
        if (Input.GetKeyDown(KeyCode.UpArrow)) SwipeUp = true;
        if (Input.GetKeyDown(KeyCode.DownArrow)) SwipeDown = true;  // ✅ NEW
#endif

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouch = touch.position;
                    isSwiping = true;
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isSwiping)
                    {
                        swipeDelta = touch.position - startTouch;

                        if (swipeDelta.magnitude > swipeThreshold)
                        {
                            float x = swipeDelta.x;
                            float y = swipeDelta.y;

                            if (Mathf.Abs(x) > Mathf.Abs(y))
                            {
                                SwipeRight = x > 0;
                                SwipeLeft = x < 0;
                            }
                            else
                            {
                                SwipeUp = y > 0;
                                SwipeDown = y < 0;  // ✅ NEW
                            }

                            isSwiping = false;
                            startTouch = Vector2.zero;
                            swipeDelta = Vector2.zero;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isSwiping = false;
                    startTouch = Vector2.zero;
                    swipeDelta = Vector2.zero;
                    break;
            }
        }
    }
}
