using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
    private CR cr;
    [SerializeField] private TextMeshProUGUI TEXT;
    [SerializeField] private List<CommandInputEntry> inputBuffer = new();
    [SerializeField] private List<CommandData> availableCommands = new();
    [SerializeField] private CommandData currentCommand;
    [SerializeField] private bool isCancelable;
    [SerializeField] private bool isSkillCasting;
    public bool IsSkillCasting
    {
        get => isSkillCasting;
        set
        {
            isSkillCasting = value;
        }
    }
    [SerializeField] private bool isMovementBlocked;
    public bool IsMovementBlocked
    {
        get => isMovementBlocked;
        set
        {
            if (value == isMovementBlocked) return;

            isMovementBlocked = value;
            cr.Movement.AddMoveBlocker(value ? 1 : -1);
        }
    }
    [SerializeField] private bool isDashBlocked;
    public bool IsDashBlocked
    {
        get => isDashBlocked;
        set
        {
            if (value == isDashBlocked) return;

            isDashBlocked = value;
            cr.Movement.AddDashBlocker(value ? 1 : -1);
        }
    }
    [SerializeField] private bool isDirectionBlocked;
    public bool IsDirectionBlocked
    {
        get => isDirectionBlocked;
        set
        {
            if (value == isDirectionBlocked) return;

            isDirectionBlocked = value;
            cr.Movement.AddDirectionBlocker(value ? 1 : -1);
        }
    }
    public System.Action<Skill> OnSkillExecute;
    public System.Action OnSkillEnd;
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
    public void SortAvailableCommands()
    {
        availableCommands = availableCommands.OrderByDescending(c => c.Priority).ToList();
    }
    public void Initialize(CR cr)
    {
        this.cr = cr;

        cr.Movement.OnLanded += OnLandedHandler;
        cr.InputHandler.OnCommandKeyInput += OnCommandInput;
        cr.Animator.OnCancelWindowOpen += OnCancelWindowOpenHandler;
        cr.Animator.OnSkillEnd += EndSkill;

        SortAvailableCommands();
    }
    public void OnCommandInput(CommandInputEntry inputEntry)
    {
        //Debug.Log($"{(int)inputEntry.CommandKey} | {inputEntry.InputTime}");
        inputBuffer.Add(inputEntry);

        if (currentCommand != null && isCancelable)
        {
            foreach (var cancelCommandEntry in currentCommand.FollowUpCommands)
            {
                if (!CheckCommandInput(cancelCommandEntry)) continue;
                if (!CheckCommandInputRequirement(cancelCommandEntry)) continue;

                EndSkill();
                CastSkill(cancelCommandEntry);
                return;
            }
        }

        if (IsSkillCasting) return;
        foreach (var commandEntry in availableCommands)
        {
            if (!CheckCommandInput(commandEntry)) continue;
            if (!CheckCommandInputRequirement(commandEntry)) continue;

            CastSkill(commandEntry);
            break;
        }
    }
    private void CastSkill(CommandData commandEntry)
    {
        if (cr.Movement.IsDashing) return;

        TEXT.text = $"{commandEntry.CommandName}";
        currentCommand = commandEntry;
        Debug.Log($"Command {commandEntry.CommandName} executed!");

        Debug.Assert(commandEntry.Skill != null, $"bro didn't made skill for command {commandEntry.CommandName}");

        IsSkillCasting = true;

        if (commandEntry.Skill.MovementBlock) IsMovementBlocked = true;
        if (commandEntry.Skill.DashBlock) IsDashBlocked = true;
        IsDirectionBlocked = true;

        OnSkillExecute?.Invoke(commandEntry.Skill);

        inputBuffer.Clear();
    }
    private void OnCancelWindowOpenHandler() => isCancelable = true;
    private void OnLandedHandler()
    {
        if (currentCommand == null) return;
        EndSkill();
    }
    private void EndSkill()
    {
        if (currentCommand.Skill.MovementBlock) IsMovementBlocked = false;
        if (currentCommand.Skill.DashBlock) IsDashBlocked = false;
        IsDirectionBlocked = false;

        currentCommand = null;
        isCancelable = false;
        IsSkillCasting = false;

        cr.Animator.StopSkill();
        OnSkillEnd?.Invoke();
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
        float latestInputTime = inputBuffer[^1].InputTime;

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
            if (!cr.InputHandler.IsHoldingLeft && !cr.InputHandler.IsHoldingRight) return false;
        }
        if (requirement.HasFlag(CommandInputRequirement.Down))
        {
            if (!cr.InputHandler.IsHoldingDown) return false;
        }
        if (requirement.HasFlag(CommandInputRequirement.Airborne))
        {
            if (cr.Movement.CurrentContact == CRMovement.SurfaceContact.GROUNDED) return false;
        }
        if (requirement.HasFlag(CommandInputRequirement.Grounded))
        {
            if (cr.Movement.CurrentContact != CRMovement.SurfaceContact.GROUNDED) return false;
        }
        return true;
    }
}
