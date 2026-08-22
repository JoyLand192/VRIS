using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CRInputHandler : MonoBehaviour
{
    private MovementActions moveActions;
    private MovementActions.CRMovementActions crMovement;
    public System.Action<Vector2> OnDPadInput;
    public System.Action<float> OnHorizontalInput;
    public System.Action<float> OnSprintInput;
    public System.Action OnJumpInput;
    public System.Action OnDashInput;
    private void OnEnable()
    {
        InputInitialize();
    }
    private void OnDisable()
    {
        InputDispose();
    }
    private void Update()
    {
        OnHorizontalInput?.Invoke(crMovement.Move.ReadValue<float>());
        OnSprintInput?.Invoke(crMovement.Sprint.ReadValue<float>());
    }
    private void InputInitialize()
    {
        moveActions = new MovementActions();
        crMovement = moveActions.CRMovement;

        moveActions.Enable();

        crMovement.DPad.performed += DPadHandler;
        crMovement.DPad.canceled += DPadHandler;

        crMovement.Jump.performed += JumpKeyHandler;
        crMovement.Dash.performed += DashKeyHandler;
    }
    private void InputDispose()
    {
        moveActions.Disable();

        crMovement.DPad.performed -= DPadHandler;
        crMovement.DPad.canceled -= DPadHandler;

        crMovement.Jump.performed -= JumpKeyHandler;
        crMovement.Dash.performed -= DashKeyHandler;
    }
    private void DPadHandler(InputAction.CallbackContext context) => OnDPadInput?.Invoke(context.ReadValue<Vector2>());
    private void JumpKeyHandler(InputAction.CallbackContext context) => OnJumpInput?.Invoke();
    private void DashKeyHandler(InputAction.CallbackContext context) => OnDashInput?.Invoke();
}
