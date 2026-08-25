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
    private const float jumpDashInterval = 0.05f;
    private const float jumpDashVelocityX = 28f;
    private const float jumpDashVelocityTime = 0.15f;
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
    private bool isSprinting = false;
    private bool isDashing = false;
    private int availableJumpCount = tempCRJumpCount;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool isMovable = true;
    public bool IsMovable
    {
        get => isMovable;
        set
        {
            isMovable = value;
        }
    }
    [field: SerializeField] public SurfaceContact CurrentContact { get; private set; }
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
    public void Initialize(CRInputHandler inputHandler)
    {
        inputHandler.OnHorizontalInput += OnHorizontalInput;
        inputHandler.OnJumpInput += OnJumpInput;
        inputHandler.OnDashInput += OnDashInput;
        inputHandler.OnSprintInput += OnSprintInput;
    }
    //   KeyInputHandler   ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|
    private void OnHorizontalInput(float value)
    {
        if (isDashing) return;
        if (value != 0) moveRatioFixed = value;

        if (moveXInertia > 0) return;

        moveRatio = value;
    }
    private void OnSprintInput(float value)
    {
        if (isDashing) return;
        isSprinting = value > 0;
    }
    private void OnJumpInput() => jumpTrigger = true;
    private void OnDashInput() => dashTrigger = true;
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
    private void FixedUpdate()
    {
        moveXInertia = Mathf.Max(0, moveXInertia - moveXInertiaReduction * Time.fixedDeltaTime);

        Move();
        PlatformRaycast();
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
    private void Move()
    {
        if (!IsMovable) return;
        if (isDashing) return;
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

        if (!IsMovable | isDashing) return;
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
        if (isDashing) return;

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
        if (isDashing) return;

        availableJumpCount--;

        var direction = moveRatioFixed;
        var speedAfter = jumpDashVelocityX - tempCRMoveSpeed;
        var gravityPrev = rb.gravityScale;

        rb.gravityScale = 0;
        isDashing = true;
        rb.velocity = Vector2.zero;

        await UniTask.Delay(TimeSpan.FromSeconds(jumpDashInterval));

        rb.velocity = Vector2.right * (direction * jumpDashVelocityX);

        await UniTask.Delay(TimeSpan.FromSeconds(jumpDashVelocityTime));

        moveXInertia = speedAfter;
        moveRatio = direction;
        rb.gravityScale = gravityPrev;

        isDashing = false;
        isSprinting = false;
    }
}
