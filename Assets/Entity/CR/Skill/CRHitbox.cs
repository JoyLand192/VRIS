using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRHitbox : MonoBehaviour
{
    [SerializeField] private List<CollisionDetector> hitBoxes;
    [SerializeField] private List<CollisionDetector> damageBoxes;
    public event System.Action<Enemy> OnEnemyHit;
    private void Awake()
    {
        Initialize();
    }
    private void OnDestroy()
    {
        Dispose();
    }
    private void Initialize()
    {
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
        if (collision.TryGetComponent<Enemy>(out var enemy)) OnEnemyHit?.Invoke(enemy);
    }
}
