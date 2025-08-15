
using UnityEngine;

public class Bubble2 : MonoBehaviour
{
    public BubbleType type;
    private Rigidbody2D rb;
    private bool isSnapped = false;

    [Header("Audio Settings")]
    public AudioClip snapSound;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isSnapped) return;

        if (collision.gameObject.CompareTag("GridBubble") ||
            collision.gameObject.CompareTag("Wall"))

        {
            SnapToGrid();
        }
    }

    void SnapToGrid()
    {
        Vector2Int gridPos = BubbleGridManager2.Instance.GetNearestGridPosition(transform.position);
        Vector3 snapPos = BubbleGridManager2.Instance.GetWorldPositionFromGrid(gridPos);

        transform.position = snapPos;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;


        PlaySnapSound();

        BubbleGridManager2.Instance.RegisterBubble(gridPos, this);


        BubbleGridManager2.Instance.CheckMatchesFrom(gridPos);

        isSnapped = true;
        gameObject.tag = "GridBubble";
    }

    void PlaySnapSound()
    {
        if (snapSound != null)
        {
            audioSource.PlayOneShot(snapSound);
        }
    }

    public void StartFallingAndDestroy()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 1f;
        }


        Destroy(gameObject, 0f);
    }
}
