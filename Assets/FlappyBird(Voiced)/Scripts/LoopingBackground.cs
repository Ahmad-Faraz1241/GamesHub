using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public float speed = 2f; // Move speed
    private float width;     // Width of background sprite

    void Start()
    {
        // Get sprite width in world units
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move left
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // If fully off-screen to the left, move to the right end
        if (transform.position.x <= -width)
        {
            Vector3 newPos = new Vector3(transform.position.x + width * 2, transform.position.y, transform.position.z);
            transform.position = newPos;
        }
    }
}
