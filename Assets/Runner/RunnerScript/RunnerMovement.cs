using UnityEngine;
using System.Collections;

public class RunnerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardSpeed = 5f;
    public float laneDistance = 2f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    public float lanespeed = 10f;

    [Header("Slide Settings")]
    public float slideHeight = 1.4f;
    public float slideCenterY = 0.7f;
    public float slideDuration = 1f;

    [Header("Audio")]
    public AudioSource runAudio;
    public AudioSource collideSound;

    private CharacterController controller;
    private Animator animator;

    private int currentLane = 1;
    private float verticalVelocity = 0f;
    private bool isDead = false;
    private bool isSliding = false;

    private float originalHeight;
    private Vector3 originalCenter;
    private float initialSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        originalHeight = controller.height;
        originalCenter = controller.center;
        initialSpeed = forwardSpeed;
    }

    void Update()
    {
        if (isDead) return;

        Vector3 move = Vector3.forward * forwardSpeed;

        if (controller.isGrounded)
        {
            verticalVelocity = -1f;
            animator.SetBool("Grounded", true);

            if (!runAudio.isPlaying)
                runAudio.Play();

            if (MobileInput.Instance.SwipeUp)
            {
                verticalVelocity = jumpForce;
                animator.SetTrigger("Jump");
                runAudio.Stop();
            }

            if (MobileInput.Instance.SwipeDown && !isSliding)
            {
                StartCoroutine(Slide());
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            animator.SetBool("Grounded", false);

            if (runAudio.isPlaying)
                runAudio.Stop();
        }

        move.y = verticalVelocity;

        if (MobileInput.Instance.SwipeLeft && currentLane > 0)
            currentLane--;
        else if (MobileInput.Instance.SwipeRight && currentLane < 2)
            currentLane++;

        float targetX = (currentLane - 1) * laneDistance;
        float diffX = targetX - transform.position.x;
        move.x = diffX * lanespeed;

        controller.Move(move * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacles"))
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        forwardSpeed = 0f;
        animator.SetTrigger("Hit");
        runAudio.Stop();

        if (collideSound && !collideSound.isPlaying)
            collideSound.Play();
    }

    private IEnumerator Slide()
    {
        isSliding = true;

        animator.SetTrigger("Slide");

        controller.height = slideHeight;
        controller.center = new Vector3(originalCenter.x, slideCenterY, originalCenter.z);

        yield return new WaitForSeconds(slideDuration);

        controller.height = originalHeight;
        controller.center = originalCenter;

        isSliding = false;
    }

    public void ResetMovement()
    {
        forwardSpeed = initialSpeed;
        verticalVelocity = 0f;
        isDead = false;
        animator.speed = 1f;

        if (runAudio) runAudio.Play();
    }

    public void IncreaseSpeed(float moveAmount, float animAmount)
    {
        forwardSpeed = Mathf.Min(forwardSpeed + moveAmount, 15f);      // Cap at 15
        animator.speed = Mathf.Min(animator.speed + animAmount, 2f);   // Cap at 2
    }
}
