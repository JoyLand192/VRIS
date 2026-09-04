using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    [SerializeField] private List<CollisionDetector> hitBoxes;
    [SerializeField] private List<CollisionDetector> damageBoxes;
    public event System.Action<CollisionDetector> OnEntityHit;
    private void OnDestroy()
    {
        Dispose();
    }
    public void Initialize(Entity entity)
    {
        hitBoxes.ForEach(h => h.SetOwner(entity));
        damageBoxes.ForEach(d => d.SetOwner(entity));
        hitBoxes.ForEach(h => h.OnTriggerEnter += OnHitHandler);
        damageBoxes.ForEach(d => d.OnTriggerEnter += OnDamageHandler);
    }
    private void Dispose()
    {
        hitBoxes.ForEach(h => h.OnTriggerEnter -= OnHitHandler);
        damageBoxes.ForEach(d => d.OnTriggerEnter -= OnDamageHandler);
    }
    public void OnHitHandler(Collider2D collision)
    {

    }
    public void OnDamageHandler(Collider2D collision)
    {
        if (collision.TryGetComponent<CollisionDetector>(out var collisionDetector)) OnEntityHit?.Invoke(collisionDetector);
    }
}
