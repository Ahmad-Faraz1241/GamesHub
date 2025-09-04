using UnityEngine;

public class PipeMovement : MonoBehaviour
{
    public float baseSpeed = 2f;
    public float maxSpeed = 6f;
    private float currentSpeed = 2f;

    public void SetDifficulty(float difficultyPercent)
    {
        currentSpeed = Mathf.Lerp(baseSpeed, maxSpeed, difficultyPercent);
    }

    void Update()
    {
        transform.position += Vector3.left * currentSpeed * Time.deltaTime;

        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
