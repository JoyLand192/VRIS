using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator rootAnimator;
    private void Awake()
    {
        rootAnimator = GetComponent<Animator>();
    }
}
