using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float distance = 5;
    [SerializeField] float minVerticalAngle = -10;
    [SerializeField] float maxVerticalAngle = 45;
    [SerializeField] float minHorizontalAngle = -180;
    [SerializeField] float maxHorizontalAngle = 180;
    [SerializeField] Vector2 framingOffset;
    [SerializeField] bool invertY;
    [SerializeField] bool invertX;

    float rotationY;
    float rotationX;
    float invertXVal;
    float invertYVal;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            followTarget = player.transform;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Stop camera movement if the game is paused
        if (PauseMenu.GameIsPaused) return;

        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;
        rotationY = Mathf.Clamp(rotationY, minHorizontalAngle, maxHorizontalAngle);

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);
        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y, 0);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    public void SetTarget(Transform newTarget)
    {
        followTarget = newTarget;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
