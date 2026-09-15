using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRIS.Skills.TheNew
{
    [CreateAssetMenu(fileName = "NeutralSlash", menuName = "VRIS/CR/Skills/The-New/5S")]
    public class NeutralSlash : Skill
    {
        private const string animationStateName = "5S";
        private const float hitStopDuration = 0.1f;
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
            return new DamageInfo(cr, 10);
        }
    }
}