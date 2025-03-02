using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Spine.Unity;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.Postprocess;
using SuperSmashRhodes.Battle.Serialization;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Character.Gauge;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Network.RoomManagement;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.UI.Battle;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;
using UnityEngine;
using UnityEngine.Events;

namespace SuperSmashRhodes.Battle {
public class PlayerCharacter : Entity {
    [Title("References")]
    public CharacterConfiguration characterConfig;
    public CharacterDescriptor descriptor;
    public ComboDecayData comboDecayData;
    public Transform cameraFollowSocket;

    public int playerIndex { get; private set; }
    public float moveDirection { get; private set; }
    public bool isDashing { get; private set; }
    public bool isCrouching { get; private set; }
    public bool airborne { get; set; }
    public bool airActionPerformed { get; set; }
    public PlayerCharacter opponent => GameManager.inst.GetOpponent(this);
    public float opponentDistance => Mathf.Abs(transform.position.x - opponent.transform.position.x);
    public ComboCounter comboCounter { get; private set; }
    public FrameDataRegister frameData { get; private set; }

    public UnityEvent onSideSwap { get; } = new();
    public UnityEvent onRoundInit { get; } = new();
    public UnityEvent onLand { get; } = new();
    
    private int applyGroundedFrictionFrames = 0;
    public float groundedFrictionAlpha { get; set; }
    public PushboxManager pushboxManager { get; private set; }
    public CharacterFXManager fxManager { get; private set; }
    public float neutralAniTransitionOverride { get; set; } = 0.05f;
    public PlayerMeterGauge meter { get; private set; }
    public PlayerBurstGauge burst { get; private set; }
    public CharacterStateFlag stateFlags { get; set; }
    public float pushboxCorrectionGraceAmount { get; set; }
    public int airOptions { get; set; }
    public IInputProvider inputProvider { get; private set; } = new NOPInputProvider(); // InputProvider assigned on round start
    public List<CharacterAttackStateBase> gatlingMovesUsed { get; } = new();

    public bool atWall => pushboxManager.atWall;
    public bool dead => activeState is State_SysDeath;
    public float wallDistance {
        get {
            if (side == EntitySide.RIGHT) {
                // right wall
                var wallWidth = GameManager.inst.rightWall.GetComponent<BoxCollider2D>().size.x;
                return GameManager.inst.rightWall.transform.position.x - transform.position.x - wallWidth + pushboxManager.physicsBox.size.x / 2f;
                
            } else {
                // left wall
                var wallWidth = GameManager.inst.leftWall.GetComponent<BoxCollider2D>().size.x;
                return transform.position.x - GameManager.inst.leftWall.transform.position.x - wallWidth + pushboxManager.physicsBox.size.x / 2f;
            }
        }
    }
    public bool burstDisabled => stateFlags.HasFlag(CharacterStateFlag.DISABLE_BURST);
    public float cameraGroupWeight {
        get {
            if (stateFlags.HasFlag(CharacterStateFlag.NO_CAMERA_WEIGHT)) {
                return 0;
            }
            if (activeState == null) return 1f;
            return 1f + activeState.stateData.cameraData.cameraWeightModifier;
        }
    }
    
    public float airHitstunRotation { get; set; } = 0f;
    private float yRotationTarget = 0f;
    public int backdashCooldown { get; set; }
    private EntityState facingCheckCurrentState;

    public float gutsDamageModifier {
        get {
            var percentage = health / config.health;
            if (percentage > .7f) return 1f;
            var guts = characterConfig.guts;

            if (percentage <= .1f) return .56f - guts * .03f;
            if (percentage <= .2f) return .66f - guts * .03f;
            if (percentage <= .3f) return .75f - guts * .02f;
            if (percentage <= .4f) return .84f - guts * .02f;
            if (percentage <= .5f) return .89f - guts * .02f;
            if (percentage <= .6f) return .92f - guts * .01f;
            return .97f - guts * .01f;
        }
    }

    public override bool shouldTickAnimation {
        get {
            if (stateFlags.HasFlag(CharacterStateFlag.PAUSE_ANIMATIONS)) return false;
            return true;
        }
    }
    
    public override bool shouldTickState {
        get {
            if (stateFlags.HasFlag(CharacterStateFlag.PAUSE_STATE)) return false;
            return true;
        }
    }

