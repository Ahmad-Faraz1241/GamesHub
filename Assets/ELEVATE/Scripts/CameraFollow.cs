using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 5f, -10f); 

    private void LateUpdate()
    {
        if (SpawnManager.Instance == null || SpawnManager.Instance.lastCube == null)
            return;

        Transform target = SpawnManager.Instance.lastCube;

        Vector3 desiredPosition = new Vector3(
            transform.position.x,
            target.position.y + offset.y,
            transform.position.z
        );

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
