using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [field: SerializeField] public EnemyStatus Status { get; protected set; }
    public void ReceiveDamage(DamageInfo damageInfo)
    {
        Status.ReceiveDamage(damageInfo);
    }
}
