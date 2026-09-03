using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [field: SerializeField] public float HP { get; protected set; }
    public void ReceiveDamage(DamageInfo damageInfo) => HP -= damageInfo.Damage;
}
