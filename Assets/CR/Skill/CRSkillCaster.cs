using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using TMPro;

[System.Flags]
public enum CommandInputRequirement
{
    None = 0,
    Forward = 1 << 0,
    Down = 1 << 1,
    Airborne = 1 << 2,
    Grounded = 1 << 3,
}
public class CRSkillCaster : MonoBehaviour
{
    private const float commandInputBufferTime = 0.5f;
    [SerializeField] private TextMeshProUGUI TEXT;
    [SerializeField] private List<CommandInputEntry> inputBuffer = new();
    [SerializeField] private List<CommandData> availableCommands = new();
    private CRInputHandler inputHandler;
    private CRMovement movement;
    private CRAnimator animator;
    private void Update()
    {
        var currentTime = Time.time;
        while (inputBuffer.Count > 0)
        {
            if (currentTime - inputBuffer[0].InputTime > commandInputBufferTime)
                inputBuffer.RemoveAt(0);
            else break;
        }
    }
    public void SortAvailableCommands()
    {
        availableCommands = availableCommands.OrderByDescending(c => c.Priority).ToList();
    }
    public void Initialize(CRInputHandler inputHandler, CRMovement movement, CRAnimator animator)
    {
        this.inputHandler = inputHandler;
        this.movement = movement;
        this.animator = animator;

        inputHandler.OnCommandKeyInput += OnCommandInput;
        SortAvailableCommands();
    }
    private static List<CommandKey> MirroredSequence(List<CommandKey> sequence) => sequence.Select(MirrorKey).ToList();
    private static CommandKey MirrorKey(CommandKey key)
    {
        return key switch
        {
            CommandKey.Left => CommandKey.Right,
            CommandKey.Right => CommandKey.Left,
            CommandKey.RightUp => CommandKey.LeftUp,
            CommandKey.RightDown => CommandKey.LeftDown,
            CommandKey.LeftUp => CommandKey.RightUp,
            CommandKey.LeftDown => CommandKey.RightDown,
            _ => key,
        };
    }
    public void OnCommandInput(CommandInputEntry inputEntry)
    {
        Debug.Log($"{(int)inputEntry.CommandKey} | {inputEntry.InputTime}");
        inputBuffer.Add(inputEntry);

        foreach (var commandEntry in availableCommands)
        {
            if (!CheckCommandInput(commandEntry)) continue;
            if (!CheckCommandInputRequirement(commandEntry)) continue;

            TEXT.text = $"{commandEntry.CommandName}";
            Debug.Log($"Command {commandEntry.CommandName} executed!");
            inputBuffer.Clear();
            break;
        }
    }
    private bool CheckCommandInput(CommandData command)
    {
        return CheckCommandInputWithSequence(command.Sequence, command) 
            || CheckCommandInputWithSequence(MirroredSequence(command.Sequence), command);
    }
    private bool CheckCommandInputWithSequence(List<CommandKey> sequence, CommandData commandData)
    {
        if (sequence.Count > inputBuffer.Count) return false;

        int progress = 1;
        int skippedIndex = 0;
        float latestInputTime = inputBuffer[inputBuffer.Count - 1].InputTime;

        while (sequence.Count - progress >= 0)
        {
            if (inputBuffer.Count - progress - skippedIndex < 0) return false;

            var currentInput = inputBuffer[inputBuffer.Count - progress - skippedIndex];
            if (latestInputTime - currentInput.InputTime > commandData.CommandInputBufferTime) return false;

            if (currentInput.CommandKey == CommandKey.Neutral)
            {
                skippedIndex++;
                continue;
            }

            if (currentInput.CommandKey != sequence[sequence.Count - progress]) return false;
            progress++;
        }
        return true;
    }
    private bool CheckCommandInputRequirement(CommandData commandData)
    {
        var requirement = commandData.InputRequirement;
        if (requirement == CommandInputRequirement.None) return true;
        if (requirement.HasFlag(CommandInputRequirement.Forward))
        {
            if (!inputHandler.IsHoldingLeft && !inputHandler.IsHoldingRight) return false;
        }
        if (requirement.HasFlag(CommandInputRequirement.Down))
        {
            if (!inputHandler.IsHoldingDown) return false;
        }
        if (requirement.HasFlag(CommandInputRequirement.Airborne))
        {
            if (movement.CurrentContact == CRMovement.SurfaceContact.GROUNDED) return false;
        }
        if (requirement.HasFlag(CommandInputRequirement.Grounded))
        {
            if (movement.CurrentContact != CRMovement.SurfaceContact.GROUNDED) return false;
        }
        return true;
    }
}
