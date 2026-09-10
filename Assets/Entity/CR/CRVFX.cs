using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CRVFX : MonoBehaviour
{
    private const float wallSlideParticleInterval = 0.075f;
    private const float hurtFlashDuration = 0.15f;
    private static readonly int hurtFlashAmountID = Shader.PropertyToID("_HurtFlashAmount");
    private MaterialPropertyBlock mpb;
    [SerializeField] private AnimationEffect landingParticleEffect;
    [SerializeField] private AnimationEffect jumpingParticleEffect;
    [SerializeField] private AnimationEffect wallSlideParticleEffect;
    [SerializeField] private AnimationEffect wallJumpParticleEffect;
    [SerializeField] private AnimationEffect dashParticleEffect;
    [SerializeField] private IntEventChannel hurtFlashEventChannel;
    private CRMovement movement;
    private SpriteRenderer render;
    private float wallSlideParticleTimer = 0f;
    private bool wallSlideParticlePlaying = false;
    private float hurtFlashAmount = 0f;
    private void Awake()
    {
        mpb = new();
        render = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        if (wallSlideParticlePlaying)
        {
            if (wallSlideParticleTimer <= 0f)
            {
                GlobalVFXManager.Instance.Generate(wallSlideParticleEffect, transform.position, movement.WallDirection);
                wallSlideParticleTimer = wallSlideParticleInterval;
            }
            wallSlideParticleTimer -= Time.deltaTime;
        }
        if (hurtFlashAmount >= 0)
        {
            hurtFlashAmount -= Time.deltaTime / hurtFlashDuration;

            render.GetPropertyBlock(mpb);
            mpb.SetFloat(hurtFlashAmountID, Mathf.Max(hurtFlashAmount, 0));
            render.SetPropertyBlock(mpb);
        }
    }
    public void Initialize(CRMovement movement)
    {
        this.movement = movement;

        movement.OnLanded += PlayLandingEffect;
        movement.OnJumped += PlayJumpingEffect;
        movement.OnWallSlide += WallSlideHandler;
        movement.OnWallJump += PlayWallJumpEffect;
        movement.OnDash += PlayDashEffect;
    }
    public void HurtFlash()
    {
        hurtFlashAmount = 1f;

        render.GetPropertyBlock(mpb);
        mpb.SetFloat(hurtFlashAmountID, hurtFlashAmount);
        render.SetPropertyBlock(mpb);

        hurtFlashEventChannel.Raise(3);
    }
    private void PlayLandingEffect() => GlobalVFXManager.Instance.Generate(landingParticleEffect, transform.position);
    private void PlayJumpingEffect() => GlobalVFXManager.Instance.Generate(jumpingParticleEffect, transform.position);
    private void PlayWallJumpEffect() => GlobalVFXManager.Instance.Generate(wallJumpParticleEffect, transform.position, movement.WallDirection);
    private void PlayDashEffect(float value) => GlobalVFXManager.Instance.Generate(dashParticleEffect, transform.position, value >= 0 ? 1 : -1);
    private void WallSlideHandler(bool value)
    {
        if (wallSlideParticlePlaying == value) return;

        wallSlideParticleTimer = wallSlideParticleInterval;
        wallSlideParticlePlaying = value;
    }
}
