using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 14f;
    public float maxSpeed = 24f;
    public float speedIncreasePerSecond = 0.8f;

    [Header("Lane Movement")]
    public float laneDistance = 3f;
    public float laneChangeSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 5.5f;
    public float groundCheckDistance = 1.1f;
    public float fallMultiplier = 2.8f;
    public float jumpCutMultiplier = 2.2f;
    public float fastFallSpeed = 12f;

    [Header("Slide")]
    public float slideDuration = 0.85f;
    public float slideColliderHeight = 1f;
    public Vector3 slideColliderCenter = new Vector3(0f, -0.25f, 0f);

    [Header("Visual Slide")]
    public Transform visualRoot;
    public Vector3 slideVisualLocalPosition = new Vector3(0f, -0.55f, 0.25f);
    public Vector3 slideVisualLocalRotation = new Vector3(32f, 0f, 0f);
    public Vector3 slideVisualLocalScale = new Vector3(1.15f, 0.65f, 1f);
    public float slideEnterLerpSpeed = 20f;
    public float slideExitLerpSpeed = 10f;

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private int desiredLane = 1;
    private bool isGrounded;
    private bool isSliding;
    private bool slideQueuedFromAir;
    private float slideTimer;

    private float normalColliderHeight;
    private Vector3 normalColliderCenter;

    private Vector3 normalVisualLocalPosition;
    private Quaternion normalVisualLocalRotation;
    private Vector3 normalVisualLocalScale;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        normalColliderHeight = capsuleCollider.height;
        normalColliderCenter = capsuleCollider.center;

        if (visualRoot != null)
        {
            normalVisualLocalPosition = visualRoot.localPosition;
            normalVisualLocalRotation = visualRoot.localRotation;
            normalVisualLocalScale = visualRoot.localScale;
        }
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        HandleLaneInput();
        HandleJumpAndSlideInput();
        HandleSlideTimer();
        HandleVisualSlidePose();
    }

    void FixedUpdate()
    {
        HandleForwardAndLaneMovement();
        HandleBetterJumpPhysics();
        HandleAirSlideLanding();
    }

    void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (desiredLane > 0)
                desiredLane--;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (desiredLane < 2)
                desiredLane++;
        }
    }

    void HandleJumpAndSlideInput()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded && !isSliding)
        {
            Vector3 currentVelocity = rb.linearVelocity;
            currentVelocity.y = jumpForce;
            rb.linearVelocity = currentVelocity;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (isGrounded && !isSliding)
            {
                StartSlide();
            }
            else if (!isGrounded)
            {
                Vector3 currentVelocity = rb.linearVelocity;

                if (currentVelocity.y > 0f)
                {
                    currentVelocity.y = 0f;
                }

                currentVelocity.y -= fastFallSpeed;
                rb.linearVelocity = currentVelocity;

                slideQueuedFromAir = true;
            }
        }
    }

    void HandleSlideTimer()
    {
        if (!isSliding) return;

        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0f)
        {
            EndSlide();
        }
    }

    void HandleForwardAndLaneMovement()
    {
        forwardSpeed += speedIncreasePerSecond * Time.fixedDeltaTime;
        forwardSpeed = Mathf.Min(forwardSpeed, maxSpeed);

        float targetX = (desiredLane - 1) * laneDistance;
        Vector3 currentPosition = rb.position;

        float newX = Mathf.Lerp(
            currentPosition.x,
            targetX,
            laneChangeSpeed * Time.fixedDeltaTime
        );

        float newZ = currentPosition.z + forwardSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = new Vector3(newX, currentPosition.y, newZ);
        rb.MovePosition(newPosition);
    }

    void HandleBetterJumpPhysics()
    {
        Vector3 velocity = rb.linearVelocity;

        if (!isGrounded)
        {
            if (velocity.y < 0f)
            {
                velocity.y += Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (!(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.UpArrow)))
            {
                velocity.y += Physics.gravity.y * (jumpCutMultiplier - 1f) * Time.fixedDeltaTime;
            }

            rb.linearVelocity = velocity;
        }
    }

    void HandleAirSlideLanding()
    {
        if (slideQueuedFromAir && isGrounded && !isSliding)
        {
            slideQueuedFromAir = false;
            StartSlide();
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        capsuleCollider.height = slideColliderHeight;
        capsuleCollider.center = slideColliderCenter;
    }

    void EndSlide()
    {
        isSliding = false;

        capsuleCollider.height = normalColliderHeight;
        capsuleCollider.center = normalColliderCenter;
    }

    void HandleVisualSlidePose()
    {
        if (visualRoot == null) return;

        Vector3 targetPosition = isSliding ? slideVisualLocalPosition : normalVisualLocalPosition;
        Quaternion targetRotation = isSliding
            ? Quaternion.Euler(slideVisualLocalRotation)
            : normalVisualLocalRotation;
        Vector3 targetScale = isSliding ? slideVisualLocalScale : normalVisualLocalScale;

        float currentLerpSpeed = isSliding ? slideEnterLerpSpeed : slideExitLerpSpeed;

        visualRoot.localPosition = Vector3.Lerp(
            visualRoot.localPosition,
            targetPosition,
            currentLerpSpeed * Time.deltaTime
        );

        visualRoot.localRotation = Quaternion.Lerp(
            visualRoot.localRotation,
            targetRotation,
            currentLerpSpeed * Time.deltaTime
        );

        visualRoot.localScale = Vector3.Lerp(
            visualRoot.localScale,
            targetScale,
            currentLerpSpeed * Time.deltaTime
        );
    }
}