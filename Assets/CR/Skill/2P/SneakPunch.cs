using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRIS.Skills.TheNew
{
    [CreateAssetMenu(fileName = "SneakPunch", menuName = "VRIS/CR/Skills/The-New/2P")]
    public class SneakPunch : Skill
    {
        private const string animationStateName = "2P";
        public override UniTask Execute(CR cr)
        {
            cr.Animator.PlayState(animationStateName);

            return UniTask.CompletedTask;
        }
        protected override DamageInfo CalculateDamage(CR cr)
        {
            return new DamageInfo(cr, 10);
        }
    }
}