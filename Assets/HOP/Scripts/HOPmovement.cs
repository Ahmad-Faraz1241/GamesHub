using UnityEngine;
using System.Collections;

public class HOPmovement : MonoBehaviour
{
    private Rigidbody rb;
    private bool canHop = true;
    public static bool isGameActive = false;

    [Header("Visuals")]
    public Transform ballVisual;

    [Header("Audios")]
    public AudioSource audioSource;
    public AudioClip hopSound;
    public AudioClip fakeTileSound;
    public AudioClip flightSound;   // NEW: sound during mid-air
    public AudioClip fallingSound;  // NEW: sound when falling

    [Header("Forces")]
    public float hopForce = 7f;
    public float forwardForce = 2.3f;
    public float horizontalSpeed = 5f;

    [Header("Snapping")]
    public float zSpacing = 3.2f;
    public float snapThreshold = 0.05f;

    [Header("Mobile Drag")]
    public float dragSensitivity = 0.25f;
    private float screenWidth;
    private float dragStartX;
    private bool isDragging = false;

    private float targetHorizontalInput = 0f;
    private float smoothedHorizontalInput = 0f;

    [Header("Difficulty Scaling")]
    public float minDragSensitivity = 0.12f;
    public int scoreThresholdForDifficulty = 15;
    public float dragDecreaseStep = 0.02f;
    private int lastDifficultyAppliedAt = 0;

    [Header("Jelly Animation")]
    public float squishAmount = 0.2f;
    public float squishDuration = 0.1f;

    [Header("FallThreshHold")]
    public float fallThreshHold = -2f;

    // NEW: control so falling sound plays only once
    private bool fallingSoundPlayed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        screenWidth = Screen.width;
    }

    void Update()
    {
        if (!isGameActive) return;

        targetHorizontalInput = 0f;

#if UNITY_EDITOR || UNITY_STANDALONE
        targetHorizontalInput = Input.GetAxisRaw("Horizontal");
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                dragStartX = touch.position.x;
            }
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
            {
                float dragDelta = touch.position.x - dragStartX;
                targetHorizontalInput = Mathf.Clamp(dragDelta / (screenWidth * dragSensitivity), -1f, 1f);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                targetHorizontalInput = 0f;
            }
        }
#endif

        // Play falling sound when ball crosses Y = 0 for the first time
        if (!fallingSoundPlayed && transform.position.y < 0f)
        {
            if (fallingSound != null)
                audioSource.PlayOneShot(fallingSound);

            fallingSoundPlayed = true;
        }

        // Game over if below threshold
        if (transform.position.y < fallThreshHold)
        {
            isGameActive = false;
            HOPUIManager.Instance.OnGameOver();
        }
    }

    void FixedUpdate()
    {
        if (!isGameActive) return;

        smoothedHorizontalInput = Mathf.Lerp(smoothedHorizontalInput, targetHorizontalInput, Time.fixedDeltaTime * 10f);
        Vector3 velocity = rb.velocity;
        velocity.x = smoothedHorizontalInput * horizontalSpeed;
        rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z);

        TrySoftSnapZ();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!canHop || !isGameActive) return;

        if (collision.gameObject.CompareTag("FakeTile"))
        {
            if (fakeTileSound != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(fakeTileSound);
                audioSource.pitch = 1f;
            }

            // Only play falling sound if it hasn't been played yet
            if (!fallingSoundPlayed && fallingSound != null)
            {
                audioSource.PlayOneShot(fallingSound);
                fallingSoundPlayed = true;
            }

            canHop = false;

            // Make all nearby fake tiles fall
            Collider[] hitTiles = Physics.OverlapSphere(collision.transform.position, 1.5f);
            foreach (Collider col in hitTiles)
            {
                if (col.CompareTag("FakeTile"))
                {
                    Rigidbody rbTile = col.GetComponent<Rigidbody>();
                    if (rbTile == null) rbTile = col.gameObject.AddComponent<Rigidbody>();
                    rbTile.mass = 0.5f;
                    rbTile.isKinematic = false;
                    rbTile.useGravity = true;
                }
            }

            isGameActive = false;
            rb.velocity = Vector3.zero;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;

            StartCoroutine(TriggerGameOverAfterDelay(1.5f));
            return;
        }

        if (collision.gameObject.CompareTag("Tile"))
        {
            canHop = false;
            fallingSoundPlayed = false; // Reset so it can play next time ball falls

            if (hopSound != null)
                audioSource.PlayOneShot(hopSound);

            // Play mid-air flight sound
            if (flightSound != null)
                audioSource.PlayOneShot(flightSound);

            Vector3 vel = rb.velocity;
            vel.y = 0f;
            vel.z = 0f;
            rb.velocity = vel;

            rb.AddForce(Vector3.up * hopForce + Vector3.forward * forwardForce, ForceMode.Impulse);

            HOPScoreManager.Instance.AddPoint();

            StartCoroutine(SquishJellyEffect(ballVisual));
            StartCoroutine(SquishJellyEffect(collision.transform));

            Invoke(nameof(ResetHop), 0.15f);
        }
    }

    private IEnumerator TriggerGameOverAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HOPUIManager.Instance.OnGameOver();
    }

    void TrySoftSnapZ()
    {
        if (rb.velocity.y < 0.05f && rb.velocity.y > -0.05f)
        {
            float snappedZ = Mathf.Round(transform.position.z / zSpacing) * zSpacing;
            if (Mathf.Abs(transform.position.z - snappedZ) > snapThreshold)
                transform.position = new Vector3(transform.position.x, transform.position.y, snappedZ);
        }
    }

    void LateUpdate()
    {
        if (!isGameActive) return;
        UpdateDragDifficulty();
    }

    void UpdateDragDifficulty()
    {
        int score = HOPScoreManager.Instance.GetScore();
        if (score >= lastDifficultyAppliedAt + scoreThresholdForDifficulty)
        {
            lastDifficultyAppliedAt = score;
            dragSensitivity = Mathf.Max(minDragSensitivity, dragSensitivity - dragDecreaseStep);
        }
    }

    void ResetHop() => canHop = true;

    IEnumerator SquishJellyEffect(Transform obj)
    {
        Vector3 originalScale = obj.localScale;
        Vector3 squishedScale = new Vector3(originalScale.x + squishAmount, originalScale.y - squishAmount, originalScale.z + squishAmount);
        obj.localScale = squishedScale;
        yield return new WaitForSeconds(squishDuration);
        obj.localScale = originalScale;
    }
}
