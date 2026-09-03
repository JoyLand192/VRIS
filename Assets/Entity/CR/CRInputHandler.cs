using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum CommandKey
{
    Neutral = 5,
    Up = 8, Left = 4, Down = 2, Right = 6,
    LeftUp = 7, LeftDown = 1,
    RightUp = 9, RightDown = 3,
    Punch = 100, Slash, Special,
}
public readonly struct CommandInputEntry
{
    public CommandKey CommandKey { get; }
    public float InputTime { get; }
    public CommandInputEntry(CommandKey commandKey, float inputTime)
    {
        CommandKey = commandKey;
        InputTime = inputTime;
    }
    public CommandInputEntry(int commandKeyIndex, float inputTime)
    {
        CommandKey = (CommandKey)commandKeyIndex;
        InputTime = inputTime;
    }
}
public class CRInputHandler : MonoBehaviour
{
    private CRActions moveActions;
    private CRActions.CRMovementActions crMovement;
    private Vector2 dPadInput;
    public bool IsHoldingLeft => dPadInput.x < 0;
    public bool IsHoldingRight => dPadInput.x > 0;
    public bool IsHoldingDown => dPadInput.y < 0;
    public System.Action<CommandInputEntry> OnCommandKeyInput;
    public System.Action<Vector2> OnDPadInput;
    public System.Action<float> OnHorizontalInput;
    public System.Action<float> OnSprintInput;
    public System.Action<bool> OnSneakInput;
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
        moveActions = new CRActions();
        crMovement = moveActions.CRMovement;

        moveActions.Enable();

        crMovement.DPad.performed += DPadHandler;
        crMovement.DPad.canceled += DPadHandler;
        crMovement.AttackKey.performed += AttackKeyHandler;

        crMovement.Jump.performed += JumpKeyHandler;
        crMovement.Sneak.performed += SneakKeyHandler;
        crMovement.Sneak.canceled += SneakKeyHandler;
        crMovement.Dash.performed += DashKeyHandler;
    }
    private void InputDispose()
    {
        moveActions.Disable();

        crMovement.DPad.performed -= DPadHandler;
        crMovement.DPad.canceled -= DPadHandler;
        crMovement.AttackKey.performed -= AttackKeyHandler;

        crMovement.Jump.performed -= JumpKeyHandler;
        crMovement.Sneak.performed -= SneakKeyHandler;
        crMovement.Sneak.canceled -= SneakKeyHandler;
        crMovement.Dash.performed -= DashKeyHandler;
    }
    private void DPadHandler(InputAction.CallbackContext context)
    {
        dPadInput = context.ReadValue<Vector2>();
        OnDPadInput?.Invoke(dPadInput);

        var dPadCommandKey = CommandKey.Neutral;

        if (dPadInput.x == 1)
        {
            if (dPadInput.y == 1) dPadCommandKey = CommandKey.RightUp;
            else if (dPadInput.y == -1) dPadCommandKey = CommandKey.RightDown;
            else dPadCommandKey = CommandKey.Right;
        }
        else if (dPadInput.x == -1)
        {
            if (dPadInput.y == 1) dPadCommandKey = CommandKey.LeftUp;
            else if (dPadInput.y == -1) dPadCommandKey = CommandKey.LeftDown;
            else dPadCommandKey = CommandKey.Left;
        }
        else
        {
            if (dPadInput.y == 1) dPadCommandKey = CommandKey.Up;
            else if (dPadInput.y == -1) dPadCommandKey = CommandKey.Down;
        }
        OnCommandKeyInput?.Invoke(new CommandInputEntry(dPadCommandKey, Time.time));
    }
    private void AttackKeyHandler(InputAction.CallbackContext context)
    {
        var performedValue = context.ReadValue<float>();
        var rawValue = (int)performedValue - 1 + (int)CommandKey.Punch;

        OnCommandKeyInput?.Invoke(new CommandInputEntry(rawValue, Time.time));
    }
    private void SneakKeyHandler(InputAction.CallbackContext context)
    {
        var performedValue = context.ReadValue<float>();
        var isSneaking = performedValue > 0;
        OnSneakInput?.Invoke(isSneaking);
    }
    private void JumpKeyHandler(InputAction.CallbackContext context) => OnJumpInput?.Invoke();
    private void DashKeyHandler(InputAction.CallbackContext context) => OnDashInput?.Invoke();
}
