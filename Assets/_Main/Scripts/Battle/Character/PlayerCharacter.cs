using System;
using System.Linq;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
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
    [Title("References")]
    public CharacterDescriptor descriptor;
    public int playerIndex { get; private set; }
    public float moveDirection { get; private set; }
    public bool isDashing { get; private set; }
    public bool isCrouching { get; private set; }
    public bool airborne { get; set; }
    public PlayerCharacter opponent => GameManager.inst.GetOpponent(this);
    public float opponentDistance => Mathf.Abs(transform.position.x - opponent.transform.position.x);
    public PlayerInputModule inputModule { get; private set; }
    public ComboCounter comboCounter { get; private set; }
    public FrameDataRegister frameData { get; private set; }

    public UnityEvent onSideSwap { get; } = new();
    
    private int applyGroundedFrictionFrames = 0;
    public float groundedFrictionAlpha { get; set; }
    public PushboxManager pushboxManager { get; private set; }
    public CharacterFXManager fxManager { get; private set; }
    
    public void Init(int playerIndex) {
        this.playerIndex = playerIndex;
    }
    
    protected override void Start() {
        base.Start();
        comboCounter = new(this);
        frameData = new(this);
        
        inputModule = GetComponent<PlayerInputModule>();
        pushboxManager = GetComponentInChildren<PushboxManager>();
        fxManager = GetComponentInChildren<CharacterFXManager>();
        
        pushboxManager.onGroundContact.AddListener(OnGroundContact);
    }

    protected override void Update() {
        base.Update();
    }

    protected override void OnTick() {
        base.OnTick();
        frameData.Tick();
        UpdateInput();
        UpdateFacing();

        if (applyGroundedFrictionFrames > 0) {
            --applyGroundedFrictionFrames;
            groundedFrictionAlpha = Mathf.Lerp(groundedFrictionAlpha, 1, Time.fixedDeltaTime);
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, Time.fixedDeltaTime * 20f * groundedFrictionAlpha);
        }
    }

    private void UpdateInput() {
        if (inputModule.localBuffer == null) return;
        
        // get priority sorted list
        var li = (from state in states.Values
            orderby ((CharacterState)state).inputPriority descending
            select (CharacterState)state).ToList();
        
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
        return;
        applyGroundedFrictionFrames = frames;
    }

    public void ApplyGroundedFrictionImmediate() {
        applyGroundedFrictionFrames = 0;
        rb.linearVelocityX = 0f;
    }
    
    protected override EntityState GetDefaultState() {
        string name = inputModule.localBuffer.thisFrame.HasInput(InputType.DOWN, InputFrameType.HELD) ? "CmnNeutralCrouch" : "CmnNeutral";
        
        if (!EntityStateRegistry.inst.CreateInstance(name, out var ret, this))
            throw new Exception("Default state [CmnNeutral] not assigned");
        
        return ret;
    }

    public override void OnRoundInit() {
        base.OnRoundInit();
        // position
        
        // TODO: Z index management
        transform.position = new(playerIndex == 0 ? -1.5f : 1.5f, 0f, 0f);
    }

    public void SetZPriority() {
        animation.animation.GetComponent<MeshRenderer>().sortingOrder = 3;
        opponent.animation.animation.GetComponent<MeshRenderer>().sortingOrder = 2;
    }

    public override void BeginLogic() {
        base.BeginLogic();
        if (playerIndex == 0) SetZPriority();
    }

    protected override bool OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        base.OnOutboundHit(victim, data);

        if (victim is PlayerCharacter player) {
            return ProcessOutboundHit(player);
        }
        
        //TODO: Others 
        return false;
    }

    protected override void OnInboundHit(Entity attacker, EntityBBInteractionData data) {
        base.OnInboundHit(attacker, data);

        var state = attacker.activeState;
        if (state is IAttack attack) {
            ApplyStandardAttack(attacker, attack, data);
        }
    }

    private bool ProcessOutboundHit(PlayerCharacter to) {
        if (!(activeState is CharacterAttackStateBase move)) {
            // invalid attack state1
            return false;
        }
        
        // reject if move has no active frames
        if (!move.hasActiveFrames) return false;
        // reject if move is not active
        if (move.phase != AttackPhase.ACTIVE) return false;
        
        move.OnContact(to);
        return true;
    }
    
    private void ApplyStandardAttack(Entity from, IAttack attack, EntityBBInteractionData data) {
        // called same frame as OutboundHit
        
        // reject if move has no active frames
        if (!attack.MayHit(this)) {
            return;
        }
        
        // hit/guard 
        var blockType = attack.GetGuardType(this);
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
        var frameData = attack.GetFrameData(this);
        int framesRemaining = frameData.total - attack.GetCurrentFrame(this) - 1;
        // Debug.Log($"inbound hit 1, neutral: {neutral}, blockheld: {blockHeld}, blockType: {blockType} blocked: {blocked}, framesRemaining: {framesRemaining}, blockstun {framesRemaining + move.frameData.onBlock}");
        ApplyGroundedFriction();
        if (blocked) {
            this.frameData.blockstunFrames = framesRemaining + frameData.onBlock;
            BeginState(crouching ? "CmnBlockStunCrouch" : "CmnBlockStun");
            attack.OnBlock(this);
            
        } else {
            this.frameData.hitstunFrames = framesRemaining + frameData.onHit;
            BeginState(crouching ? "CmnHitStunGroundCrouch" : "CmnHitStunGround"); 
            comboCounter.RegisterAttack(attack, this);
            attack.OnHit(this);
            
            //TODO: Damage logic
            health -= attack.GetUnscaledDamage(this) * comboCounter.finalScale;
        }

        StandardAttackResult result = new() {
            attack = attack,
            result = blocked ? AttackResult.BLOCKED : AttackResult.HIT,
            from = from,
            to = this,
            interactionData = data
        };
        
        fxManager.NotifyHit(result);
        ApplyCarriedPushback(attack.GetPushback(this, airborne));
        
        // apply freeze frames
        if (comboCounter.count < 2) {
            PhysicsTickManager.inst.Schedule(4, attack.GetFreezeFrames(this));
        }
        
        attack.OnHitProcessed(this);
    }

    private void OnGroundContact() {
        if (airborne) airborne = false;
    }

    private void ApplyCarriedPushback(Vector2 vec) {
        float direction = side == EntitySide.LEFT ? -1 : 1;
        
        if (pushboxManager.atWall) {
            opponent.rb.AddForceX(vec.x * -direction, ForceMode2D.Impulse);
            opponent.groundedFrictionAlpha = 0;
            // Debug.Log("add opponent force");
            
        } else {
            rb.AddForceX(vec.x * direction, ForceMode2D.Impulse);
            groundedFrictionAlpha = 0;
        }
        
        // y force
        // Debug.Log(vec);
        rb.linearVelocityY = 0;
        rb.AddForceY(vec.y, ForceMode2D.Impulse);
        
    }
    
}

public abstract class RuntimeCharacterDataRegister {
    public PlayerCharacter owner { get; private set; }
    public RuntimeCharacterDataRegister(PlayerCharacter owner) {
        this.owner = owner;
    }
}
}
