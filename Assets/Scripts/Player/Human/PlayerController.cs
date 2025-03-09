using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float gravity = -9.81f * 2;
    [SerializeField] float jumpHeight = 3f;
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
        var moveInput = (new Vector3(h, 0, v)).normalized;
        var moveDir = cameraController.PlanarRotation * moveInput;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && moveAmount > 0)
        {
            currentSpeed = runSpeed;
            moveAmount = 1f;
        }
        else if (moveAmount > 0)
        {
            moveAmount = 0.5f;
        }

        if (moveAmount > 0)
        {
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        animator.SetFloat("moveAmount", moveAmount, 0.2f, Time.deltaTime);
        animator.SetBool("isGround", isGrounded);

        if (!isGrounded && velocity.y < 0)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }

        if (isGrounded)
        {
            animator.SetBool("isJumping", false);
        }
        else if (velocity.y > 0)
        {
            animator.SetBool("isJumping", true);
        }
    }

    public void ActivateHuman()
    {
        Debug.Log("Human activated!");
    }

    public void DeactivateHuman()
    {
        Debug.Log("Human deactivated!");
    }
}
