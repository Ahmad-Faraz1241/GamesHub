using UnityEngine;

public class Bubble2 : MonoBehaviour
{
    public BubbleType type;
    private Rigidbody2D rb;
    private bool isSnapped = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
{
    if (isSnapped) return;

    if (collision.gameObject.CompareTag("GridBubble") || 
        collision.gameObject.CompareTag("Wall") || 
        collision.gameObject.CompareTag("Ceiling"))
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

    BubbleGridManager2.Instance.RegisterBubble(gridPos, this);
    BubbleGridManager2.Instance.CheckMatchesFrom(gridPos);

    isSnapped = true;
    gameObject.tag = "GridBubble";
}
    public void StartFallingAndDestroy()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false; // allow physics
            rb.gravityScale = 1f;   // enable gravity
        }

        // Destroy after 3 seconds so it has time to fall
        Destroy(gameObject, 3f);
    }

}