using System;
using System.Linq;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle {
public class PlayerCharacter : Entity {
    public int playerIndex { get; private set; }
    public float moveDirection { get; private set; }
    public bool isDashing { get; private set; }
    public bool isCrouching { get; private set; }
    public bool airborne { get; set; }
    public PlayerCharacter opponent => GameManager.inst.GetOpponent(this);
    public PlayerInputModule inputModule { get; private set; }
    public ComboCounter comboCounter { get; private set; }
    public FrameDataRegister frameData { get; private set; }

    public UnityEvent onSideSwap { get; } = new();
    
    private int applyGroundedFrictionFrames = 0;
    private PushboxManager pushboxManager;
    
    public void Init(int playerIndex) {
        this.playerIndex = playerIndex;
    }
    
    protected override void Start() {
        base.Start();
        comboCounter = new(this);
        frameData = new(this);
        
        inputModule = GetComponent<PlayerInputModule>();
        pushboxManager = GetComponentInChildren<PushboxManager>();
        
        pushboxManager.onGroundContact.AddListener(OnGroundContact);
    }

    protected override void Update() {
        base.Update();
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        frameData.Tick();
        UpdateInput();
        UpdateFacing();

        if (applyGroundedFrictionFrames > 0) {
            --applyGroundedFrictionFrames;
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, Time.fixedDeltaTime * 20f);
        }
    }
    
    private void UpdateInput() {
        if (inputModule.localBuffer == null) return;
        
        // get priority sorted list
        var li = (from state in states.Values
            orderby state.inputPriority descending
            select state).ToList();
        
        foreach (var state in li) {
            if (state == activeState) continue;
            
            if (state.IsInputValid(inputModule.localBuffer) && state.mayEnterState) {
                // check cancel state
                if (activeState.stateData.cancelOptions.Contains(state) || BitUtil.CheckFlag((ulong)activeState.stateData.cancelFlag, (ulong)state.type)) {
                    // state is valid
                    BeginState(state);
                    break;
                }
            }
        }
        
    }

    private void UpdateFacing() {
        float pos = transform.position.x;
        float opponentPos = opponent.transform.position.x;
        var side = this.side;
        
        if (pos < opponentPos) {
            side = EntitySide.LEFT;
            
        } else if (pos > opponentPos) {
            side = EntitySide.RIGHT;
        }

        if (side != this.side) {
            if (activeState != null && activeState.stateData.disableSideSwap)
                return;
            if (Mathf.Abs(pos - opponentPos) < pushboxManager.pushbox.size.x)
                return;
            
            this.side = side;
            onSideSwap.Invoke();
        }
        
    }
    
    public void ApplyGroundedFriction(int frames = 1) {
        applyGroundedFrictionFrames = frames;
    }
    
    protected override EntityState GetDefaultState() {
        if (!EntityStateRegistry.inst.CreateInstance("CmnNeutral", out var ret, this))
            throw new Exception("Default state [CmnNeutral] not assigned");
        
        return ret;
    }

    public override void OnRoundInit() {
        base.OnRoundInit();
        // position
        
        // TODO: Z index management
        transform.position = new(playerIndex == 0 ? -1.5f : 1.5f, 0f, playerIndex);
    }

    protected override bool OnOutboundHit(Entity victim) {
        base.OnOutboundHit(victim);

        if (victim is PlayerCharacter player) {
            return ProcessOutboundHit(player);
        }
        
        //TODO: Others
        return false;
    }

    protected override void OnInboundHit(Entity attacker) {
        base.OnInboundHit(attacker);
        if (attacker is PlayerCharacter player) ProcessInboundHitPost(player);
    }

    private bool ProcessOutboundHit(PlayerCharacter to) {
        if (!(activeState is AttackStateBase move)) {
            // invalid attack state1
            return false;
        }
        
        // reject if move has no active frames
        if (!move.hasActiveFrames) return false;
        // reject if move is not active
        if (move.phase != AttackPhase.ACTIVE) return false;
        
        move.OnHit(to);
        return true;
    }
    
    private void ProcessInboundHitPost(PlayerCharacter from) {
        // called same frame as OutboundHit
        // get state of other player 
        var state = from.activeState;
        if (!(state is AttackStateBase move)) {
            // invalid attack state
            return;  
        }

        // reject if move has no active frames
        // hit/guard
        var blockType = move.attackProperties.guardType;
        bool blocked = false;

        // neutral check
        bool neutral = BitUtil.CheckFlag((ulong)activeState.type, (ulong)EntityStateType.CHR_NEUTRAL) || activeState is State_CmnMoveBackward;
        bool crouching = activeState is State_CmnNeutralCrouch || activeState is State_CmnBlockStunCrouch;
        bool blockHeld = inputModule.localBuffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD);

        AttackGuardType currentGuardType = crouching ? AttackGuardType.CROUCHING : AttackGuardType.STANDING;
        
        if (blockType == AttackGuardType.THROW) {
            throw new NotImplementedException("Throw guard not implemented");
            
        } else if (blockType != AttackGuardType.NONE) {
            if (neutral && blockHeld && BitUtil.CheckFlag((ulong)blockType, (ulong)currentGuardType)) {
                blocked = true; 
            }
        }
        
        // register hit
        int framesRemaining = move.frameData.total - move.frame - 1;
        // Debug.Log($"inbound hit 1, neutral: {neutral}, blockheld: {blockHeld}, blockType: {blockType} blocked: {blocked}, framesRemaining: {framesRemaining}, blockstun {framesRemaining + move.frameData.onBlock}");
        if (blocked) {
            frameData.blockstunFrames = framesRemaining + move.frameData.onBlock;
            BeginState(crouching ? "CmnBlockStunCrouch" : "CmnBlockStun");
            
        } else {
            frameData.hitstunFrames = framesRemaining + move.frameData.onHit;
            
        }
        
    }

    private void OnGroundContact() {
        if (airborne) airborne = false;
    }

}

public abstract class RuntimeCharacterDataRegister {
    public PlayerCharacter owner { get; private set; }
    public RuntimeCharacterDataRegister(PlayerCharacter owner) {
        this.owner = owner;
    }
}
}
