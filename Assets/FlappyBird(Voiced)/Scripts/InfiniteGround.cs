using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    public float speed = 2f;
    private float width;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        width = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        // Move left
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // If this piece goes fully off screen → move to right side
        if (transform.position.x < -width)
        {
            transform.position += new Vector3(width * 2f, 0, 0);
        }
    }
}
