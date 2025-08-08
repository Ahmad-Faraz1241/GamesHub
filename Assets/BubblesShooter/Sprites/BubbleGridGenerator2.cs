


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generates a staggered grid using the manager's math so snapping aligns perfectly.
/// </summary>
public class BubbleGridGenerator2 : MonoBehaviour
{
    public GameObject[] bubblePrefabs;
    public int rows = 5;
    public int columns = 8;
    public bool clearInitialMatches = false;

    private void Start()
    {
        if (BubbleGridManager2.Instance == null)
        {
            Debug.LogError("[BubbleGridGenerator2] No BubbleGridManager2 found in scene.");
            return;
        }

        GenerateGrid();

        if (clearInitialMatches)
            StartCoroutine(ClearInitialMatchesNextFrame());
    }

    private void GenerateGrid()
    {
        if (bubblePrefabs == null || bubblePrefabs.Length == 0)
        {
            Debug.LogWarning("[BubbleGridGenerator2] No bubble prefabs assigned.");
            return;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector2Int gridPos = new Vector2Int(c, r);
                Vector3 spawnWorld = BubbleGridManager2.Instance.GetWorldPositionFromGrid(gridPos);

                int rand = Random.Range(0, bubblePrefabs.Length);
                GameObject prefab = bubblePrefabs[rand];
                GameObject go = Instantiate(prefab, spawnWorld, Quaternion.identity, transform);

                Bubble2 b = go.GetComponent<Bubble2>();
                if (b != null)
                    b.type = (BubbleType)rand;

                go.tag = "GridBubble";

                var col = go.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                BubbleGridManager2.Instance.RegisterBubble(gridPos, b);
            }
        }

        Debug.Log("[BubbleGridGenerator2] Grid generated. rows=" + rows + " cols=" + columns);
    }

    private IEnumerator ClearInitialMatchesNextFrame()
    {
        yield return null; // let registrations settle

        var positions = BubbleGridManager2.Instance.GetAllRegisteredPositions();
        foreach (var pos in positions)
        {
            BubbleGridManager2.Instance.CheckMatchesFrom(pos);
            yield return null; // avoid big hitch
        }
    }
}