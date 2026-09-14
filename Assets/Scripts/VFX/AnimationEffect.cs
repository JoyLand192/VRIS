using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEffect : Effect
{
    protected Animator animator;
    protected override void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public override void Play(EffectData effectData)
    {
        gameObject.SetActive(true);
        animator.Rebind();
    }
    public override void EffectEnd()
    {
        gameObject.SetActive(false);
        returnAction?.Invoke(this);
    }
}
