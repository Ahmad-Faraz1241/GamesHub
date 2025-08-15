using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
           
            return;
        }

       

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                Vector2Int gridPos = new Vector2Int(c, r);
                Vector3 spawnWorld = BubbleGridManager2.Instance.GetWorldPositionFromGrid(gridPos);

              
                int prefabIndex = (r * columns + c) % bubblePrefabs.Length;

               
                GameObject prefab = bubblePrefabs[prefabIndex];
                GameObject go = Instantiate(prefab, spawnWorld, Quaternion.identity, transform);

                Bubble2 b = go.GetComponent<Bubble2>();
                if (b != null)
                {
                   
                    b.type = (BubbleType)(prefabIndex % 3);

                    
                }
                else
                {
                    
                    Destroy(go);
                    continue;
                }

                go.tag = "GridBubble";

                var col = go.GetComponent<Collider2D>();
                if (col != null) col.enabled = true;

                BubbleGridManager2.Instance.RegisterBubble(gridPos, b);

                
            }
        }

        

        
        BubbleGridManager2.Instance.ValidateGridTypes();
    }


    private IEnumerator ClearInitialMatchesNextFrame()
    {
        yield return null; 

        var positions = BubbleGridManager2.Instance.GetAllRegisteredPositions();
        foreach (var pos in positions)
        {
            BubbleGridManager2.Instance.CheckMatchesFrom(pos);
            yield return null; 
        }
    }
}
