
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform planetCenter;

    [Header("Movement")]
    [SerializeField, Range(1f, 10f)]
    private float moveSpeed = 5f;

    [Header("Rotation")]
    [SerializeField, Range(20f, 360f)]
    private float rotationSpeed = 180f;

    [SerializeField] private float rotationAngleLimit = 40f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private int jumpBufferFrames = 10;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayers = ~0;

    [Header("Look Inversion")]
    [SerializeField] private bool isXInverted;
    [SerializeField] private bool isYInverted;

    private Rigidbody _rb;

    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private bool _jumpRequested;
    private int _jumpBufferFrameCount;
    private bool _isGrounded;

    private int _xInversion => isXInverted ? -1 : 1;
    private int _yInversion => isYInverted ? -1 : 1;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _rb.useGravity = false;
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (cameraTransform == null)
        {
            Camera playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera != null)
                cameraTransform = playerCamera.transform;
        }
    }

    private void Update()
    {
        _moveInput = InputManager.Instance.GetMoveInput();
        _lookInput = InputManager.Instance.GetLookInput();

        UpdateJumpBuffer();
        ApplyCameraRotation();
    }

    private void FixedUpdate()
    {
        if (planetCenter == null)
            return;

        Vector3 upDirection =
            (transform.position - planetCenter.position).normalized;

        CheckGrounded(upDirection);
        ApplyMovement(upDirection);
        ApplyJump(upDirection);
    }

    private void ApplyCameraRotation()
    {
        if (!cameraTransform)
            return;

        float yaw =
            _lookInput.x * _xInversion * rotationSpeed * Time.deltaTime;

        float pitch =
            -_lookInput.y * _yInversion * rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.up, yaw, Space.Self);

        Vector3 angles = cameraTransform.localEulerAngles;

        float currentPitch = angles.x > 180f
            ? angles.x - 360f
            : angles.x;

        currentPitch += pitch;

        currentPitch = Mathf.Clamp(
            currentPitch,
            -rotationAngleLimit,
            rotationAngleLimit
        );

        cameraTransform.localEulerAngles =
            new Vector3(currentPitch, 0f, 0f);
    }

    private void ApplyMovement(Vector3 upDirection)
    {
        Vector3 moveDirection =
            transform.right * _moveInput.x +
            transform.forward * _moveInput.y;
        moveDirection = Vector3.ProjectOnPlane(moveDirection, upDirection);

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        Vector3 velocity = _rb.linearVelocity;

        Vector3 radialVelocity = Vector3.Project(velocity, upDirection);

        Vector3 tangentVelocity = moveDirection * moveSpeed;

        _rb.linearVelocity = tangentVelocity + radialVelocity;
    }

    private void ApplyJump(Vector3 upDirection)
    {
        if (!_isGrounded || !_jumpRequested)
            return;

        _jumpRequested = false;
        _jumpBufferFrameCount = 0;

        _rb.AddForce(upDirection * jumpForce, ForceMode.VelocityChange);
    }

    private void UpdateJumpBuffer()
    {
        if (!_jumpRequested)
            return;

        _jumpBufferFrameCount++;

        if (_jumpBufferFrameCount > jumpBufferFrames)
        {
            _jumpRequested = false;
            _jumpBufferFrameCount = 0;
        }
    }

    private void CheckGrounded(Vector3 upDirection)
    {
        Vector3 origin = _rb.position;

        float radius = 0.3f;
        CapsuleCollider capsule = GetComponent<CapsuleCollider>();

        if (capsule)
            radius = capsule.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);

        float castRadius = Mathf.Max(0.05f, radius * 0.9f);

        _isGrounded = Physics.SphereCast(origin, castRadius, -upDirection, out _, groundCheckDistance + castRadius, groundLayers, QueryTriggerInteraction.Ignore
        );
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.Jump += OnJump;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.Jump -= OnJump;
    }

    private void OnJump()
    {
        _jumpRequested = true;
        _jumpBufferFrameCount = 0;
    }
}
