using UnityEngine;

public class BubbleGridGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 6;
    public int columns = 7;
    public float bubbleRadius = 1.28f;
    public float horizontalSpacingMultiplier = 1.0f;
    public float verticalSpacingMultiplier = 0.866f; // √3/2 for hex grid

    [Header("Bubble Prefabs")]
    public GameObject[] bubblePrefabs;

    void Start()
    {
        if (bubblePrefabs == null || bubblePrefabs.Length == 0)
        {
            Debug.LogWarning("No bubble prefabs assigned!");
            return;
        }

        GenerateGrid();
    }

    void GenerateGrid()
    {
        float diameter = bubbleRadius * 2f;
        float horizontalSpacing = diameter * horizontalSpacingMultiplier;
        float verticalSpacing = diameter * verticalSpacingMultiplier;

        Vector3 origin = transform.position;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                float offsetX = (row % 2 == 0) ? 0f : horizontalSpacing / 2f;

                float x = col * horizontalSpacing + offsetX;
                float y = -row * verticalSpacing;
                Vector3 pos = origin + new Vector3(x, y, 0f);

                GameObject prefab = bubblePrefabs[Random.Range(0, bubblePrefabs.Length)];
                GameObject obj = Instantiate(prefab, pos, Quaternion.identity, transform);

                obj.layer = LayerMask.NameToLayer("GridLayer");

                Bubble bubble = obj.GetComponent<Bubble>();
                if (bubble != null)
                {
                    Vector2Int gridPos = new Vector2Int(col, row);
                    bubble.gridPos = gridPos;
                    BubbleGridManager.Instance.RegisterBubble(gridPos, bubble);
                }
                else
                {
                    Debug.LogError("❌ Generated bubble missing Bubble component!");
                }
            }
        }
    }
}