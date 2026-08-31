using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CR : Entity
{
    [field: SerializeField] public CRHitbox Hitbox { get; private set; }
    [field: SerializeField] public CRInputHandler InputHandler { get; private set; }
    [field: SerializeField] public CRMovement Movement { get; private set; }
    [field: SerializeField] public CRStatus Status { get; private set; }
    [field: SerializeField] public CRAnimator Animator { get; private set; }
    [field: SerializeField] public CRSkillCaster SkillCaster { get; private set; }
    [field: SerializeField] public CRVFX VFX { get; private set; }

    private void Awake()
    {
        Movement.Initialize(InputHandler, Animator);
        SkillCaster.Initialize(this);
        SkillCaster.OnSkillExecute += SkillExecuteHandler;
    }
    private void SkillExecuteHandler(Skill skill)
    {
        skill.Initialize(this);
        skill.Execute(this);
    }
}
