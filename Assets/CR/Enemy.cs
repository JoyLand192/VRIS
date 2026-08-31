using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public void ReceiveDamage(DamageInfo damageInfo)
    {
        Debug.Log($"Ouch ({damageInfo.Damage})");
    }
}