    public void Init(int playerIndex) {
        this.playerIndex = playerIndex;
    }
    
    public void ResetAirOptions() {
        airOptions = characterConfig.airOptionsFinal;
        if (burst.driveRelease) ++airOptions;
    }

    public void ResetGatlings() {
        gatlingMovesUsed.Clear();
    }

    public override void Init() {
        base.Init();
        owner = this;
        comboCounter = new(this);
        frameData = new(this);
        
        pushboxManager = GetComponentInChildren<PushboxManager>();
        fxManager = GetComponentInChildren<CharacterFXManager>();
        meter = GetComponent<PlayerMeterGauge>();
        burst = GetComponent<PlayerBurstGauge>();
        
        pushboxManager.onGroundContact.AddListener(OnGroundContact);
    }

    public override void ManualUpdate() {
        base.ManualUpdate();
        
        if (!GameManager.inst.inGame) {
            transform.position = new(playerIndex == 0 ? -1.5f : 1.5f, 0f, 0f);
            return;
        }
    }

    protected override void OnTick() {
        base.OnTick();
        frameData.Tick();
        UpdateInput();
        UpdateFacing();
        UpdatePosition();
        UpdateGravity();
        UpdateRotation();
        inputProvider = InputDevicePool.inst.GetInputProvider(this);
        if (backdashCooldown > 0) {
            --backdashCooldown;
        }

        if (applyGroundedFrictionFrames > 0) {
            --applyGroundedFrictionFrames;
            groundedFrictionAlpha = Mathf.Lerp(groundedFrictionAlpha, 1, Time.fixedDeltaTime);
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, Time.fixedDeltaTime * 20f * groundedFrictionAlpha);
        }
        
        // death
        if (GameManager.inst.inGame && health <= 0 && !RoomManager.current.config.isTraining && !(activeState is State_SysDeath) && !stateFlags.HasFlag(CharacterStateFlag.DEATH_HOLD)) {
            BeginState("SysDeath");
            stateFlags |= CharacterStateFlag.NO_NEW_STATE;
            GameManager.inst.HandlePlayerDeath(this);
            comboCounter.Reset();
        }
        
    }

    private void UpdateGravity() {
        var ret = characterConfig.baseGravityFinal;
        if (comboCounter.inCombo) {
            var decay = comboCounter.comboDecay;
            // Debug.Log(decay);
            var data = opponent.comboDecayData;
            // Debug.Log($"{playerIndex} {decay}");
            ret *= data.opponentGravityCurve.Evaluate(decay);
        }

        // Debug.Log($"p{playerIndex} {activeState.stateData.gravityScale}");
        ret *= activeState.stateData.gravityScale;
        
        if (stateFlags.HasFlag(CharacterStateFlag.PAUSE_PHYSICS)) {
            ret = 0f;
        }
        
        if (activeState is State_CmnAirDash) {
            ret = 0f;
        }
        
        rb.gravityScale = ret;
    }

    public void UpdateRotation() {
        if (airborne && activeState is State_CmnHitStunAir) {
            airHitstunRotation = Math.Min(airHitstunRotation + (airHitstunRotation >= 0 ? .5f : -1f), 55f);

        } else {
            airHitstunRotation = Mathf.Lerp(airHitstunRotation, 0, Time.fixedDeltaTime * 10f);
        }
        
        // facing animation
        float facing = side == EntitySide.LEFT ? 0 : 180; 
        yRotationTarget = Mathf.Lerp(yRotationTarget, facing, Time.deltaTime * 20f);
        
        var ea = rotationContainer.transform.localEulerAngles;
        ea.y = yRotationTarget;
        ea.z = airHitstunRotation;
        rotationContainer.transform.localEulerAngles = ea;

        {
            // pushbox correction grace
            rotationContainer.transform.localPosition = new Vector3(pushboxCorrectionGraceAmount, 0, 0);
            pushboxCorrectionGraceAmount = Mathf.Lerp(pushboxCorrectionGraceAmount, 0, Time.fixedDeltaTime * 20f);
        }
        
    }
    
