using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineEffect : Effect
{
    protected LineRenderer lineRenderer;
    protected MaterialPropertyBlock mpb;
    protected readonly int progressPropertyID = Shader.PropertyToID("_Progress");
    protected float duration;
    protected override void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        mpb = new();
    }
    public override void Play(EffectData effectData)
    {
        transform.position = Vector2.zero;
        gameObject.SetActive(true);
        if (effectData.TargetPosition == null)
        {
            EffectEnd();
            return;
        }
        lineRenderer.SetPositions(new Vector3[] { effectData.Position, effectData.TargetPosition.Value });

        LineDisappear().Forget();
    }
    public override void EffectEnd()
    {
        gameObject.SetActive(false);
        returnAction?.Invoke(this);
    }
    protected async UniTask LineDisappear()
    {
        await DOTween.To(
            () => lineRenderer.material.GetFloat(progressPropertyID),
            (x) =>
            {
                lineRenderer.GetPropertyBlock(mpb);
                mpb.SetFloat(progressPropertyID, x);
                lineRenderer.SetPropertyBlock(mpb);
            },
            1f,
            0.5f);

        EffectEnd();
    }
}
