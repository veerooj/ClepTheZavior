using System;
using Unity.VisualScripting;
using UnityEngine;
[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private PlayerInputActions inputActions;

    public Action Jump, FirePerformed, FireCancelled, SuckPerformed, SuckCancelled;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
        inputActions.Enable();
        inputActions.UI.Enable();
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Jump?.Invoke();
    }

    private void OnFirePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        FirePerformed?.Invoke();
    }
    private void OnFireCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        FireCancelled?.Invoke();
    }
    private void OnSuckPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        
        SuckPerformed?.Invoke();
    }
    private void OnSuckCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        SuckCancelled?.Invoke();
    }

    public Vector2 GetMoveInput()
    {
        return inputActions.Player.Move.ReadValue<Vector2>();
    }

    public Vector2 GetLookInput()
    {
        return inputActions.Player.Look.ReadValue<Vector2>();
    }

    private void OnEnable()
    {
        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Fire.performed += OnFirePerformed;
        inputActions.Player.Fire.canceled += OnFireCancelled;
        inputActions.Player.Suck.performed += OnSuckPerformed;
        inputActions.Player.Suck.canceled += OnSuckCancelled;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Fire.performed -= OnFirePerformed;
        inputActions.Player.Fire.canceled -= OnFireCancelled;
        inputActions.Player.Suck.performed -= OnSuckPerformed;
        inputActions.Player.Suck.canceled -= OnSuckCancelled;

    }


}