    private void UpdateInput() { 
        if (inputProvider.inputBuffer == null) return;
        if (stateFlags.HasFlag(CharacterStateFlag.PAUSE_INPUT) || GameManager.inst.globalStateFlags.HasFlag(CharacterStateFlag.PAUSE_INPUT)) return;
        
        // get priority sorted list
        var li = (from state in states.Values
            orderby ((CharacterState)state).inputPriority descending
            select (CharacterState)state).ToList();
        
        foreach (var state in li) {
            if (state == activeState && !activeState.isSelfCancellable) continue;
            
            if (activeState.stateData.cancelOptions.Contains(state) || BitUtil.CheckFlag((ulong)activeState.stateData.cancelFlag, (ulong)state.type)) {
                
                // state is valid
                if (state.mayEnterState && state.IsInputValid(inputProvider.inputBuffer)) {
                    // check cancel state
                    BeginState(state);
                    break;
                }
            }
            
        }
        
    }

    private void UpdatePosition() {
        var x = transform.position.x;
        
        if (x <= GameManager.inst.leftWall.transform.position.x || x >= GameManager.inst.rightWall.transform.position.x) {
            rb.linearVelocityX = 0;
            Vector3 offset = new((side == EntitySide.LEFT ? 1 : -1) * .4f, 0, 0);
            
            transform.position = GameManager.inst.ClampPositionToStage(transform.position) + offset;
        }
    }
    
    public void UpdateFacing() {
        float pos = transform.position.x;
        float opponentPos = opponent.transform.position.x;
        var side = this.side;
        
        if (pos < opponentPos) {
            side = EntitySide.LEFT;
            
        } else if (pos > opponentPos) {
            side = EntitySide.RIGHT;
        }

        if (side != this.side) {
            if (activeState != null && activeState.stateData.disableSideSwap) return;
            if (Mathf.Abs(pos - opponentPos) < pushboxManager.correctionBox.size.x) return;
            
            // Debug.Log($"{playerIndex} {opponent.transform.position.y} {transform.position.y + pushboxManager.correctionBox.size.y}");
            
            if (opponent.transform.position.y >= transform.position.y + pushboxManager.correctionBox.size.y) return;
            if ((airborne && activeState is State_CmnHitStunAir)) return;

            if (facingCheckCurrentState != activeState || facingCheckCurrentState == null) {
                facingCheckCurrentState = activeState;
            } else if (activeState != null && !activeState.stateData.maySwitchSides) {
                return;
            }
            
            if (airborne) return;
            
            this.side = side;
            onSideSwap.Invoke();
        }
        
    }
    
    public void ApplyGroundedFriction(int frames = 1) {
        // applyGroundedFrictionFrames = frames;
    }

    public void ApplyGroundedFrictionImmediate() {
        applyGroundedFrictionFrames = 0;
        rb.linearVelocityX = 0f;
    }
    
    public override EntityState GetDefaultState() {
        string name;
        if (airborne) {
            name = "CmnAirNeutral";
        } else {
            name = inputProvider.inputBuffer.thisFrame.HasInput(side, InputType.DOWN, InputFrameType.HELD) ? "CmnNeutralCrouch" : "CmnNeutral";
        } 
        
        // if (!EntityStateRegistry.inst.CreateInstance(name, out var ret, this))
        //     throw new Exception("Default state [CmnNeutral] not assigned");
        //
        // return ret;
        return states[name];
    }

    public override void OnRoundInit() {
        base.OnRoundInit();
        GameStateManager.inst.RefreshComponentReferences();
        // position
        
        // TODO: Z index management
        transform.position = new(playerIndex == 0 ? -1.5f : 1.5f, 0f, 0f);
        onRoundInit.Invoke();
        airActionPerformed = false;
        
        // input provider
        inputProvider = InputDevicePool.inst.GetInputProvider(this);
        
        ResetAirOptions();
        
        side = playerIndex == 0 ? EntitySide.LEFT : EntitySide.RIGHT;
        ForceUpdateColliders();
    }

    public void SetZPriority() {
        animation.animation.GetComponent<MeshRenderer>().sortingOrder = 3;
        opponent.animation.animation.GetComponent<MeshRenderer>().sortingOrder = 2;
    }

    public void ForceSetAirborne() {
        // Debug.Log($"force set, a={airborne}");
        if (airborne) return;
        transform.position += new Vector3(0, .5f, 0);
    }
    
    public override void BeginLogic() {
        base.BeginLogic();
        if (playerIndex == 0) {
            SetZPriority();
        }
    }

