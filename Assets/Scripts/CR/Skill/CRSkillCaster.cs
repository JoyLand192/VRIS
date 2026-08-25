using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

public class CRSkillCaster : MonoBehaviour
{
    private const float commandInputBufferTime = 0.5f;
    [SerializeField] private List<CommandInputEntry> inputBuffer = new();
    [SerializeField] private List<CommandData> availableCommands = new();
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
    public void Initialize(CRInputHandler inputHandler)
    {
        inputHandler.OnCommandKeyInput += OnCommandInput;
        SortAvailableCommands();
    }
    public void OnCommandInput(CommandInputEntry inputEntry)
    {
        Debug.Log($"{(int)inputEntry.CommandKey} | {inputEntry.InputTime}");
        inputBuffer.Add(inputEntry);

        foreach (var commandEntry in availableCommands)
        {
            if (CheckCommandInput(commandEntry))
            {
                Debug.Log($"Command {commandEntry.CommandName} executed!");
                inputBuffer.Clear();
                break;
            }
        }
    }
    private bool CheckCommandInput(CommandData commandData)
    {
        var sequence = commandData.Sequence;
        if (sequence.Count > inputBuffer.Count) return false;

        int progress = 1;
        int skippedIndex = 0;

        while (sequence.Count - progress >= 0 && inputBuffer.Count - progress - skippedIndex >= 0)
        {
            var currentInput = inputBuffer[inputBuffer.Count - progress - skippedIndex];
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
}
