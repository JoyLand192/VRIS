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
    [SerializeField] private CRMovement movement;
    [SerializeField] private CRInputHandler inputHandler;
    [SerializeField] private CRSkillCaster skillCaster;
    [SerializeField] private CRAnimator animator;
    public CRMovement Movement => movement;
    public CRInputHandler InputHandler => inputHandler;
    public CRSkillCaster SkillCaster => skillCaster;
    public CRAnimator Animator => animator;
    private void Awake()
    {
        movement.Initialize(inputHandler, animator);
        skillCaster.Initialize(inputHandler, movement, animator);
    }
}
