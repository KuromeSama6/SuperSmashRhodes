using System;
using System.Collections;
using SuperSmashRhodes.Battle;
using SuperSmashRhodes.FScript.Instruction;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.FScript {
/// <summary>
/// An EntityState represents an active action (driven by a FScript) that an entity is currently performing.
/// </summary>
public class EntityState {
    public UnityEvent<EntityState> onEnded { get; } = new();
    
    public Entity owner { get; private set; }
    public FScriptObject script { get; private set; }
    public FScriptRuntimeContext context { get; private set; }
    public int frame { get; private set; } = 0;
    public int currentAddress { get; private set; } = 0;

    public bool ended { get; private set; }
    private int interruptFrames = 0;
    private int queuedJump = -1;
    
    public EntityState(Entity owner, FScriptObject script) {
        this.owner = owner;
        this.script = script;
        
        context = new FScriptRuntimeContext(this);
    }

    public IEnumerator Begin() {
        if (script.main == null)
            throw new FScriptRuntimeException("Script has no main function.");
        
        frame = 0;
        currentAddress = script.main.address;
        
        return StateCoroutine();
    }

    private void TickState() {
        // do nothing if we have interrupt frames
        if (interruptFrames > 0) {
            interruptFrames--;
            return;
        }
        
        // execute all instructions in that frame
        int count = 0;
        while (interruptFrames <= 0) {
            ++count;
            if (count > 5000)
                throw new FScriptRuntimeException("More than 5000 instructions executed in one frame.");

            // queued jump
            if (queuedJump != -1) {
                currentAddress = queuedJump;
                queuedJump = -1;
            }
            
            var exe = script.addressRegistry.GetInstruction(currentAddress);
            ++currentAddress; // already next address
            
            // end if null
            if (exe == null) {
                EndImediately();
                break;
            }
            
            UpdateScriptConstants();
            
            // nop if block
            if (exe is FScriptProcedure proc) {
                proc.ProcedureInit(context);
                continue;
            }

            var instruction = (FInstruction)exe;
            try {
                context.WriteRegister(FScriptRegister.CURRENT_INSTRUCTION_PTR, exe.address);
                instruction.Execute(context);
                context.WriteRegister(FScriptRegister.LAST_INSTRUCTION_PTR, exe.address);
                
            } catch (Exception e) {
                Debug.LogError($"fscript: {owner.config.id}: {script.descriptor.id}: [0x{instruction.address:x8}] `{instruction.rawLine.raw}`: {e.Message}");
                Debug.LogException(e);
                EndImediately();
                return;
            }
        }
        
    }
    
    private IEnumerator StateCoroutine() {
        while (!ended) {
            TickState();
            if (ended) break;
            yield return new WaitForFixedUpdate();
        }
    }

    private void UpdateScriptConstants() {
        context.WriteConstant(FScriptConstant.GROUNDED, owner.isLogicallyGrounded);
        context.WriteConstant(FScriptConstant.PHY_GROUNDED, owner.isPhysicallyGrounded);
    }
    
    public void AddInterrupt(int frames) {
        interruptFrames += frames;
    }
    
    public void EndImediately() {
        if (ended) return;
        ended = true;
        onEnded.Invoke(this);
    }

    public void QueueJump(int address) {
        if (queuedJump != -1)
            throw new FScriptRuntimeException("Jump already queued.");
        queuedJump = address;
    }
    
    public void Dispose() {
        onEnded.RemoveAllListeners();
    }
    
    public static implicit operator bool(EntityState state) {
        return state != null;
    }
}
}
