using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GuardStateInfoEventChannel", menuName = "EventChannels/GuardStateInfo")]
public class GuardStateInfoEventChannel : ScriptableObject
{
    public event Action<GuardStateInfo> OnRaised;
    public void Raise(GuardStateInfo value) => OnRaised?.Invoke(value);
}
