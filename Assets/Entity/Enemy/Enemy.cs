using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [field: SerializeField] public HitboxManager Hitbox { get; protected set; }
    [field: SerializeField] public EnemyStatus Status { get; protected set; }
    private void Awake()
    {
        Hitbox.Initialize(this);
    }
    public void ReceiveDamage(DamageInfo damageInfo)
    {
        Status.ReceiveDamage(damageInfo);
    }
}
