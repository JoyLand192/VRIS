using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffect : Effect
{
    [SerializeField] protected ParticleSystem mainParticleSystem;
    protected List<ParticleSystem> particleSystems;
    protected override void Awake()
    {
        Debug.Assert(mainParticleSystem != null);
        particleSystems = GetComponentsInChildren<ParticleSystem>(true).ToList();
    }
    protected async UniTask WaitForEnd(float duration)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(duration), true);
        EffectEnd();
    }
    public override void Play(EffectData effectData)
    {
        transform.position = effectData.Position;
        gameObject.SetActive(true);

        particleSystems.ForEach(p => p.Play());
        WaitForEnd(mainParticleSystem.main.duration).Forget();
    }
    public override void EffectEnd()
    {
        gameObject.SetActive(true);
        returnAction?.Invoke(this);   
    }
}