    public void ForceUpdateColliders() {
        gameObject.SetActive(false);
        gameObject.SetActive(true);

        foreach (var collider in GetComponentsInChildren<Collider2D>()) {
            var enabled = collider.enabled;
            collider.enabled = !enabled;
            collider.enabled = enabled;
        }

    }

    protected override IAttack OnOutboundHit(Entity victim, EntityBBInteractionData data) {
        base.OnOutboundHit(victim, data);
        
        if (victim is PlayerCharacter player) {
            return ProcessOutboundHit(player);
        }
        
        //TODO: Others 
        return null;
    }

    protected override void OnInboundHit(AttackData data) {
        base.OnInboundHit(data);

        if (!data.attack.GetSpecialProperties(this).HasFlag(AttackSpecialProperties.IGNORE_INVINCIBILITY)) {
            if (activeState.invincibility.HasFlag(data.attack.attackType)) return;   
        }
        
        ApplyStandardAttack(data);
    }

    private IAttack ProcessOutboundHit(PlayerCharacter to) {
        if (!(activeState is CharacterAttackStateBase move)) {
            // invalid attack state1
            return null;
        }
        
        // reject if move has no active frames
        if (!move.hasActiveFrames) return null;
        // Debug.Log($"move {move.phase} {move.frame}"); 
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
        var hitstate = ((CharacterState)activeState).hitstate;
        
        // register hit
        var frameData = attack.GetFrameData(this);
        bool addFreezeFrames = true;
        bool knockedDown = activeState.type.HasFlag(EntityStateType.CHR_HARD_KNOCKDOWN);
        
        opponent.SetZPriority();
        
        // Debug.Log($"inbound hit 1, neutral: {neutral}, blockheld: {blockHeld}, blockType: {blockType} blocked: {blocked}, framesRemaining: {framesRemaining}, blockstun {framesRemaining + move.frameData.onBlock}");
        ApplyGroundedFriction();
        
        var modifierFlag = ((CharacterState)activeState).OnHitByOther(data);
        if (modifierFlag.HasFlag(InboundHitModifier.STOP_ATTACK)) {
            return;
        }

        var armor = modifierFlag.HasFlag(InboundHitModifier.NO_STUN);
        if (armor) {
            audioManager.PlaySound("cmn/battle/sfx/generic/generic_armor_success");
            activeState.stateData.extraIndicatorFlag |= StateIndicatorFlag.ARMOR;
        }
        
        if (blocked) {
            this.frameData.blockstunFrames = attack.GetStunFrames(this, true);
            bool inBlockstun = activeState.type.HasFlag(EntityStateType.CHR_BLOCKSTUN);

            if (!inBlockstun) {
                if (airborne) BeginState("CmnBlockStunAir");
                else BeginState(crouching ? "CmnBlockStunCrouch" : "CmnBlockStun");
            }
            // Debug.Log("blocked");
            attack.OnBlock(this);
            
            // burst penalty
            {
                burst.AddDeltaTotal(inBlockstun ? -5.3f : -8.5f, 90);   
            }

            var rawDamage = attack.GetUnscaledDamage(this);
            var chipDamage = attack.GetChipDamagePercentage(this);
            if (opponent.burst.driveRelease) chipDamage = Mathf.Clamp01(chipDamage + .05f);
            
            if (chipDamage > 0) ApplyDamage(rawDamage * chipDamage, data, DamageProperties.SKIP_REGISTER, true);

            foreach (var summon in summons.ToArray()) {
                if (summon is Token token) {
                    if (token.flags.HasFlag(TokenFlag.DESTROY_ON_OWNER_BLOCK)) {
                        DestroySummon(token);
                    }
                }
            }

            meter.AddMeter(1f);
            
        } else {
            // this.frameData.SetHitstunFrames(framesRemaining + frameData.onHit, Mathf.Max(frameData.total - attack.GetCurrentFrame(this), 0));
            
            // counterhit hit state
            if (!armor) {
                this.frameData.SetHitstunFrames(attack.GetStunFrames(this, false), 0);
            
                // hit state select
                HandleOnHitStateTransition(attack, crouching, out addFreezeFrames);   
            }
            
            float hitStateDamageMultiplier = 1f;
            if (hitstate == Hitstate.COUNTER) {
                opponent.activeState.stateData.extraIndicatorFlag |= StateIndicatorFlag.COUNTER;
                hitStateDamageMultiplier = 1.1f;
                
            } else {
                
            }
            
            if (hitstate == Hitstate.PUNISH) {
                opponent.activeState.stateData.extraIndicatorFlag |= StateIndicatorFlag.PUNISH;
                hitStateDamageMultiplier = 1.05f;
            }
            
            // Debug.Log($"{activeState} {activeState.stateData.extraIndicatorFlag}");
            
            attack.OnHit(this);
            airHitstunRotation = 0f;
            
            var driveReleaseMultiplier = opponent.burst.driveRelease ? 1.2f : 1f;
            ApplyDamage(attack.GetUnscaledDamage(this) * hitStateDamageMultiplier * driveReleaseMultiplier, data, attack.GetDamageSpecialProperties(this));
            
            foreach (var summon in summons.ToArray()) {
                if (summon is Token token) {
                    if (token.flags.HasFlag(TokenFlag.DESTROY_ON_OWNER_DAMAGE)) {
                        DestroySummon(token);
                    }
                }
            }
        }

        data.result = blocked ? AttackResult.BLOCKED : AttackResult.HIT;
        fxManager.NotifyHit(data);
        
        // counter hit effects
        if (hitstate == Hitstate.COUNTER && !blocked && !armor) {
            var level = attack.GetCounterHitType(this);
            // Debug.Log(level);
            if (level == CounterHitType.LARGE) {
                SimpleCameraShakePlayer.inst.Play("cmn/battle/fx/camerashake/counter", "slide_large");
                SimpleCameraShakePlayer.inst.Play("cmn/battle/fx/camerashake/counter", "zoomin_large");
                PostProcessManager.inst.PlayShaker("chromab_counter_large");
                PostProcessManager.inst.PlayShaker("vignette_counter_large");
                PostProcessManager.inst.PlayShaker("panini_counter_large");
                // PostProcessManager.inst.PlayShaker("dof_counter_large");
                
                audioManager.PlaySoundClip("cmn/battle/sfx/counter/large");
                fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/counter/flash", CharacterFXSocketType.SELF);
                
                BackgroundUIManager.inst.Flash(0.1f);
                
                TimeManager.inst.Queue(() => slowdownFrames = 35);
                inputProvider.inputBuffer.SimulatedClear();

            } else if (level == CounterHitType.MEDIUM) {
                SimpleCameraShakePlayer.inst.Play("cmn/battle/fx/camerashake/counter", "slide_medium");
                SimpleCameraShakePlayer.inst.Play("cmn/battle/fx/camerashake/counter", "zoomin_medium");
                TimeManager.inst.Queue(() => slowdownFrames = 35);
                PostProcessManager.inst.PlayShaker("chromab_counter_medium");
                PostProcessManager.inst.PlayShaker("vignette_counter_medium");
                
                BackgroundUIManager.inst.Flash(0.05f);
                
                fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/counter/flash", CharacterFXSocketType.WORLD_UNBOUND, transform.position, new(side == EntitySide.LEFT ? 0 : 180, 0, 0));
                
                audioManager.PlaySoundClip("cmn/battle/sfx/counter/large");
                inputProvider.inputBuffer.SimulatedClear();


            } else if (level == CounterHitType.SMALL) {
                slowdownFrames = 11;
            }
        }
        
        // pushback
        if (!armor) {
            var amount = attack.GetPushback(this, airborne, blocked);
            if (blocked) {
                amount.y = 0f;
                
            } else {
                if (hitstate == Hitstate.COUNTER) {
                    amount.y *= 1.2f;
                }
            }
            
            if (knockedDown) {
                amount.y = 0f;
                amount.x *= .2f;
            }
            
            if (data.from is PlayerCharacter player) {
                var decayData = player.comboDecayData;
                if (comboCounter.inCombo) {
                    amount *= new Vector2(
                        decayData.opponentBlowbackCurve.Evaluate(comboCounter.comboDecay), 
                        decayData.opponentLaunchCurve.Evaluate(comboCounter.comboDecay)
                        );
                }
            }
            
            ApplyCarriedPushback(amount, attack.GetCarriedMomentumPercentage(this), attack.GetAtWallPushbackMultiplier(this));
        }
        
        // apply freeze frames

        var freezeFrames = attack.GetFreezeFrames(this);
        if (addFreezeFrames) {
            var delay = armor ? 0 : 4;
            if (hitstate == Hitstate.COUNTER) {
                var level = attack.GetCounterHitType(this);
                // Debug.Log(level);
                if (level == CounterHitType.LARGE) freezeFrames = 31;
                else if (level == CounterHitType.MEDIUM) freezeFrames = 21;
            }
            
            TimeManager.inst.Schedule(delay, freezeFrames);
        }
    }

