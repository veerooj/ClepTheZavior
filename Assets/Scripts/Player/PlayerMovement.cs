using UnityEngine;

public class PlayerMovement: MonoBehaviour
{
    
    [SerializeField] Transform cameraTransform;
    [SerializeField, Range(1f, 5f)]
    private float moveSpeed;
    [SerializeField, Range(20f, 360f)]
    private float rotationSpeed;
    [SerializeField] private float
        jumpForce,
        gravity,
        verticalVelocity,
        terminalVelocity,
        groundVelocity = -1f;

    [SerializeField] private int jumpBufferFrames = 10;
    [SerializeField] private float rotationAngleLimit = 40f;
    [SerializeField] bool isXInverted, isYInverted;
    private int _xInversion => isXInverted ? -1 : 1;
    private int _yInversion => isYInverted ? -1 : 1;

    private int _jumpBufferFrameCount = 0;

    private bool _isJumping, _jumpRequested;

    private CharacterController _cc;

    private Vector3 _playerCameraRotation;
    private Vector2 _cameraRotation;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();

        _cameraRotation = Vector2.zero;
        _playerCameraRotation = Vector3.zero;
    }

    private void Start()
    {
        cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        ApplyJump();

        var finalMoveDir = GetMoveFinalDirection();

        _cc.Move(finalMoveDir);

        ApplyRotation();
    }
    private void ApplyRotation()
    {
        var cameraInput = InputManager.Instance.GetLookInput();

        _cameraRotation.x = cameraInput.y * _yInversion;
        _cameraRotation.y = cameraInput.x * _xInversion;

        var cameraRotationVector =
            new Vector3(_cameraRotation.x, 0, 0) *
            (rotationSpeed * Time.deltaTime);
        cameraTransform.Rotate(cameraRotationVector);

        var angleTransformation = (cameraTransform.localEulerAngles.x > 180)
            ? cameraTransform.localEulerAngles.x - 360
            : cameraTransform.localEulerAngles.x;

        cameraRotationVector = new Vector3(
            Mathf.Clamp(
                angleTransformation,
                -rotationAngleLimit,
                rotationAngleLimit),
            0f, 0f);

        cameraTransform.localEulerAngles = cameraRotationVector;

        _playerCameraRotation.y = _cameraRotation.y;
        transform.Rotate(_playerCameraRotation * (rotationSpeed * Time.deltaTime));
    }

    private void ApplyJump()
    {
        _jumpRequested = _jumpRequested && !(_jumpBufferFrameCount++ >= jumpBufferFrames);

        if(!_jumpRequested) _jumpBufferFrameCount = 0;

        if(!_cc.isGrounded)
        {
            ApplyGravity();
        }
        else
        {
            if(_jumpRequested && !_isJumping)
            {
                _jumpRequested = false;
                _jumpBufferFrameCount = 0;
                verticalVelocity = jumpForce;
            }
            else
            {
                verticalVelocity = groundVelocity;
            }
        }

    }

    private void ApplyGravity()
    {
        verticalVelocity -= gravity * Time.deltaTime;
        if (verticalVelocity <= -terminalVelocity)
            verticalVelocity = -terminalVelocity;
    }

    private Vector3 GetMoveFinalDirection()
    {
        var inputDir = InputManager.Instance.GetMoveInput();

        //var moveDir = new Vector3(inputDir.x, 0f, inputDir.y);
        var moveDir = inputDir.x * transform.right + inputDir.y * transform.forward;
        if (moveDir.sqrMagnitude > 1)
            moveDir.Normalize();
        return moveDir * (moveSpeed * Time.deltaTime)
            + Vector3.up * (verticalVelocity * Time.deltaTime); ;
    }

    private void OnEnable()
    {
        InputManager.Instance.Jump += () => _jumpRequested = true;
    }

    private void OnDisable()
    {
        InputManager.Instance.Jump -= () => _jumpRequested = true;
    }
    
}
