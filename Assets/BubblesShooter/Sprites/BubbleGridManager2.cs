using System.Collections.Generic;
using UnityEngine;

public class BubbleGridManager2 : MonoBehaviour
{
    public static BubbleGridManager2 Instance;

    [Header("Grid Settings")]
    public float cellWidth = 1f;
    public float cellHeight = 0.88f;
    public Vector2 gridOrigin = new Vector2(-3.5f, 4.5f);

    private Dictionary<Vector2Int, Bubble2> grid = new Dictionary<Vector2Int, Bubble2>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterBubble(Vector2Int gridPos, Bubble2 bubble)
    {
        if (bubble == null) return;
        grid[gridPos] = bubble;
    }

    public void RemoveBubble(Vector2Int gridPos)
    {
        if (grid.ContainsKey(gridPos)) grid.Remove(gridPos);
    }

    public bool IsOccupied(Vector2Int gridPos) => grid.ContainsKey(gridPos);

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
        return new Vector2Int(x, y);
    }

    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPos)
    {
        float xOffset = (gridPos.y % 2 != 0) ? cellWidth / 2f : 0f;
        return new Vector3(
            gridOrigin.x + gridPos.x * cellWidth + xOffset,
            gridOrigin.y - gridPos.y * cellHeight,
            0f
        );
    }

    public Vector2Int[] GetHexNeighbors(Vector2Int pos)
    {
        if (pos.y % 2 == 0)
        {
            return new Vector2Int[] {
                pos + new Vector2Int(-1, 0), pos + new Vector2Int(1, 0),
                pos + new Vector2Int(0, -1), pos + new Vector2Int(0, 1),
                pos + new Vector2Int(-1, -1), pos + new Vector2Int(-1, 1)
            };
        }
        else
        {
            return new Vector2Int[] {
                pos + new Vector2Int(-1, 0), pos + new Vector2Int(1, 0),
                pos + new Vector2Int(0, -1), pos + new Vector2Int(0, 1),
                pos + new Vector2Int(1, -1), pos + new Vector2Int(1, 1)
            };
        }
    }

    public void CheckMatchesFrom(Vector2Int startPos)
    {
        if (!grid.ContainsKey(startPos) || grid[startPos] == null) return;

        BubbleType targetType = grid[startPos].type;
        var connected = new List<Vector2Int>();
        var visited = new HashSet<Vector2Int>();
        var q = new Queue<Vector2Int>();

        q.Enqueue(startPos);
        visited.Add(startPos);

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (!grid.ContainsKey(cur) || grid[cur] == null) continue;
            if (grid[cur].type != targetType) continue;

            connected.Add(cur);

            foreach (var n in GetHexNeighbors(cur))
            {
                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    q.Enqueue(n);
                }
            }
        }

        if (connected.Count >= 3)
        {
            foreach (var pos in connected)
            {
                if (grid.ContainsKey(pos) && grid[pos] != null)
                    Destroy(grid[pos].gameObject);
                grid.Remove(pos);
            }
            RemoveFloatingBubbles();
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
            if (!connectedToTop.Contains(pos)) toRemove.Add(pos);
        }

        foreach (var pos in toRemove)
        {
            if (grid[pos] != null)
            {
                // Make orphan bubble fall
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
}