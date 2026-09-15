using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IntEventChannel", menuName = "EventChannels/int")]
public class IntEventChannel : ScriptableObject
{
    public event Action<int> OnRaised;
    public void Raise(int value) => OnRaised?.Invoke(value);
}
