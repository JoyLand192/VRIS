using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{
    public Effect Prefab { get; private set; }
    protected System.Action<Effect> returnAction;
    protected abstract void Awake();
    public virtual void Initialize(Effect keyPrefab, System.Action<Effect> returnAction)
    {
        this.returnAction = returnAction;
        Prefab = keyPrefab;
    }
    public abstract void Play(EffectData effectData);
    public abstract void EffectEnd();
}
