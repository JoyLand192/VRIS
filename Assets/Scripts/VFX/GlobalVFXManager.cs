using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVFXManager : MonoBehaviour
{   
    public static GlobalVFXManager Instance { get; private set; }
    private readonly Dictionary<AnimationEffect, Queue<AnimationEffect>> pools = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }
    public void Generate(AnimationEffect prefab, Vector3 pos, float? direction = null)
    {
        if (!pools.ContainsKey(prefab)) pools.Add(prefab, new Queue<AnimationEffect>());

        AnimationEffect eff;
        if (pools[prefab].Count <= 0)
        {
            eff = Instantiate(prefab);
            eff.Initialize(prefab, ReturnEffect);
        }
        else eff = pools[prefab].Dequeue();

        eff.transform.position = pos;
        eff.transform.rotation = prefab.transform.rotation;
        
        if (direction != null)
        {
            var scale = prefab.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * (direction > 0 ? 1 : -1);
            eff.transform.localScale = scale;
        }

        eff.Play();
    }
    private void ReturnEffect(AnimationEffect eff)
    {
        if (!pools.ContainsKey(eff.Prefab)) pools.Add(eff.Prefab, new Queue<AnimationEffect>());
        pools[eff.Prefab].Enqueue(eff);
    }
}
