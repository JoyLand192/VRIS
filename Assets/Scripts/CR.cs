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
    public CRMovement Movement => movement;
    public CRInputHandler InputHandler => inputHandler;
    private void Awake()
    {
        movement.Initialize(inputHandler);
    }
}