    private void HandleOnHitStateTransition(IAttack attack, bool crouching, out bool addFreezeFrames) {
        var specialProperties = attack.GetSpecialProperties(this);
        addFreezeFrames = true;
        
        if (activeState.type.HasFlag(EntityStateType.CHR_HARD_KNOCKDOWN)) {
            BeginState("CmnSoftKnockdown");
            return;
        }
        
        if (specialProperties.HasFlag(AttackSpecialProperties.HARD_KNOCKDOWN)) {
            // Debug.Log(activeState.id);
            if (airborne || specialProperties.HasFlag(AttackSpecialProperties.FORCE_LAUNCH)) {
                frameData.landingFlag |= LandingRecoveryFlag.HARD_KNOCKDOWN_LAND;
                BeginState("CmnHitStunAir");
                
            } else {
                addFreezeFrames = false;
                BeginState("CmnHardKnockdown");
            }

        } else if (specialProperties.HasFlag(AttackSpecialProperties.SOFT_KNOCKDOWN)) {
            addFreezeFrames = false;
            // airborne = true;
            transform.position += new Vector3(0, .5f, 0);
            BeginState("CmnHitStunAir");
        } else {
            if (!activeState.type.HasFlag(EntityStateType.CHR_HITSTUN)) {
                if (airborne) BeginState("CmnHitStunAir");
                else BeginState(crouching ? "CmnHitStunGroundCrouch" : "CmnHitStunGround");
            } 
        }
    }
    
