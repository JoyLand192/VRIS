using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CRInputHandler : MonoBehaviour
{
    private MovementActions moveActions;
    private MovementActions.CRMovementActions crMovement;
    public System.Action<float> OnHorizontalInput;
    public System.Action OnJumpInput;
    public System.Action OnDashInput;
    public System.Action OnDashCancel;
    private void OnEnable()
    {
        InputInitialize();
    }
    private void OnDestroy()
    {
        InputDispose();
    }
    private void InputInitialize()
    {
        moveActions = new MovementActions();
        crMovement = moveActions.CRMovement;

        moveActions.Enable();

        crMovement.Jump.performed += JumpKeyHandler;
        crMovement.Dash.performed += DashKeyHandler;
        crMovement.Dash.canceled += DashCancelHandler;
    }
    private void InputDispose()
    {
        moveActions.Disable();

        crMovement.Jump.performed -= JumpKeyHandler;                         
        crMovement.Dash.performed -= DashKeyHandler;
        crMovement.Dash.canceled -= DashCancelHandler;
    }
    private void Update()
    {
        OnHorizontalInput?.Invoke(crMovement.Move.ReadValue<float>());
    }
    private void JumpKeyHandler(InputAction.CallbackContext context) => OnJumpInput?.Invoke();
    private void DashKeyHandler(InputAction.CallbackContext context) => OnDashInput?.Invoke();
    private void DashCancelHandler(InputAction.CallbackContext context) => OnDashCancel?.Invoke();
}
