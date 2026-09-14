using System.Collections.Generic;
using UnityEngine;

public struct EffectData
{
    public Effect Prefab;
    public Vector3 Position;
    public Vector3? TargetPosition;
    public float? Direction;
    public float Duration;

    public EffectData(Effect prefab, Vector3 position, Vector3? targetPosition = null, float? direction = null, float duration = 1f)
    {
        Prefab = prefab;
        Position = position;
        TargetPosition = targetPosition;
        Direction = direction;
        Duration = duration;
    }
}
public class GlobalVFXManager : MonoBehaviour
{
    public static GlobalVFXManager Instance { get; private set; }
    private readonly Dictionary<Effect, Queue<Effect>> pools = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
    public void GenerateAnimationEffect(EffectData effectData)
    {
        var prefab = effectData.Prefab;
        if (!pools.ContainsKey(prefab)) pools.Add(prefab, new Queue<Effect>());

        Effect eff;
        if (pools[prefab].Count <= 0)
        {
            eff = Instantiate(prefab);
            eff.Initialize(prefab, ReturnAnimationEffect);
        }
        else eff = pools[prefab].Dequeue();

        eff.transform.SetPositionAndRotation(effectData.Position, prefab.transform.rotation);
        if (effectData.Direction.HasValue)
        {
            var scale = prefab.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (effectData.Direction.Value > 0 ? 1 : -1);
            eff.transform.localScale = scale;
        }

        eff.Play(effectData);
    }
    private void ReturnAnimationEffect(Effect eff)
    {
        if (!pools.ContainsKey(eff.Prefab)) pools.Add(eff.Prefab, new Queue<Effect>());
        pools[eff.Prefab].Enqueue(eff);
    }
}
