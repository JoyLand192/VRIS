using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRIS.Skills.TheNew
{
    [CreateAssetMenu(fileName = "JumpSlash", menuName = "VRIS/CR/Skills/The-New/j.S")]
    public class JumpSlash : Skill
    {
        [SerializeField] private AnimationEffect hitEffect;
        [SerializeField] private ParticleEffect hitParticleEffect;
        private const float hitStopDuration = 0.1f;
        private const string animationStateName = "j_S";
        public override UniTask Execute(CR cr)
        {
            cr.Animator.PlayState(animationStateName);

            return UniTask.CompletedTask;
        }
        protected override void HitEnemy(Enemy enemy, CR caster)
        {
            base.HitEnemy(enemy, caster);

            var effectPosition = enemy.Hitbox.ClosestHitboxPoint(caster.VFX.HitEffectPosition);
            var effectDirection = caster.Movement.MoveRatioFixed;

            caster.VFX.GenerateSkillEffect(new EffectData(hitParticleEffect, effectPosition, direction: effectDirection));
            caster.VFX.GenerateSkillEffect(new EffectData(hitEffect, effectPosition, direction: effectDirection));
            TimeManager.Instance.HitStop(hitStopDuration);
        }
        protected override DamageInfo CalculateDamage(CR cr)
        {
            return new DamageInfo(cr, 15);
        }
    }
}