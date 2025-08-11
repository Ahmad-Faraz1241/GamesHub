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
    [Tooltip("Width of the aiming line")]
    public float lineStartWidth = 0.05f;
    public float lineEndWidth = 0.05f;
    [Tooltip("Max length of the aiming line in world units")]
    public float maxLineLength = 5f;

    private GameObject currentBubble;
    private bool isDragging = false;

    private LineRenderer aimLine;

    // Audio settings
    public AudioClip shootSound;  // Assign sound effect here
    private AudioSource audioSource;

    void Start()
    {
        // Initialize the shooting bubble and line renderer
        CreateNewBubble();

        aimLine = gameObject.AddComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.enabled = false;

        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = lineStartColor;
        aimLine.endColor = lineEndColor;
        aimLine.startWidth = lineStartWidth;
        aimLine.endWidth = lineEndWidth;

        aimLine.useWorldSpace = true;
        aimLine.sortingOrder = 10;

        // Set up the AudioSource component
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (currentBubble == null) return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    aimLine.enabled = true;
                    UpdateAimLine(touchPos);
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                        UpdateAimLine(touchPos);
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDragging)
                    {
                        Vector3 dir = GetShootDirection(touchPos);
                        FireCurrentBubble(dir);
                        isDragging = false;
                        aimLine.enabled = false;
                    }
                    break;
            }
        }
        else
        {
            // Mouse controls for testing in editor
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
        }
    }

    void UpdateAimLine(Vector2 dragEndScreenPos)
    {
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - shootPoint.position.z);
        Vector3 dragEndWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(dragEndScreenPos.x, dragEndScreenPos.y, zDistance));

        Vector3 dir = (dragEndWorldPos - shootPoint.position).normalized;
        if (dir == Vector3.zero)
            dir = Vector3.up;  // fallback direction

        Vector3 lineEndPos = shootPoint.position + dir * maxLineLength;

        aimLine.SetPosition(0, shootPoint.position);
        aimLine.SetPosition(1, lineEndPos);
    }

    Vector3 GetShootDirection(Vector2 dragEndScreenPos)
    {
        float zDistance = Mathf.Abs(Camera.main.transform.position.z - shootPoint.position.z);
        Vector3 dragEndWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(dragEndScreenPos.x, dragEndScreenPos.y, zDistance));
        Vector3 dir = (dragEndWorldPos - shootPoint.position).normalized;
        if (dir == Vector3.zero)
            dir = Vector3.up;
        return dir;
    }

    private void FireCurrentBubble(Vector3 direction)
    {
        currentBubble.transform.position = shootPoint.position;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.velocity = direction * shootSpeed;

        currentBubble.tag = "ShooterBubble";

        // Play the shooting sound effect
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);  // Play the sound effect
        }

        currentBubble = null;

        // Start the Coroutine to spawn the next bubble with a delay
        StartCoroutine(SpawnBubbleWithDelay());
    }

    private IEnumerator SpawnBubbleWithDelay()
    {
        // Wait for the specified respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Create a new bubble after the delay
        CreateNewBubble();
    }

    void CreateNewBubble()
    {
        if (bubblePrefabs.Length == 0) return;

        int rand = Random.Range(0, bubblePrefabs.Length);
        GameObject bubbleObj = Instantiate(bubblePrefabs[rand], shootPoint.position, Quaternion.identity);

        Bubble2 b = bubbleObj.GetComponent<Bubble2>();
        b.type = (BubbleType)rand;

        bubbleObj.tag = "ShooterBubble";
        currentBubble = bubbleObj;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }
}
