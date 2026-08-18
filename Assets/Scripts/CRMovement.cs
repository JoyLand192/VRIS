using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CRMovement : MonoBehaviour
{
    private const int tempCRJumpCount = 3;
    private const float tempCRMoveSpeed = 7f;
    private const float tempCRSprintMultiplier = 2f;
    private const float tempCRJumpPower = 35.5f;

    private const float moveXInertiaReduction = 7f;
    private const float jumpDashInterval = 0.08f;
    private const float jumpDashVelocityX = 26f;
    private const float jumpDashVelocityTime = 0.2f;
    private const float jumpDashSpeedReduction = 0.65f;

    private const float platformRayLength = 0.25f;
    private const float wallRayLength = 0.15f;
    private const float groundedVelocityThreshold = 0.35f;
    [SerializeField] private SurfaceLayerMaskSettings surfaceLayerMaskSettings;
    private float moveRatio = 0f;
    private float moveRatioFixed = 1f;
    private float moveXInertia = 0f;
    private bool jumpTrigger = false;
    private bool dashTrigger = false;
    private bool dashCancelTrigger = false;
    private bool isSprinting = false;
    private bool isDashing = false;
    [SerializeField] int availableJumpCount = tempCRJumpCount;
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
    public SurfaceContact CurrentContact { get; private set; }
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
        inputHandler.OnDashCancel += OnDashCancel;
    }
    //   KeyInputHandler   ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ|
    private void OnHorizontalInput(float value)
    {
        if (isDashing) return;

        moveRatio = value;
        if (value != 0) moveRatioFixed = value;
    }
    private void OnJumpInput()
    {
        jumpTrigger = true;
    }
    private void OnDashInput() => dashTrigger = true;
    private void OnDashCancel()
    {
        if (CurrentContact != SurfaceContact.GROUNDED && isSprinting)
        {
            dashCancelTrigger = true;
            return;
        }
        isSprinting = false;
    }
    //ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ
    private void Update()
    {
        Move();
    }
    private void FixedUpdate()
    {
        moveXInertia = Mathf.Max(0, moveXInertia - moveXInertiaReduction * Time.fixedDeltaTime);

        PlatformRaycast();
        Jump();
        Dash();
    }
    private void PlatformRaycast()
    {
        var platformLayer = surfaceLayerMaskSettings.PlatformLayer;
        var wallLayer = surfaceLayerMaskSettings.WallLayer;

        var platformHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0, Vector2.down, platformRayLength, platformLayer);

        if (platformHit)
        {
            CurrentContact = SurfaceContact.GROUNDED;
            availableJumpCount = tempCRJumpCount;
            moveXInertia = 0;

            if (dashCancelTrigger)
            {
                dashCancelTrigger = false;
                isSprinting = false;
            }
            return;
        }

        var wallHit = Physics2D.BoxCast(col.bounds.center - new Vector3(wallRayLength, 0), col.bounds.size, 0, Vector2.right, wallRayLength * 2, wallLayer);

        if (wallHit)
        {
            CurrentContact = SurfaceContact.WALLCONTACT;

            return;
        }

        CurrentContact = SurfaceContact.AIRBORNE;
    }
    private void Move()
    {
        if (!IsMovable) return;
        if (isDashing) return;

        var sprintRatio = isSprinting ? tempCRSprintMultiplier : 1;
        var fixedMoveRatio = moveRatio * (tempCRMoveSpeed * sprintRatio + moveXInertia);
        rb.velocity = new Vector2(fixedMoveRatio, rb.velocity.y);
    }
    private void Jump()
    {
        if (!jumpTrigger) return;
        jumpTrigger = false;

        if (!IsMovable) return;
        if (availableJumpCount < 1) return;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * tempCRJumpPower, ForceMode2D.Impulse);
        availableJumpCount--;
    }
    private void Dash()
    {
        if (!dashTrigger) return;
        dashTrigger = false;

        switch (CurrentContact)
        {
            case SurfaceContact.GROUNDED:
                {
                    isSprinting = true;
                    break;
                }
            case SurfaceContact.AIRBORNE:
                {
                    if (availableJumpCount < 1) return;

                    isSprinting = false;
                    dashCancelTrigger = false;
                    JumpDash().Forget();

                    break;
                }
        }
    }
    private async UniTask JumpDash()
    {
        availableJumpCount--;
        var direction = moveRatioFixed;
        var speedAfter = (jumpDashVelocityX - tempCRMoveSpeed) * jumpDashSpeedReduction;

        rb.isKinematic = true;
        isDashing = true;
        rb.velocity = Vector2.zero;

        await UniTask.Delay(TimeSpan.FromSeconds(jumpDashInterval));

        rb.velocity = Vector2.right * (direction * jumpDashVelocityX);

        await UniTask.Delay(TimeSpan.FromSeconds(jumpDashVelocityTime));

        rb.isKinematic = false;
        moveXInertia = speedAfter;
        isDashing = false;
    }
}
