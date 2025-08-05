using UnityEngine;

public class HOPCameraFollow : MonoBehaviour
{
    public Transform target;
    public float zOffset = -8f;
    public float yOffset = 6f;
    public float followSpeed = 5f;

    private Vector3 initialPosition;

    void Start()
    {
        if (target != null)
        {
            initialPosition = transform.position;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;


        float targetZ = target.position.z + zOffset;
        Vector3 desiredPosition = new Vector3(initialPosition.x, initialPosition.y + yOffset, targetZ);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);
    }
}
