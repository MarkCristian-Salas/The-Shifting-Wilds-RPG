using UnityEngine;

public class EagleController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float rotationSpeed = 600f;
    [SerializeField] float gravity = -9.81f * 2;
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

        HandleMovement();

        animator.SetFloat("moveAmount", Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")), 0.2f, Time.deltaTime);
        animator.SetBool("isGround", isGrounded);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveInput = new Vector3(h, 0, v).normalized;
        Vector3 moveDir = cameraController.PlanarRotation * moveInput;

        if (moveInput.magnitude > 0)
        {
            controller.Move(moveDir * moveSpeed * Time.deltaTime);
            targetRotation = Quaternion.LookRotation(moveDir);
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    public void ActivateEagle()
    {
        Debug.Log("Eagle activated!");
    }
    public void DeactivateEagle()
    {
        Debug.Log("Eagle deactivated!");
    }
}
