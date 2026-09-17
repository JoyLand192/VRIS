using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpriteEventChannel", menuName = "EventChannels/Sprite")]
public class SpriteEventChannel : ScriptableObject
{
    public event Action<Sprite> OnRaised;
    public void Raise(Sprite value) => OnRaised?.Invoke(value);
}
