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
        private const float hitStopDuration = 0.25f;
        private const string animationStateName = "j_S";
        public override UniTask Execute(CR cr)
        {
            cr.Animator.PlayState(animationStateName);

            return UniTask.CompletedTask;
        }
        protected override void HitEnemy(Enemy enemy, CR caster)
        {
            base.HitEnemy(enemy, caster);
            TimeManager.Instance.HitStop(hitStopDuration);
        }
        protected override DamageInfo CalculateDamage(CR cr)
        {
            return new DamageInfo(cr, 65);
        }
    }
}