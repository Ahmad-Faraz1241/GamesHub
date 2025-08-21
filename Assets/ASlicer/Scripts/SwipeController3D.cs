using UnityEngine;
using System.Collections.Generic;

public class SwipeController3D : MonoBehaviour
{
    public float minSwipeDistance = 0.05f;
    public float swipeRadius = 0.2f;
    public LayerMask sliceMask;

    public int maxPositions = 15;
    public float trailWidth = 0.1f;

    private Camera cam;
    private Vector3 lastPos;
    private LineRenderer trail;
    private Queue<Vector3> trailPositions = new Queue<Vector3>();

    void Start()
    {
        cam = Camera.main;

        GameObject trailObj = new GameObject("SwipeTrail");
        trailObj.transform.parent = transform;

        trail = trailObj.AddComponent<LineRenderer>();
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = Color.white;
        trail.endColor = new Color(1f, 1f, 1f, 0.2f);
        trail.startWidth = trailWidth;
        trail.endWidth = 0.02f;
        trail.positionCount = 0;
        trail.numCapVertices = 5;
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
        if (Input.touchCount > 0)
        {
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
    }

    void ProcessSwipe(Vector3 newPos)
    {
        if (Vector3.Distance(lastPos, newPos) > minSwipeDistance)
        {
            trailPositions.Enqueue(newPos);
            if (trailPositions.Count > maxPositions)
                trailPositions.Dequeue();

            Vector3 dir = newPos - lastPos;
            float dist = dir.magnitude;

            if (Physics.SphereCast(lastPos, swipeRadius, dir.normalized, out RaycastHit hit, dist, sliceMask))
            {
                var slicer = hit.collider.GetComponent<FruitSlicer3D>();
                if (slicer != null)
                    slicer.Slice(dir.normalized);
            }

            Debug.DrawLine(lastPos, newPos, Color.red, 0.2f);
            lastPos = newPos;
        }
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
