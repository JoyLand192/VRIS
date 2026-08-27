using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRAnimator : MonoBehaviour
{
    [SerializeField] private Transform graphic;
    [SerializeField] private Animator rootAnimator;
    private static readonly int dashStateHash = Animator.StringToHash(nameof(DashState));
    private static readonly int isDashingHash = Animator.StringToHash(nameof(IsDashing));
    private static readonly int velocityYHash = Animator.StringToHash(nameof(VelocityY));
    private static readonly int isSneakingHash = Animator.StringToHash(nameof(IsSneaking));
    private static readonly int isGroundedHash = Animator.StringToHash(nameof(IsGrounded));
    public event System.Action OnCancelWindowOpen;
    public event System.Action OnSkillEnd;
    private int playerDirection;
    public int PlayerDirection
    {
        get => playerDirection;
        set
        {
            if (playerDirection == value) return;

            playerDirection = value;

            var localScale = graphic.localScale;
            localScale.x = Mathf.Abs(localScale.x) * playerDirection;
            graphic.localScale = localScale;
        }
    }
    public int DashState
    {
        get => rootAnimator.GetInteger(dashStateHash);
        set => rootAnimator.SetInteger(dashStateHash, value);
    }
    public float VelocityY
    {
        get => rootAnimator.GetFloat(velocityYHash);
        set => rootAnimator.SetFloat(velocityYHash, value);
    }
    public bool IsDashing
    {
        get => rootAnimator.GetBool(isDashingHash);
        set => rootAnimator.SetBool(isDashingHash, value);
    }
    public bool IsSneaking
    {
        get => rootAnimator.GetBool(isSneakingHash);
        set => rootAnimator.SetBool(isSneakingHash, value);
    }
    public bool IsGrounded
    {
        get => rootAnimator.GetBool(isGroundedHash);
        set => rootAnimator.SetBool(isGroundedHash, value);
    }
    public void PlayState(string stateName) => rootAnimator.Play(stateName);
    public void OpenCancelWindow() => OnCancelWindowOpen?.Invoke();
    public void SkillEnd() => OnSkillEnd?.Invoke();
}
