using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationEffect : MonoBehaviour
{
    public AnimationEffect Prefab { get; private set; }
    private Animator animator;
    private System.Action<AnimationEffect> returnAction;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Initialize(AnimationEffect keyPrefab, System.Action<AnimationEffect> returnAction)
    {
        this.returnAction = returnAction;
        Prefab = keyPrefab;
    }
    public void Play()
    {
        gameObject.SetActive(true);
        animator.Rebind();
    }
    public void EffectEnd()
    {
        gameObject.SetActive(false);
        returnAction?.Invoke(this);
    }
}
