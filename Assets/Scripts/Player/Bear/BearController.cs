using UnityEngine;

public class BearController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float gravity = -9.81f * 2;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Quaternion targetRotation;
    CameraController cameraController;
    CharacterController controller;
    Animator animator;
    Transform groundCheck;
    Vector3 velocity;
    bool isGrounded;

    private void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        groundCheck = transform.Find("GroundCheck");
    }

    void Update()
    {
        // Ground check and gravity handling
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement and jumping
        HandleMovement();
        HandleJumping();

        // Animator update
        animator.SetFloat("moveAmount", Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")), 0.2f, Time.deltaTime);
        animator.SetBool("isGround", isGrounded);
    }
    // Speed and running

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(h, 0, v).normalized;
        Vector3 moveDir = cameraController.PlanarRotation * moveInput;
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;

        if (moveInput.magnitude > 0)
        {
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void HandleJumping()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    public void ActivateBear()
    {
        Debug.Log("Bear activated!");
    }
    public void DeactivateBear()
    {
        Debug.Log("Bear deactivated!");
    }
}
