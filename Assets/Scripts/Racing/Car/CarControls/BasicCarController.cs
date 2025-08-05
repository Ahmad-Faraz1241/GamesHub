using UnityEngine;
using System.Collections;

public class BasicCarController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody playerRB;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public AudioSource engineAudio;

    [Header("Driving Settings")]
    public float motorPower = 800f;
    public float brakePower = 3000f;
    public float maxSteerAngle = 25f;
    public float downforce = 80f;
    public float topSpeed = 25f;

    [Header("Mobile Input")]
    [Range(-1f, 1f)] public float mobileSteeringInput = 0f;
    [Range(0f, 1f)] public float mobileGasInput = 0f;
    [Range(0f, 1f)] public float mobileBrakeInput = 0f;
    [Range(0f, 1f)] public float mobileReverseInput = 0f;

    [Header("Advanced Steering")]
    public AnimationCurve steeringCurve = AnimationCurve.EaseInOut(0, 1.2f, 1, 0.4f);
    public float steeringSmoothSpeed = 4f;

    [Header("Engine Sound Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;

    private float gasInput;
    private float brakeInput;
    private float steeringInput;
    private float currentSteerAngle = 0f;
    private bool isLocked = false;

    void Start()
    {
        if (playerRB)
            playerRB.centerOfMass = new Vector3(0f, -0.8f, 0f);
    }

    void Update()
    {
        if (isLocked) return;

#if UNITY_EDITOR || UNITY_STANDALONE
        gasInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        if (Mathf.Abs(steeringInput) < 0.05f) steeringInput = 0f;

        bool isBraking = Input.GetKey(KeyCode.Space);
        bool reverseBraking = gasInput < 0f && playerRB.velocity.z > 1f;

        if (reverseBraking)
        {
            brakeInput = Mathf.Abs(gasInput);
            gasInput = 0f;
        }
        else if (isBraking)
        {
            brakeInput = 1f;
        }
        else
        {
            brakeInput = 0f;
        }

#else
        steeringInput = Mathf.Abs(mobileSteeringInput) < 0.05f ? 0f : mobileSteeringInput;

        if (mobileGasInput > 0f)
        {
            gasInput = mobileGasInput;
            brakeInput = 0f;
        }
        else if (mobileReverseInput > 0f)
        {
            gasInput = -mobileReverseInput;
            brakeInput = 0f;
        }
        else if (mobileBrakeInput > 0f)
        {
            gasInput = 0f;
            brakeInput = mobileBrakeInput;
        }
        else
        {
            gasInput = 0f;
            brakeInput = 0f;
        }
#endif
    }

    void FixedUpdate()
    {
        if (isLocked) return;

        ApplyMotor();
        ApplySteering();
        ApplyBrakes();
        ApplyDownforce();
        StabilizeLateralVelocity();
        LimitTopSpeed();
        UpdateWheelVisuals();
        UpdateEngineSound();
    }

    public void LockControlsAndStopCar(bool stopCompletely)
    {
        if (isLocked) return;
        isLocked = true;
        StartCoroutine(SmoothStopCar(stopCompletely));
    }

    private IEnumerator SmoothStopCar(bool stopCompletely)
    {
        float duration = 2f;
        float elapsed = 0f;

        Vector3 initialVelocity = playerRB.velocity;
        Vector3 initialAngular = playerRB.angularVelocity;
        float initialPitch = engineAudio != null ? engineAudio.pitch : 1f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            playerRB.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, t);
            playerRB.angularVelocity = Vector3.Lerp(initialAngular, Vector3.zero, t);

            if (engineAudio != null)
            {
                engineAudio.pitch = Mathf.Lerp(initialPitch, minPitch * 0.5f, t);
                engineAudio.volume = Mathf.Lerp(1f, 0f, t);
            }

            brakeInput = Mathf.Lerp(0f, 1f, t);
            ApplyBrakes();

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerRB.velocity = Vector3.zero;
        playerRB.angularVelocity = Vector3.zero;

        if (engineAudio != null)
        {
            engineAudio.pitch = minPitch * 0.5f;
            engineAudio.volume = 0f;
            engineAudio.Stop();
        }

        if (stopCompletely)
            playerRB.isKinematic = true;
    }

    void ApplyMotor()
    {
        float torque = gasInput * motorPower;
        colliders.RRWheel.motorTorque = torque;
        colliders.RLWheel.motorTorque = torque;
    }

    void ApplySteering()
    {
        float speed = playerRB.velocity.magnitude;
        float normalizedSpeed = Mathf.Clamp01(speed / topSpeed);
        float steerSensitivity = steeringCurve.Evaluate(normalizedSpeed);

        float targetAngle = steeringInput * maxSteerAngle * steerSensitivity;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetAngle, Time.fixedDeltaTime * steeringSmoothSpeed);

        if (Mathf.Abs(steeringInput) < 0.01f)
        {
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, 0f, Time.fixedDeltaTime * (steeringSmoothSpeed * 0.5f));
        }

        colliders.FRWheel.steerAngle = currentSteerAngle;
        colliders.FLWheel.steerAngle = currentSteerAngle;
    }

    void ApplyBrakes()
    {
        float brakeForce = brakeInput * brakePower;
        float steerInfluence = Mathf.Clamp01(1f - Mathf.Abs(steeringInput));

        float frontBrake = brakeForce * steerInfluence;
        colliders.FRWheel.brakeTorque = frontBrake;
        colliders.FLWheel.brakeTorque = frontBrake;
        colliders.RRWheel.brakeTorque = brakeForce;
        colliders.RLWheel.brakeTorque = brakeForce;
    }

    void ApplyDownforce()
    {
        float downforceAmount = downforce * playerRB.velocity.magnitude;
        playerRB.AddForce(-transform.up * downforceAmount);
    }

    void StabilizeLateralVelocity()
    {
        Vector3 localVel = transform.InverseTransformDirection(playerRB.velocity);
        localVel.x *= 0.85f;
        playerRB.velocity = transform.TransformDirection(localVel);
    }

    void LimitTopSpeed()
    {
        Vector3 horizontalVel = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);
        if (horizontalVel.magnitude > topSpeed)
        {
            Vector3 limitedVel = horizontalVel.normalized * topSpeed;
            playerRB.velocity = new Vector3(limitedVel.x, playerRB.velocity.y, limitedVel.z);
        }
    }

    void UpdateWheelVisuals()
    {
        UpdateWheelPose(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheelPose(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheelPose(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheelPose(colliders.RLWheel, wheelMeshes.RLWheel);
    }

    void UpdateWheelPose(WheelCollider collider, MeshRenderer mesh)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        mesh.transform.position = pos;
        mesh.transform.rotation = rot;
    }

    void UpdateEngineSound()
    {
        if (engineAudio == null || playerRB == null) return;

        float speedPercent = playerRB.velocity.magnitude / topSpeed;
        speedPercent = Mathf.Clamp01(speedPercent);

        engineAudio.pitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);

        if (!engineAudio.isPlaying)
            engineAudio.Play();
    }

    [System.Serializable]
    public class WheelColliders
    {
        public WheelCollider FRWheel;
        public WheelCollider FLWheel;
        public WheelCollider RRWheel;
        public WheelCollider RLWheel;
    }

    [System.Serializable]
    public class WheelMeshes
    {
        public MeshRenderer FRWheel;
        public MeshRenderer FLWheel;
        public MeshRenderer RRWheel;
        public MeshRenderer RLWheel;
    }
}
