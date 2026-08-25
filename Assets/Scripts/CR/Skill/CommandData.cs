using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Command Data", menuName = "VRIS/CR/Skill/Create New Command")]
public class CommandData : ScriptableObject
{
    [SerializeField] private string commandName;
    [SerializeField] private float commandInputBufferTime = 0.5f;
    [SerializeField] private float priority = 0f;
    [SerializeField] private List<CommandKey> sequence;
    public string CommandName => commandName;
    public float CommandInputBufferTime => commandInputBufferTime;
    public float Priority => priority;
    public List<CommandKey> Sequence => sequence;
}
