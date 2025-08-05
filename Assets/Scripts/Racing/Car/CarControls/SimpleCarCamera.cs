using UnityEngine;

public class SimpleCarCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2.5f, -6.5f);
    public float tiltAmount = 2f; // Reduced tilt for subtle effect
    public float tiltSpeed = 4f;

    private float currentTilt = 0f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.TransformPoint(offset);
        transform.position = desiredPosition;

        float steerInput = Input.GetAxis("Horizontal");
        float targetTilt = -steerInput * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        transform.LookAt(target);
        transform.Rotate(Vector3.forward, currentTilt);
    }
}
