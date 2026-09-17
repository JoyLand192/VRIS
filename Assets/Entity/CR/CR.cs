using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Mediator Pattern
public class CR : Entity
{
    [field: SerializeField] public HitboxManager Hitbox { get; private set; }
    [field: SerializeField] public CRInputHandler InputHandler { get; private set; }
    [field: SerializeField] public CRMovement Movement { get; private set; }
    [field: SerializeField] public CRStatus Status { get; private set; }
    [field: SerializeField] public CRAnimator Animator { get; private set; }
    [field: SerializeField] public CRSkillCaster SkillCaster { get; private set; }
    [field: SerializeField] public CRVFX VFX { get; private set; }

    private void Awake()
    {
        Hitbox.Initialize(this);
        Movement.Initialize(InputHandler, Animator);
        SkillCaster.Initialize(this);
        VFX.Initialize(Movement);

        SkillCaster.OnSkillExecute += SkillExecuteHandler;
    }
    [SerializeField] private ScreenEffectUI seui;
    private readonly Color guardFlashColor = new(0.59f, 0.95f, 1.0f);
    public DamageReceiveResult ReceiveDamage(DamageInfo damageInfo)
    {
        if (Movement.IsGuarding)
        {
            Status.GuardGauge -= 15;
            Status.ReceiveDamage(damageInfo.Scale(1));
            seui.FlashScreen(guardFlashColor, 1);
            
            return DamageReceiveResult.Guard;
        }

        Status.ReceiveDamage(damageInfo);
        VFX.HurtFlash();

        return DamageReceiveResult.Hit;
    }
    private void SkillExecuteHandler(Skill skill)
    {
        skill.Initialize(this);
        skill.Execute(this);
    }
}
