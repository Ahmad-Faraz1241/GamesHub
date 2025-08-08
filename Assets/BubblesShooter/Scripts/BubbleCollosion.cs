using UnityEngine;

public class BubbleCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.layer.Equals(LayerMask.NameToLayer("GridLayer"))) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        gameObject.tag = "Bubble";
        gameObject.layer = LayerMask.NameToLayer("GridLayer");

        Bubble bubble = GetComponent<Bubble>();
        if (bubble == null)
        {
            Debug.LogError("❌ Bubble component missing on: " + gameObject.name);
            return;
        }

        Vector2Int gridPos = BubbleGridManager.Instance.GetNearestGridPosition(transform.position);
        transform.position = BubbleGridManager.Instance.GetWorldPositionFromGrid(gridPos);

        bubble.gridPos = gridPos;
        BubbleGridManager.Instance.RegisterBubble(gridPos, bubble);

        MatchChecker matchChecker = FindObjectOfType<MatchChecker>();
        if (matchChecker)
        {
            matchChecker.CheckAndPopMatches(bubble);
        }
    }
}