using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [field: SerializeField] public HitboxManager Hitbox { get; protected set; }
    [field: SerializeField] public EnemyStatus Status { get; protected set; }
    [field: SerializeField] public EnemyVFX VFX { get; protected set; }
    private void Awake()
    {
        Hitbox.Initialize(this);

        Hitbox.OnEntityHit += (collisionDetector) =>
        {
            if (collisionDetector.Owner is not CR cr) return;

            var result = cr.ReceiveDamage(new DamageInfo(this, 5));
            if (result == DamageReceiveResult.Hit) TimeManager.Instance.HitStop(0.2f);
            else TimeManager.Instance.HitStop(0.1f);
        };
    }
    public void ReceiveDamage(DamageInfo damageInfo)
    {
        Status.ReceiveDamage(damageInfo);
        VFX.HurtFlash();
    }
}
