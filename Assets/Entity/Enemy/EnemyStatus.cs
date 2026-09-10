using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{
    [SerializeField] private FloatEventChannel TEMPbossHealthBarChannel;
    [field: SerializeField] public float MaxHP { get; protected set; } = 500;
    [SerializeField] private float hp;
    public float HP
    {
        get => hp;
        protected set
        {
            hp = value;
            TEMPbossHealthBarChannel.Raise(hp / MaxHP);
        }
    }
    private void Awake()
    {
        hp = MaxHP;
    }
    public void ReceiveDamage(DamageInfo damageInfo) => HP -= damageInfo.Damage;
}
