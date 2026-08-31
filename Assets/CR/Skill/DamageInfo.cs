using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public Entity Caster;
    public float Damage;
    public DamageInfo(Entity Caster, float Damage)
    {
        this.Caster = Caster;
        this.Damage = Damage;
    }
}
