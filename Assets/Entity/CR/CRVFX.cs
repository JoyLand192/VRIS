using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRVFX : MonoBehaviour
{
    private const float wallSlideParticleInterval = 0.075f;
    [SerializeField] private AnimationEffect landingParticleEffect;
    [SerializeField] private AnimationEffect jumpingParticleEffect;
    [SerializeField] private AnimationEffect wallSlideParticleEffect;
    [SerializeField] private AnimationEffect wallJumpParticleEffect;
    [SerializeField] private AnimationEffect dashParticleEffect;
    private CRMovement movement;
    private float wallSlideParticleTimer = 0f;
    private bool wallSlideParticlePlaying = false;
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
