using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRIS.Skills.TheNew
{
    [CreateAssetMenu(fileName = "SneakSlash", menuName = "VRIS/CR/Skills/The-New/2S")]
    public class SneakSlash : Skill
    {
        private const string animationStateName = "2S";
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