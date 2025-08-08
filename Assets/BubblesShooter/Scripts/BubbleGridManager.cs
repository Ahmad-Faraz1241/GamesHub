using System.Collections.Generic;
using UnityEngine;

public class BubbleGridManager : MonoBehaviour
{
    public static BubbleGridManager Instance;

    private Dictionary<Vector2Int, Bubble> grid = new Dictionary<Vector2Int, Bubble>();
    public int columns = 7;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterBubble(Vector2Int gridPos, Bubble bubble)
    {
        grid[gridPos] = bubble;
    }

    public void UnregisterBubble(Vector2Int gridPos)
    {
        grid.Remove(gridPos);
    }

    public Bubble GetBubble(Vector2Int gridPos)
    {
        grid.TryGetValue(gridPos, out Bubble bubble);
        return bubble;
    }

    public Vector2Int GetNearestGridPosition(Vector3 worldPos)
    {
        float x = worldPos.x / 1.28f;
        float y = -worldPos.y / (1.28f * 0.866f);
        int row = Mathf.RoundToInt(y);
        int col = Mathf.RoundToInt(x - ((row % 2 == 1) ? 0.5f : 0f));
        return new Vector2Int(col, row);
    }

    public Vector3 GetWorldPositionFromGrid(Vector2Int gridPos)
    {
        float x = gridPos.x * 1.28f + ((gridPos.y % 2 == 1) ? 0.64f : 0f);
        float y = -gridPos.y * 1.28f * 0.866f;
        return new Vector3(x, y, 0);
    }

    public List<Vector2Int> GetAllGridPositions()
    {
        return new List<Vector2Int>(grid.Keys);
    }
    public void RemoveFloatingBubbles()
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Vector2Int> toVisit = new Queue<Vector2Int>();

        // Start from all bubbles in the top row
        foreach (var kvp in grid)
        {
            if (kvp.Key.y == 0)
            {
                visited.Add(kvp.Key);
                toVisit.Enqueue(kvp.Key);
            }
        }

        // BFS to find all connected bubbles
        while (toVisit.Count > 0)
        {
            Vector2Int current = toVisit.Dequeue();
            foreach (Vector2Int neighbor in GetHexNeighbors(current))
            {
                if (!visited.Contains(neighbor) && grid.ContainsKey(neighbor))
                {
                    visited.Add(neighbor);
                    toVisit.Enqueue(neighbor);
                }
            }
        }

        // Remove all bubbles not connected to top
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in grid)
        {
            if (!visited.Contains(kvp.Key))
            {
                Destroy(kvp.Value.gameObject);
                toRemove.Add(kvp.Key);
            }
        }

        foreach (var key in toRemove)
        {
            grid.Remove(key);
        }
    }

    private List<Vector2Int> GetHexNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        bool isOdd = pos.y % 2 == 1;
        neighbors.Add(pos + new Vector2Int(0, 1));  // Up
        neighbors.Add(pos + new Vector2Int(0, -1)); // Down
        neighbors.Add(pos + new Vector2Int(1, 0));  // Right
        neighbors.Add(pos + new Vector2Int(-1, 0)); // Left

        if (isOdd)
        {
            neighbors.Add(pos + new Vector2Int(1, 1));   // Up-right
            neighbors.Add(pos + new Vector2Int(1, -1));  // Down-right
        }
        else
        {
            neighbors.Add(pos + new Vector2Int(-1, 1));  // Up-left
            neighbors.Add(pos + new Vector2Int(-1, -1)); // Down-left
        }

        return neighbors;
    }

}