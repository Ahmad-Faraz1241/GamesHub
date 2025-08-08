using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    [Header("Shooter Settings")]
    public GameObject[] bubblePrefabs;
    public Transform shootPoint;
    public float shootSpeed = 10f;

    private GameObject currentPrefab;
    private bool isShooting = false;

    void Start()
    {
        if (bubblePrefabs.Length == 0)
        {
            Debug.LogError("❌ No shooter bubble prefabs assigned!");
            return;
        }
        LoadNextBubble();
    }

    void Update()
    {
        if (!isShooting && Input.GetMouseButtonDown(0))
        {
            ShootBubble();
        }
    }

    void ShootBubble()
    {
        if (currentPrefab == null) return;

        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = 0;

        Vector2 direction = (target - shootPoint.position).normalized;

        GameObject bubble = Instantiate(currentPrefab, shootPoint.position, Quaternion.identity);
        bubble.layer = LayerMask.NameToLayer("ShooterLayer");

        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction * shootSpeed;
        }

        isShooting = true;
        bubble.AddComponent<BubbleShotTracker>().onStopped = () =>
        {
            isShooting = false;
            LoadNextBubble();
        };
    }

    void LoadNextBubble()
    {
        currentPrefab = bubblePrefabs[Random.Range(0, bubblePrefabs.Length)];
    }
}