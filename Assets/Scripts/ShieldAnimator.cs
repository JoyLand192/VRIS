using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShieldAnimator : MonoBehaviour
{
    private SpriteRenderer render;
    private Tween justTween;
    private Tween alphaTween;
    private MaterialPropertyBlock mpb;
    private readonly int alphaFieldID = Shader.PropertyToID("_Alpha");
    private readonly int justProgressFieldID = Shader.PropertyToID("_JustProgress");
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        mpb = new();
    }
    public void Disappear(float duration = 0.4f)
    {
        alphaTween.Kill();
        alphaTween = DOTween.To(
            () => GetFloatValue(alphaFieldID),
            (x) => SetFloatValue(alphaFieldID, x), 0, duration);
    }
    public void Play(float duration = 0.4f)
    {
        justTween.Kill();
        alphaTween.Kill();
        SetFloatValue(alphaFieldID, 1f);
        SetFloatValue(justProgressFieldID, 0f);

        justTween = DOTween.To(
            () => GetFloatValue(justProgressFieldID),
            (x) => SetFloatValue(justProgressFieldID, x), 1, duration);
    }
    private float GetFloatValue(int id)
    {
        render.GetPropertyBlock(mpb);
        //if (!mpb.HasProperty(id)) return default;

        return mpb.GetFloat(id);
    }
    private void SetFloatValue(int id, float value)
    {
        render.GetPropertyBlock(mpb);
        //if (!mpb.HasProperty(id)) return;

        mpb.SetFloat(id, value);
        render.SetPropertyBlock(mpb);
    }
}
