using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    public int matchThreshold = 3;

    public void CheckAndPopMatches(Bubble startBubble)
    {
        List<Bubble> bubblesToPop = new List<Bubble>();

        foreach (Vector2Int direction in HexDirections())
        {
            List<Bubble> line = GetBubblesInLine(startBubble.gridPos, direction, startBubble.bubbleType);
            if (line.Count + 1 >= matchThreshold)
            {
                bubblesToPop.AddRange(line);
            }
        }

        if (bubblesToPop.Count > 0)
        {
            bubblesToPop.Add(startBubble);
            foreach (Bubble bubble in bubblesToPop)
            {
                BubbleGridManager.Instance.UnregisterBubble(bubble.gridPos);
                Destroy(bubble.gameObject);
            }

            // Check for floating bubbles
            BubbleGridManager.Instance.RemoveFloatingBubbles();
        }
    }

    private List<Bubble> GetBubblesInLine(Vector2Int start, Vector2Int direction, string type)
    {
        List<Bubble> line = new List<Bubble>();

        // Forward direction
        Vector2Int pos = start + direction;
        while (true)
        {
            Bubble bubble = BubbleGridManager.Instance.GetBubble(pos);
            if (bubble != null && bubble.bubbleType == type)
            {
                line.Add(bubble);
                pos += direction;
            }
            else break;
        }

        // Reverse direction
        pos = start - direction;
        while (true)
        {
            Bubble bubble = BubbleGridManager.Instance.GetBubble(pos);
            if (bubble != null && bubble.bubbleType == type)
            {
                line.Add(bubble);
                pos -= direction;
            }
            else break;
        }

        return line;
    }

    private List<Vector2Int> HexDirections()
    {
        return new List<Vector2Int>
        {
            new Vector2Int(1, 0),  // Right
            new Vector2Int(-1, 0), // Left
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(1, -1), // Down-right
            new Vector2Int(-1, 1), // Up-left
        };
    }
}