using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FloatEventChannel", menuName = "EventChannels/float")]
public class FloatEventChannel : ScriptableObject
{
    public event Action<float> OnRaised;
    public void Raise(float value) => OnRaised?.Invoke(value);
}
