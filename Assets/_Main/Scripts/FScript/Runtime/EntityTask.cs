using System;
using System.Collections.Generic;
using SuperSmashRhodes._Main.Scripts.FScript;
using SuperSmashRhodes.FScript.Instruction;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace SuperSmashRhodes.FScript {
public class EntityTask {
    public int frame { get; private set; }
    public bool isState { get; private set; }
    public int address { get; private set; }

    private FScriptRuntime runtime;
    private int queuedJumpAddress;
    public Stack<int> callStack { get; } = new();

    public bool isBusy => address > 0;
    private FInstruction currentInstruction => (FInstruction)runtime.script.addressRegistry.GetInstruction(address);
    
    public EntityTask(FScriptRuntime runtime) {
        this.runtime = runtime;
    }
    
    public void BeginState(FInstruction instruction, bool isState) {
        this.isState = isState;
        frame = 0;
        address = instruction.address;
    }

    public void TickState() {
        if (!isBusy) return;

        int count = 0;
        
        while (true) {
            if (!isBusy) break;
            
            // safety check
            ++count;
            if (count >= 5000)
                throw new FScriptRuntimeException("More than 5000 instructions executed in one frame");

            var instruction = (FInstruction)runtime.script.addressRegistry.GetInstruction(address);
            if (instruction == null)
                throw new FScriptRuntimeException($"access violation: {address:x8}");
            
            if (!isState && instruction is InterruptInstruction)
                throw new FScriptRuntimeException($"InterruptInstruction in non-state section");
            
            // execute
            try {
                runtime.WriteRegister(FScriptRegister.CURRENT_INSTRUCTION_PTR, instruction.address);
                // Debug.Log($"before: {instruction.address:x8} `{instruction.rawLine.raw}` (stack={string.Join(", ", callStack)})");
                instruction.Execute(runtime);
                // Debug.Log($"after: {instruction.address:x8} `{instruction.rawLine.raw}` (stack={string.Join(", ", callStack)})");
                runtime.WriteRegister(FScriptRegister.LAST_INSTRUCTION_PTR, instruction.address);
                
            } catch (Exception e) {
                Debug.LogError($"fscript: {runtime.owner.name}: [0x{instruction.address:x8}] `{instruction.rawLine.raw}`: {e.GetType()}: {e.Message}");
                Debug.LogException(e);
                ExitState();
                return;
            }
            
            // next
            var nextAddress = instruction.nextAddress;

            if (queuedJumpAddress > 0) {
                nextAddress = queuedJumpAddress;
                queuedJumpAddress = 0;
            }
            
            address = nextAddress;
            if (address == 0) {
                // block end
                ExitState();
                break;
            }
        }

    }

    // only called by instructions
    public void QueueJump(int address) {
        if (!runtime.script.addressRegistry.registry.TryGetValue(address, out var instruction))
            throw new FScriptRuntimeException($"access violation: jmp: {address:x8}");

        if (instruction is SectionInstruction section) {
            callStack.Push(currentInstruction.nextAddress);
        }
        
        // Debug.Log($"Queue jump {address:X}");
        queuedJumpAddress = address;
    }
    
    public void ExitState() {
        address = 0;
        frame = 0;
    }
    
}
}
