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
    public CharacterConfiguration characterConfig;
    public CharacterDescriptor descriptor;
    public ComboDecayData comboDecayData;
    
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
    public float neutralAniTransitionOverride { get; set; } = 0.05f;
    
    private float airHitstunRotation = 0f;
    
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
        UpdateGravity();
        UpdateRotation();

        if (applyGroundedFrictionFrames > 0) {
            --applyGroundedFrictionFrames;
            groundedFrictionAlpha = Mathf.Lerp(groundedFrictionAlpha, 1, Time.fixedDeltaTime);
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, Time.fixedDeltaTime * 20f * groundedFrictionAlpha);
        }
    }

    private void UpdateGravity() {
        var ret = characterConfig.baseGravity;
        if (comboCounter.inCombo) {
            var decay = comboCounter.comboDecay;
            var data = opponent.comboDecayData;
            ret *= data.opponentGravityCurve.Evaluate(decay);
        }
        
        rb.gravityScale = ret;
    }

    private void UpdateRotation() {
        if (airborne && activeState is State_CmnHitStunAir) {
            airHitstunRotation = Mathf.Clamp(airHitstunRotation + 0.5f, -55f, 55f);

        } else {
            airHitstunRotation = Mathf.Lerp(airHitstunRotation, 0, Time.fixedDeltaTime * 10f);
        }
        
        // facing animation
        float facing = side == EntitySide.LEFT ? 0 : 180;
        
        var ea = rotationContainer.transform.localEulerAngles;
        ea.y = facing;
        ea.z = airHitstunRotation;
        rotationContainer.transform.localEulerAngles = ea;
    }
    
    private void UpdateInput() { 
        if (inputModule.localBuffer == null) return;
        
        // get priority sorted list
        var li = (from state in states.Values
            orderby ((CharacterState)state).inputPriority descending
            select (CharacterState)state).ToList();
        
        foreach (var state in li) {
            if (state == activeState) continue;
            
            if (activeState.stateData.cancelOptions.Contains(state) || BitUtil.CheckFlag((ulong)activeState.stateData.cancelFlag, (ulong)state.type)) {
                // state is valid
                if (state.IsInputValid(inputModule.localBuffer) && state.mayEnterState) {
                    // check cancel state
                
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
            if ((airborne && activeState is State_CmnHitStunAir) || (opponent.airborne && opponent.activeState is State_CmnHitStunAir)) return;
            
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

    protected override IAttack OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        base.OnOutboundHit(victim, data);
        // Debug.Log("outbound");
        
        if (victim is PlayerCharacter player) {
            return ProcessOutboundHit(player);
        }
        
        //TODO: Others 
        return null;
    }

    protected override void OnInboundHit(AttackData data) {
        base.OnInboundHit(data);
        ApplyStandardAttack(data);
    }

    private IAttack ProcessOutboundHit(PlayerCharacter to) {
        if (!(activeState is CharacterAttackStateBase move)) {
            // invalid attack state1
            return null;
        }

        if (to.activeState.fullyInvincible) return null;
        // reject if move has no active frames
        if (!move.hasActiveFrames) return null;
        // reject if move is not active
        if (move.phase != AttackPhase.ACTIVE) return null;
        
        // move.OnContact(to);
        return move;
    }
    
    private void ApplyStandardAttack(AttackData data) {
        // called same frame as OutboundHit
        var attack = data.attack;
        
        // reject if move has no active frames
        if (!attack.MayHit(this)) {
            return;
        }
        
        attack.OnContact(this);
        // hit/guard 
        bool blocked = !CheckAttackHit(data);

        // neutral check
        bool crouching = activeState is State_CmnNeutralCrouch || activeState is State_CmnBlockStunCrouch || activeState is State_CmnHitStunCrouch;
        
        // register hit
        var frameData = attack.GetFrameData(this);
        int framesRemaining = frameData.active + frameData.recovery;
        // Debug.Log($"inbound hit 1, neutral: {neutral}, blockheld: {blockHeld}, blockType: {blockType} blocked: {blocked}, framesRemaining: {framesRemaining}, blockstun {framesRemaining + move.frameData.onBlock}");
        ApplyGroundedFriction();
        if (blocked) {
            this.frameData.blockstunFrames = framesRemaining + frameData.onBlock;
            if (!activeState.type.HasFlag(EntityStateType.CHR_BLOCKSTUN)) BeginState(crouching ? "CmnBlockStunCrouch" : "CmnBlockStun");
            attack.OnBlock(this);
            
        } else {
            this.frameData.SetHitstunFrames(framesRemaining + frameData.onHit, Mathf.Max(frameData.total - attack.GetCurrentFrame(this), 0));
            if (!activeState.type.HasFlag(EntityStateType.CHR_HITSTUN)) BeginState(crouching ? "CmnHitStunGroundCrouch" : "CmnHitStunGround"); 
            comboCounter.RegisterAttack(attack, this);
            attack.OnHit(this);
            airHitstunRotation = 0f;
            
            // Debug.Log($"unscaled damage: {attack.GetUnscaledDamage(this)}, final scale: {comboCounter.finalScale}, final dmg {attack.GetUnscaledDamage(this) * comboCounter.finalScale}");
            {
                // damage
                var dmg = attack.GetUnscaledDamage(this) * comboCounter.finalScale * comboCounter.GetMoveSpecificProration(attack);
                
                // combo decay
                if (data.from is PlayerCharacter player) {
                    var decayData = player.comboDecayData;
                    if (comboCounter.inCombo) {
                        dmg *= decayData.extraProrationCurve.Evaluate(comboCounter.comboDecay);
                    }
                }
                
                health -= Mathf.Max(1, dmg);
            }
        }

        data.result = blocked ? AttackResult.BLOCKED : AttackResult.HIT;
        fxManager.NotifyHit(data);
        
        // pushback
        {
            var amount = attack.GetPushback(this, airborne);
            if (blocked) amount.y = 0f;
            
            if (data.from is PlayerCharacter player) {
                var decayData = player.comboDecayData;
                if (comboCounter.inCombo) {
                    amount *= new Vector2(
                        decayData.opponentBlowbackCurve.Evaluate(comboCounter.comboDecay), 
                        decayData.opponentLaunchCurve.Evaluate(comboCounter.comboDecay)
                        );
                }
            }
            
            ApplyCarriedPushback(amount);
        }
        
        // apply freeze frames

        var freezeFrames = attack.GetFreezeFrames(this);
        PhysicsTickManager.inst.Schedule(4, freezeFrames);
    }

    private bool CheckAttackHit(AttackData data) {
        var attack = data.attack;
        
        var blockType = attack.GetGuardType(this);
        bool crouching = activeState is State_CmnNeutralCrouch || activeState is State_CmnBlockStunCrouch;
        bool blockHeld = inputModule.localBuffer.thisFrame.HasInput(InputType.BACKWARD, InputFrameType.HELD);

        AttackGuardType currentGuardType = crouching ? AttackGuardType.CROUCHING : AttackGuardType.STANDING;
        
        if (blockType == AttackGuardType.THROW) {
            throw new NotImplementedException("Throw guard not implemented");
        }

        // incorrect block
        if (!BitUtil.CheckFlag((ulong)blockType, (ulong)currentGuardType))
            return true;
        
        // blockstun carryover
        if (BitUtil.CheckFlag((ulong)activeState.type, (ulong)EntityStateType.CHR_BLOCKSTUN))
            return false;

        // neutral
        if (blockHeld && (BitUtil.CheckFlag((ulong)activeState.type, (ulong)EntityStateType.CHR_NEUTRAL) || activeState is State_CmnMoveBackward)) {
            return false;
        }
        

        return true;
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
