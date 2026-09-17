using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRStatus : MonoBehaviour
{
    [field: SerializeField] public float HP { get; protected set; }
    private const float maxGuardGauge = 100f;
    [SerializeField] protected GuardStateInfoEventChannel guardStateEventChannel;
    private float guardGauge = maxGuardGauge;
    public float GuardGauge
    {
        get => guardGauge;
        set
        {
            guardGauge = Mathf.Clamp(value, 0, maxGuardGauge);
            UpdateGuardState();
        }
    }
    private bool isGuardBroken;
    public bool IsGuardBroken
    {
        get => isGuardBroken;
        set
        {
            isGuardBroken = value;
            UpdateGuardState();
        }
    }
    public float TempDamage => 5;
    public void ReceiveDamage(DamageInfo damageInfo) => HP -= damageInfo.Damage;
    public void UpdateGuardState()
    {
        guardStateEventChannel.Raise(new GuardStateInfo(GuardGauge / maxGuardGauge, IsGuardBroken));
    }
}
