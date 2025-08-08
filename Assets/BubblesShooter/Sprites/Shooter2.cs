using UnityEngine;

public class Shooter2 : MonoBehaviour
{
    public GameObject[] bubblePrefabs;
    public Transform shootPoint;
    public float shootSpeed = 10f;
    public float respawnDelay = 0.4f;

    private GameObject currentBubble;

    void Start()
    {
        CreateNewBubble();
    }

    void Update()
    {
        if (currentBubble == null) return;

        if (Input.GetMouseButtonDown(0))
            FireCurrentBubble();
    }

    private void FireCurrentBubble()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z - shootPoint.position.z);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Vector2 dir = (mouseWorld - shootPoint.position).normalized;

        currentBubble.transform.position = shootPoint.position;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.velocity = dir * shootSpeed;

        currentBubble.tag = "ShooterBubble";

        currentBubble = null;
        Invoke(nameof(CreateNewBubble), respawnDelay);
    }

    void CreateNewBubble()
    {
        if (bubblePrefabs.Length == 0) return;

        int rand = Random.Range(0, bubblePrefabs.Length);
        GameObject bubbleObj = Instantiate(bubblePrefabs[rand], shootPoint.position, Quaternion.identity);

        Bubble2 b = bubbleObj.GetComponent<Bubble2>();
        b.type = (BubbleType)rand;

        bubbleObj.tag = "ShooterBubble";
        currentBubble = bubbleObj;

        Rigidbody2D rb = currentBubble.GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
    }
}
