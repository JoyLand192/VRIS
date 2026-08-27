using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 사용할 클래스
 * CRMovement (O)
 * CRStatus
 * CRSkillCaster (O)
 * CRInputHandler (O)
 * CRAnimator (O)
 * CRVFX
 * 
 */

public class CR : MonoBehaviour
{
    [field: SerializeField] public CRMovement Movement { get; private set; }
    [field: SerializeField] public CRInputHandler InputHandler { get; private set; }
    [field: SerializeField] public CRSkillCaster SkillCaster { get; private set; }
    [field: SerializeField] public CRAnimator Animator { get; private set; }
    [field: SerializeField] public CRVFX VFX { get; private set; }

    private void Awake()
    {
        Movement.Initialize(InputHandler, Animator);
        SkillCaster.Initialize(InputHandler, Movement, Animator);
        SkillCaster.OnSkillExecute += SkillExecuteHandler;
    }
    private void SkillExecuteHandler(Skill skill) => skill.Execute(this);
}
