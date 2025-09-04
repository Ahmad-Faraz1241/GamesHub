using UnityEngine;
using System.Collections.Generic;

public class SwipeController3D : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float minSwipeDistance = 0.05f;   // Minimum movement to register a slice
    public float swipeRadius = 0.2f;         // SphereCast radius
    public LayerMask sliceMask;              // Layer(s) of slicable objects

    [Header("Trail Settings")]
    public int maxPositions = 15;            // Max positions stored for trail
    public float trailWidth = 0.1f;          // Trail start width
    public Gradient trailColor;              // Optional color gradient

    private Camera cam;
    private Vector3 lastPos;
    private LineRenderer trail;
    private Queue<Vector3> trailPositions = new Queue<Vector3>();

    void Awake()
    {
        cam = Camera.main;

        // Create trail
        GameObject trailObj = new GameObject("SwipeTrail");
        trailObj.transform.parent = transform;

        trail = trailObj.AddComponent<LineRenderer>();
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startWidth = trailWidth;
        trail.endWidth = 0.02f;
        trail.positionCount = 0;
        trail.numCapVertices = 5;

        if (trailColor != null)
            trail.colorGradient = trailColor;
        else
        {
            trail.startColor = Color.white;
            trail.endColor = new Color(1f, 1f, 1f, 0.2f);
        }
    }

    void Update()
    {
#if UNITY_EDITOR
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
        UpdateTrail();
    }

    #region Input Handling
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastPos = ScreenToWorld(Input.mousePosition);
            trailPositions.Clear();
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 newPos = ScreenToWorld(Input.mousePosition);
            ProcessSwipe(newPos);
        }

        if (Input.GetMouseButtonUp(0))
            trailPositions.Clear();
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        Vector3 touchPos = ScreenToWorld(touch.position);

        if (touch.phase == TouchPhase.Began)
        {
            lastPos = touchPos;
            trailPositions.Clear();
        }
        else if (touch.phase == TouchPhase.Moved)
            ProcessSwipe(touchPos);
        else if (touch.phase == TouchPhase.Ended)
            trailPositions.Clear();
    }
    #endregion

    void ProcessSwipe(Vector3 newPos)
    {
        if (Vector3.Distance(lastPos, newPos) < minSwipeDistance) return;

        trailPositions.Enqueue(newPos);
        if (trailPositions.Count > maxPositions)
            trailPositions.Dequeue();

        Vector3 dir = newPos - lastPos;
        float dist = dir.magnitude;

        // SphereCast to detect sliceable objects
        if (Physics.SphereCast(lastPos, swipeRadius, dir.normalized, out RaycastHit hit, dist, sliceMask))
        {
            var slicer = hit.collider.GetComponent<FruitSlicer3D>();
            if (slicer != null)
                slicer.Slice(dir.normalized);
        }

        lastPos = newPos;
    }

    void UpdateTrail()
    {
        trail.positionCount = trailPositions.Count;
        int i = 0;
        foreach (var pos in trailPositions)
        {
            trail.SetPosition(i, pos);
            i++;
        }
    }

    Vector3 ScreenToWorld(Vector2 screenPos)
    {
        return cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
    }
}
