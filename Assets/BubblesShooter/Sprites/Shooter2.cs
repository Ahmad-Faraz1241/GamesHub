using UnityEngine;
using System.Collections;

public class Shooter2 : MonoBehaviour
{
    public GameObject[] bubblePrefabs;
    public Transform shootPoint;
    public float shootSpeed = 10f;
    public float respawnDelay = 0.4f;

    [Header("Aim Line Settings")]
    public Color lineStartColor = Color.cyan;
    public Color lineEndColor = Color.cyan;
    public float lineStartWidth = 0.05f;
    public float lineEndWidth = 0.05f;
    public float maxLineLength = 5f;

    [Header("Wall Settings")]
    public LayerMask wallLayer; // Assign "Wall" layer in Inspector

    private GameObject currentBubble;
    private bool isDragging = false;
    private LineRenderer aimLine;
    private Rigidbody2D shotBubbleRb;
    private Vector2 shotDirection;

    private int bubbleCounter = 0;

    public AudioClip shootSound;
    private AudioSource audioSource;

    void Start()
    {
        CreateNewBubble();

        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.enabled = false;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = lineStartColor;
        aimLine.endColor = lineEndColor;
        aimLine.startWidth = lineStartWidth;
        aimLine.endWidth = lineEndWidth;
        aimLine.useWorldSpace = true;
        aimLine.sortingOrder = 10;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (currentBubble == null) return;

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            aimLine.enabled = true;
            UpdateAimLine(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateAimLine(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 dir = GetShootDirection(Input.mousePosition);
            FireCurrentBubble(dir);
            isDragging = false;
            aimLine.enabled = false;
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    aimLine.enabled = true;
                    UpdateAimLine(touch.position);
                    break;
                case TouchPhase.Moved:
                    if (isDragging) UpdateAimLine(touch.position);
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDragging)
                    {
                        Vector3 dir = GetShootDirection(touch.position);
                        FireCurrentBubble(dir);
                        isDragging = false;
                        aimLine.enabled = false;
                    }
                    break;
            }
        }
#endif
    }

    void FixedUpdate()
    {
        if (shotBubbleRb != null)
        {
            float distance = shootSpeed * Time.fixedDeltaTime + 0.05f;
            float radius = shotBubbleRb.GetComponent<CircleCollider2D>() != null
                ? shotBubbleRb.GetComponent<CircleCollider2D>().radius * Mathf.Max(shotBubbleRb.transform.localScale.x, shotBubbleRb.transform.localScale.y)
                : 0.25f;

            RaycastHit2D hit = Physics2D.CircleCast(shotBubbleRb.position, radius, shotDirection, distance, wallLayer);

            if (hit.collider != null)
            {
                shotDirection.x = -shotDirection.x;
                shotBubbleRb.velocity = shotDirection * shootSpeed;
                shotBubbleRb.position = hit.point + hit.normal * radius;
            }
        }
    }

    void UpdateAimLine(Vector2 dragEndScreenPos)
    {
        Vector3 dir = GetShootDirection(dragEndScreenPos);
        DrawAimLine(shootPoint.position, dir);
    }

    void DrawAimLine(Vector2 startPos, Vector2 dir)
    {
        aimLine.positionCount = 1;
        aimLine.SetPosition(0, startPos);

        Vector2 currentPos = startPos;
        Vector2 currentDir = dir;
        int reflections = 0;
        float maxDistance = maxLineLength;

        while (reflections <= 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentPos, currentDir, maxDistance, wallLayer);

            if (hit.collider != null)
            {
                aimLine.positionCount++;
                aimLine.SetPosition(aimLine.positionCount - 1, hit.point);

                currentDir = Vector2.Reflect(currentDir, hit.normal);
                currentPos = hit.point + currentDir * 0.01f;
                reflections++;
            }
            else
            {
                aimLine.positionCount++;
                aimLine.SetPosition(aimLine.positionCount - 1, currentPos + currentDir * maxDistance);
                break;
            }
        }
    }

    Vector3 GetShootDirection(Vector2 dragEndScreenPos)
    {
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - shootPoint.position.z);
        Vector3 dragEndWorldPos = Camera.main.ScreenToWorldPoint(
            new Vector3(dragEndScreenPos.x, dragEndScreenPos.y, zDistance)
        );
        Vector3 dir = (dragEndWorldPos - shootPoint.position).normalized;
        if (dir == Vector3.zero) dir = Vector3.up;
        return dir;
    }

    private void FireCurrentBubble(Vector3 direction)
    {
        currentBubble.transform.position = shootPoint.position;

        shotBubbleRb = currentBubble.GetComponent<Rigidbody2D>();
        shotBubbleRb.isKinematic = false;
        shotDirection = direction.normalized;
        shotBubbleRb.velocity = shotDirection * shootSpeed;

        currentBubble.tag = "ShooterBubble";

        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        currentBubble = null;
        StartCoroutine(SpawnBubbleWithDelay());
    }

    private IEnumerator SpawnBubbleWithDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        CreateNewBubble();
    }

    void CreateNewBubble()
    {
        if (bubblePrefabs.Length == 0) return;

        int prefabIndex = bubbleCounter % bubblePrefabs.Length;
        bubbleCounter++;

        GameObject bubbleObj = Instantiate(bubblePrefabs[prefabIndex], shootPoint.position, Quaternion.identity);
        Bubble2 b = bubbleObj.GetComponent<Bubble2>();
        if (b != null)
        {
            b.type = (BubbleType)(prefabIndex % 3);
        }

        bubbleObj.tag = "ShooterBubble";
        currentBubble = bubbleObj;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }
    }
}

