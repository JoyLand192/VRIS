using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Skill : ScriptableObject
{
    [field: SerializeField] public string SkillName { get; protected set; }
    [field: SerializeField] public bool MovementBlock { get; protected set; } = true;
    public abstract UniTask Execute(CR cr);
    public virtual bool CheckCondition(CR cr) => true;
}
