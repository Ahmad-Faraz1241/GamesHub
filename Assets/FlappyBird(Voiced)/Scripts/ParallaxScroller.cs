using UnityEngine;

public class ParallaxScroller : MonoBehaviour
{
    public float speed = 0.5f; // Scroll speed
    private float width;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float newPos = Mathf.Repeat(Time.time * speed, width);
        transform.position = startPos + Vector3.left * newPos;
    }
}
