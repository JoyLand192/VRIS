using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CRMovement : MonoBehaviour
{
    private const int tempCRJumpCount = 2;
    private const float tempCRMoveSpeed = 7f;
    private const float tempCRSprintMultiplier = 2.5f;
    private const float tempCRJumpPower = 35.5f;

    private const float moveXInertiaReduction = 45f;
    private const float jumpDashInterval = 0.1f;
    private const float jumpDashVelocityX = 30f;
    private const float jumpDashVelocityTime = 0.25f;
    private const float jumpDashInertiaAfterRatio = 0.6f;
    private const float wallSlideVelocityY = 3.5f;

    private const float platformRayLength = 0.1f;
    private const float wallRayLength = 0.15f;
    private const float groundedVelocityThreshold = 0.35f;
    [SerializeField] private SurfaceLayerMaskSettings surfaceLayerMaskSettings;
    private int wallDirection;
    private float moveRatio = 0f;
    private float moveRatioFixed = 1f;
    private float moveXInertia = 0f;
    private bool jumpTrigger = false;
    private bool dashTrigger = false;
    [SerializeField] private bool sneakTrigger = false;
    private bool isSprinting = false;
    private int availableJumpCount = tempCRJumpCount;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private CRAnimator animator;
    private bool isDashing = false;
    public bool IsDashing
    {
        get => isDashing;
        private set
        {
            isDashing = value;
            animator.IsDashing = value;
        }
    }
    [SerializeField] private bool isSneaking = false;
    public bool IsSneaking
    {
        get => isSneaking;
        set
        {
            isSneaking = value;
            animator.IsSneaking = value;
        }
    }
    private bool isMovable = true;
    public bool IsMovable
    {
        get => isMovable;
        set
        {
            isMovable = value;
        }
    }
    [SerializeField] private SurfaceContact currentContact = SurfaceContact.AIRBORNE;
    public SurfaceContact CurrentContact
    {
        get => currentContact;
        private set
        {
            currentContact = value;
            animator.IsGrounded = value == SurfaceContact.GROUNDED;
        }
    }
    public enum SurfaceContact
    {
        AIRBORNE,
        GROUNDED,
        WALLCONTACT,
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
    }
    public void Initialize(CRInputHandler inputHandler, CRAnimator animator)
    {
        this.animator = animator;

        inputHandler.OnHorizontalInput += OnHorizontalInput;
        inputHandler.OnJumpInput += OnJumpInput;
        inputHandler.OnSneakInput += OnSneakInput;
        inputHandler.OnDashInput += OnDashInput;
        inputHandler.OnSprintInput += OnSprintInput;
    }
    //   KeyInputHandler   ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|
    private void OnHorizontalInput(float value)
    {
        if (IsDashing) return;
        if (value != 0)
        {
            moveRatioFixed = value;
            animator.PlayerDirection = (int)Mathf.Sign(value);
        }
        if (moveXInertia > 0) return;

        moveRatio = value;
    }
    private void OnSprintInput(float value)
    {
        if (IsDashing) return;
        isSprinting = value > 0;
    }
    private void OnSneakInput(bool value)
    {
        sneakTrigger = value;
    }
    private void OnJumpInput() => jumpTrigger = true;
    private void OnDashInput() => dashTrigger = true;
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
    private void FixedUpdate()
    {
        moveXInertia = Mathf.Max(0, moveXInertia - moveXInertiaReduction * Time.fixedDeltaTime);
        animator.VelocityY = rb.velocity.y;

        Move();
        PlatformRaycast();
        Sneak();
        Jump();
        Dash();
    }
    private void PlatformRaycast()
    {
        var platformLayer = surfaceLayerMaskSettings.PlatformLayer;
        var wallLayer = surfaceLayerMaskSettings.WallLayer;

        var platformHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, platformRayLength, platformLayer);

        if (platformHit && rb.velocity.y <= groundedVelocityThreshold)
        {
            CurrentContact = SurfaceContact.GROUNDED;
            availableJumpCount = tempCRJumpCount;
            moveXInertia = 0;

            return;
        }

        var wallHit = Physics2D.BoxCast(col.bounds.center - new Vector3(wallRayLength, 0), col.bounds.size, 0, Vector2.right, wallRayLength * 2, wallLayer);

        if (wallHit)
        {
            CurrentContact = SurfaceContact.WALLCONTACT;
            wallDirection = wallHit.transform.position.x >= transform.position.x ? 1 : -1;

            return;
        }
        CurrentContact = SurfaceContact.AIRBORNE;
    }
    private void Sneak()
    {
        if (!sneakTrigger)
        {
            IsSneaking = false;
            return;
        }
        if (!IsMovable || IsDashing || CurrentContact != SurfaceContact.GROUNDED) return;
        IsSneaking = true;
    }
    private void Move()
    {
        if (!IsMovable) return;
        if (IsDashing) return;
        if (IsSneaking)
        {
            rb.velocity = Vector2.up * rb.velocity.y;
            return;
        }
        if (CurrentContact == SurfaceContact.WALLCONTACT && IsHoldingWall()) WallSlide();

        var sprintRatio = isSprinting ? tempCRSprintMultiplier : 1;
        var fixedMoveRatio = moveRatio * (tempCRMoveSpeed * sprintRatio + moveXInertia);
        rb.velocity = new Vector2(fixedMoveRatio, rb.velocity.y);
    }
    private bool IsHoldingWall() => moveRatioFixed != 0 && Mathf.Sign(moveRatioFixed) == wallDirection;
    private void WallSlide()
    {
        var fixedVelocityY = Mathf.Max(rb.velocity.y, -wallSlideVelocityY);
        rb.velocity = new Vector2(0, fixedVelocityY);
    }
    private void Jump()
    {
        if (!jumpTrigger) return;
        jumpTrigger = false;

        if (!IsMovable || IsDashing) return;
        IsSneaking = false; // 점프 시 웅크리기 해제

        if (CurrentContact == SurfaceContact.WALLCONTACT)
        {
            WallJump();
            return;
        }

        if (availableJumpCount < 1) return;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * tempCRJumpPower, ForceMode2D.Impulse);
        availableJumpCount--;
    }
    private void WallJump()
    {
        if (IsDashing) return;

        moveRatio = -wallDirection;

        var power = new Vector2(-wallDirection, 1.7f).normalized * tempCRJumpPower;
        rb.velocity = power;

        moveXInertia = Mathf.Abs(power.x) - tempCRMoveSpeed;
    }
    private void Dash()
    {
        if (!dashTrigger) return;
        dashTrigger = false;

        if (CurrentContact != SurfaceContact.AIRBORNE) return;
        if (availableJumpCount < 1) return;
        JumpDash().Forget();
    }
    private async UniTask JumpDash()
    {
        if (IsDashing) return;

        availableJumpCount--;

        var direction = moveRatioFixed;
        var speedAfter = (jumpDashVelocityX - tempCRMoveSpeed) * jumpDashInertiaAfterRatio;
        var gravityPrev = rb.gravityScale;

        rb.gravityScale = 0;
        IsDashing = true;
        animator.DashState = 1;
        rb.velocity = Vector2.zero;

        await UniTask.Delay(TimeSpan.FromSeconds(jumpDashInterval));

        rb.velocity = Vector2.right * (direction * jumpDashVelocityX);
        animator.DashState = 2;

        await UniTask.Delay(TimeSpan.FromSeconds(jumpDashVelocityTime));

        moveXInertia = speedAfter;
        moveRatio = direction;
        rb.gravityScale = gravityPrev;

        IsDashing = false;
        animator.DashState = 0;
        isSprinting = false;
    }
}