    private bool CheckAttackHit(AttackData data) {
        var attack = data.attack;
        
        var blockType = attack.GetGuardType(this);
        bool crouching = activeState is State_CmnNeutralCrouch || activeState is State_CmnBlockStunCrouch;
        bool blockHeld = inputProvider.inputBuffer.thisFrame.HasInput(side, InputType.BACKWARD, InputFrameType.HELD);

        AttackGuardType currentGuardType;
        if (airborne) {
            currentGuardType = blockType;
        } else {
            currentGuardType = crouching ? AttackGuardType.CROUCHING : AttackGuardType.STANDING;
        }
        
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
            // Debug.Log($"neutral, {activeState.id}");
            return false;
        }

        return true;
    }
    
    private void OnGroundContact() {
        if (airborne) {
            airborne = false;
            onLand.Invoke();

            if (activeState is State_CmnHitStun) {
                fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/land/medium", CharacterFXSocketType.WORLD_UNBOUND, transform.position);
            } else {
                fxManager.PlayGameObjectFX("cmn/battle/fx/prefab/common/land/light", CharacterFXSocketType.WORLD_UNBOUND, transform.position);
            }
            
            // effect
            {
                var flag = frameData.landingFlag;
                string shakeData;
                if (flag.HasFlag(LandingRecoveryFlag.HARD_LAND_COSMETIC)) shakeData = "land_large";
                else if (flag.HasFlag(LandingRecoveryFlag.HARD_KNOCKDOWN_LAND)) shakeData = "land_medium";
                else if (activeState is State_CmnHitStunAir) shakeData = "land_small";
                else shakeData = "land_normal";
                
                SimpleCameraShakePlayer.inst.PlayCommon(shakeData);
            }
            
            activeState.OnLand(frameData.landingFlag, frameData.landingRecoveryFrames);
            frameData.landingFlag = LandingRecoveryFlag.NONE;
        }

        ResetAirOptions();
    }

    public void ApplyCarriedPushback(Vector2 vec, Vector2 carriedMomentum, float atWallMultiplier = 1f) {
        float direction = side == EntitySide.LEFT ? -1 : 1;
        
        if (pushboxManager.atWall) {
            opponent.rb.linearVelocity *= carriedMomentum;
            opponent.rb.AddForceX(vec.x * -direction * atWallMultiplier, ForceMode2D.Impulse);
            opponent.groundedFrictionAlpha = 0;
            // Debug.Log("add opponent force");
            
        } else {
            rb.linearVelocity *= carriedMomentum;
            // Debug.Log(vec.x * direction * atWallMultiplier);
            rb.AddForceX(vec.x * direction, ForceMode2D.Impulse);
            groundedFrictionAlpha = 0;
            // Debug.Log("not at wall");
        }
        
        // y force
        // Debug.Log(vec);
        rb.linearVelocityY = 0;

        var yAmp = (Mathf.Max(1, characterConfig.baseGravityFinal / 1.9f) - 1) / 2f;
        rb.AddForceY(vec.y + yAmp, ForceMode2D.Impulse);
    }
    
    public void PlayOwnedFx(string fx, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default) {
        PlayFx($"chr/{config.id}/battle/fx/{fx}", type, offset, direction);
    }
    
    public void PlayFx(string fx, CharacterFXSocketType type, Vector3 offset = default, Vector3 direction = default) {
        AssetManager.Get<GameObject>(fx, go => {
            fxManager.PlayGameObjectFX(go, type, offset, direction);
        });
    }

    public void ApplyDamage(float rawDamage, [CanBeNull] AttackData data, DamageProperties flags = DamageProperties.NONE, bool blocked = false) {
        var attack = data == null ? null : data.attack;
        var dmg = rawDamage;

        var skipRegister = flags.HasFlag(DamageProperties.SKIP_REGISTER);
        if (attack != null || skipRegister) {
            comboCounter.RegisterAttack(
                rawDamage,
                attack,
                this, 
                flags, 
                data != null && attack != null ? attack.GetComboDecayIncreaseMultiplier(data.to) : 1f,
                blocked
            );
        }
        
        if (!flags.HasFlag(DamageProperties.IGNORE_COMBO)) {
            dmg *= comboCounter.finalScale * (attack == null ? 1 : comboCounter.GetMoveSpecificProration(attack));
        }
        
        // combo decay
        if (data != null && data.from is PlayerCharacter player) {
            var decayData = player.comboDecayData;
            if (comboCounter.inCombo) {
                dmg *= decayData.extraProrationCurve.Evaluate(comboCounter.comboDecay);
            }
        }

        dmg *= gutsDamageModifier;
        dmg *= characterConfig.defenseModifierFinal;
        if (attack != null && activeState is State_CmnHardKnockdown) dmg *= attack.GetOtgDamagePercentage(this);
        
        var minDmg = attack == null ? 0 : rawDamage * attack.GetMinimumDamagePercentage(this);
        // Debug.Log(rawDamage);
        health -= Mathf.Max(Mathf.Max(1, minDmg), dmg);
    }

    public bool MatchesAirState(AttackAirOkType flag) {
        if (flag.HasFlag(AttackAirOkType.ALL)) return true;
        if (!flag.HasFlag(AttackAirOkType.GROUND) && !airborne) return false;
        if (!flag.HasFlag(AttackAirOkType.AIR) && airborne) return false;
        return true;
    }

    public override IHandle GetHandle() {
        return new PlayerHandle(this);
    }

    public override void Serialize(StateSerializer serializer) {
        base.Serialize(serializer);
        
        serializer.Put("inputs", inputProvider.inputBuffer);
    }

    public override void Deserialize(StateSerializer serializer) {
        base.Deserialize(serializer);
        
        
    }

}

public abstract class RuntimeCharacterDataRegister {
    public PlayerCharacter owner { get; private set; }
    public RuntimeCharacterDataRegister(PlayerCharacter owner) {
        this.owner = owner;
    }
}

public struct PlayerHandle : IHandle {
    private int id;
    
    public PlayerHandle(PlayerCharacter player) {
        id = player.playerIndex;
    }
    
    public void Serialize(StateSerializer serializer) {
        serializer.Put("playerIndex", id);
    }
    public void Deserialize(StateSerializer serializer) {
        id = serializer.Get<int>("playerIndex");
    }
    public object Resolve() {
        return GameManager.inst.GetPlayer(id);
    }

    public override string ToString() {
        return $"PlayerHandle({id})";;
    }
}
}
