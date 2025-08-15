
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGridManager2 : MonoBehaviour
{
    public static BubbleGridManager2 Instance;

    [Header("Grid Settings")]
    public float cellWidth = 1f;
    public float cellHeight = 0.88f;
    public Vector2 gridOrigin = new Vector2(-3.5f, 4.5f);

    [Header("Match Settings")]
    [Tooltip("Delay (in seconds) before popping matched bubbles.")]
    public float popDelay = 0.2f;

    private Dictionary<Vector2Int, Bubble2> grid = new Dictionary<Vector2Int, Bubble2>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterBubble(Vector2Int gridPos, Bubble2 bubble)
    {
        if (bubble == null)
        {
            return;
        }

        if (!IsValidBubbleType(bubble.type))
        {

        }

        grid[gridPos] = bubble;
    }

    private bool IsValidBubbleType(BubbleType type)
    {
        int typeValue = (int)type;
        return typeValue >= 0 && typeValue <= 2;
    }

    public void ValidateGridTypes()
    {
        foreach (var kvp in grid)
        {
            if (kvp.Value != null)
            {
                IsValidBubbleType(kvp.Value.type);
            }
        }
    }

    public void DisplayGridState()
    {
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var pos in grid.Keys)
        {
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

        if (grid.Count == 0) return;

        for (int y = maxY; y >= minY; y--)
        {
            string row = "";
            for (int x = minX; x <= maxX; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (grid.ContainsKey(pos) && grid[pos] != null)
                {
                    string typeSymbol = GetTypeSymbol(grid[pos].type);
                    row += $"[{typeSymbol}]";
                }
                else
                {
                    row += "[ ]";
                }
            }
        }
    }

    private string GetTypeSymbol(BubbleType type)
    {
        switch (type)
        {
            case BubbleType.Red: return "R";
            case BubbleType.Green: return "G";
            case BubbleType.Blue: return "B";
            default: return "?";
        }
    }

    public void RemoveBubble(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos))
        {
            grid.Remove(gridPos);
        }
    }

    public bool IsOccupied(Vector2Int gridPos)
    {
        return grid.ContainsKey(gridPos);
    }

    public Bubble2 GetBubble(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos))
        {
            return grid[gridPos];
        }
        return null;
    }

    public List<Vector2Int> GetAllRegisteredPositions()
    {
        return new List<Vector2Int>(grid.Keys);
    }

    public Vector2Int GetNearestGridPosition(Vector3 worldPos)
    {
        Vector3 local = worldPos - (Vector3)gridOrigin;
        int y = Mathf.RoundToInt(-local.y / cellHeight);
        float xOffset = (y % 2 != 0) ? cellWidth / 2f : 0f;
        int x = Mathf.RoundToInt((local.x - xOffset) / cellWidth);
        Vector2Int gridPos = new Vector2Int(x, y);
        return gridPos;
    }

    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPos)
    {
        float xOffset = (gridPos.y % 2 != 0) ? cellWidth / 2f : 0f;
        Vector3 worldPos = new Vector3(
            gridOrigin.x + gridPos.x * cellWidth + xOffset,
            gridOrigin.y - gridPos.y * cellHeight,
            0f
        );
        return worldPos;
    }

    public Vector2Int[] GetHexNeighbors(Vector2Int pos)
    {
        return (pos.y % 2 == 0)
            ? new Vector2Int[] {
                pos + new Vector2Int(-1, 0), pos + new Vector2Int(1, 0),
                pos + new Vector2Int(0, -1), pos + new Vector2Int(0, 1),
                pos + new Vector2Int(-1, -1), pos + new Vector2Int(-1, 1)
            }
            : new Vector2Int[] {
                pos + new Vector2Int(-1, 0), pos + new Vector2Int(1, 0),
                pos + new Vector2Int(0, -1), pos + new Vector2Int(0, 1),
                pos + new Vector2Int(1, -1), pos + new Vector2Int(1, 1)
            };
    }

    public void CheckMatchesFrom(Vector2Int startPos)
    {
        if (!grid.ContainsKey(startPos) || grid[startPos] == null)
        {
            return;
        }

        BubbleType targetType = grid[startPos].type;

        var connected = new List<Vector2Int>();
        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            connected.Add(current);

            foreach (var neighbor in GetHexNeighbors(current))
            {
                if (!visited.Contains(neighbor) && grid.ContainsKey(neighbor) && grid[neighbor] != null)
                {
                    if (grid[neighbor].type == targetType)
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }
        }

        if (connected.Count >= 3)
        {
            StartCoroutine(PopBubblesWithDelay(connected, popDelay));
        }
    }

    private IEnumerator PopBubblesWithDelay(List<Vector2Int> connected, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (var pos in connected)
        {
            if (grid.ContainsKey(pos) && grid[pos] != null)
            {
                StartCoroutine(SquashAndPop(grid[pos]));
                grid.Remove(pos);
            }
        }
        RemoveFloatingBubbles();
    }

    private IEnumerator SquashAndPop(Bubble2 bubble)
    {
        Transform t = bubble.transform;
        Vector3 originalScale = t.localScale;
        Vector3 enlargedScale = originalScale * 1.2f;


        float elapsed = 0f;
        float upDuration = 0.05f;
        while (elapsed < upDuration)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(originalScale, enlargedScale, elapsed / upDuration);
            yield return null;
        }

        elapsed = 0f;
        float downDuration = 0.1f;
        while (elapsed < downDuration)
        {
            elapsed += Time.deltaTime;
            t.localScale = Vector3.Lerp(enlargedScale, Vector3.zero, elapsed / downDuration);
            yield return null;
        }

        Destroy(bubble.gameObject);
    }

    void DebugAllBubbles()
    {
        foreach (var kvp in grid)
        {

        }
    }

    public void RemoveFloatingBubbles()
    {
        var connectedToTop = new HashSet<Vector2Int>();
        var q = new Queue<Vector2Int>();

        foreach (var pos in grid.Keys)
        {
            if (pos.y == 0)
            {
                connectedToTop.Add(pos);
                q.Enqueue(pos);
            }
        }

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            foreach (var n in GetHexNeighbors(cur))
            {
                if (grid.ContainsKey(n) && !connectedToTop.Contains(n))
                {
                    connectedToTop.Add(n);
                    q.Enqueue(n);
                }
            }
        }

        var toRemove = new List<Vector2Int>();
        foreach (var pos in grid.Keys)
        {
            if (!connectedToTop.Contains(pos))
                toRemove.Add(pos);
        }

        foreach (var pos in toRemove)
        {
            if (grid[pos] != null)
            {
                Rigidbody2D rb = grid[pos].GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.gravityScale = 2f;
                    rb.velocity = Vector2.zero;
                }
                grid[pos].StartFallingAndDestroy();
            }
            grid.Remove(pos);
        }
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        Gizmos.color = Color.green;
        foreach (var pos in grid.Keys)
        {
            Vector3 worldPos = GetWorldPositionFromGrid(pos);
            Gizmos.DrawWireSphere(worldPos, 0.3f);
        }
    }
}
