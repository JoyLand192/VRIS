using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRStatus : MonoBehaviour
{
    [field: SerializeField] public float HP { get; protected set; }
    public float TempDamage => 5;
}
