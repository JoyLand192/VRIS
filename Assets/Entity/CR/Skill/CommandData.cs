using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Command Data", menuName = "VRIS/CR/Skills/Create New Command")]
public class CommandData : ScriptableObject
{
    [field: SerializeField] public string CommandName { get; private set; }
    [field: SerializeField] public float CommandInputBufferTime { get; private set; } = 0.5f;
    [field: SerializeField] public float Priority { get; private set; } = 0f;
    [field: SerializeField] public CommandInputRequirement InputRequirement { get; private set; }
    [field: SerializeField] public List<CommandKey> Sequence { get; private set; }
    [field: SerializeField] public List<CommandData> FollowUpCommands { get; private set; }
    [field: SerializeField] public Skill Skill { get; private set; }
}
