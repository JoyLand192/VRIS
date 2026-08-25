using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 
 * 사용할 클래스
 * CRMovement
 * CRStatus
 * CRSkillCaster
 * CRInputHandler
 * CRAnimator
 * CRVFX
 * 
 */

public class CR : MonoBehaviour
{
    [SerializeField] private CRMovement movement;
    [SerializeField] private CRInputHandler inputHandler;
    [SerializeField] private CRSkillCaster skillCaster;
    public CRMovement Movement => movement;
    public CRInputHandler InputHandler => inputHandler;
    public CRSkillCaster SkillCaster => skillCaster;
    private void Awake()
    {
        movement.Initialize(inputHandler);
        skillCaster.Initialize(inputHandler);
    }
}
