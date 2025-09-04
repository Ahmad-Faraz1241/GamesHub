using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        BirdController bird = other.GetComponent<BirdController>();
        if (bird != null)
        {
            bird.AddScore(1);
            Destroy(gameObject); // Prevent multiple scoring from same pipe
        }
    }
}
