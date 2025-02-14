using System;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using SuperSmashRhodes.Adressable;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Battle.State;
using SuperSmashRhodes.Battle.State.Implementation;
using SuperSmashRhodes.Character.Gauge;
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
    public int airOptions { get; set; }
    public IInputProvider inputProvider { get; private set; } = new NOPInputProvider(); // InputProvider assigned on round start

    public bool atWall => pushboxManager.atWall;
    public float wallDistance {
        get {
            var stageData = GameManager.inst.stageData;
            // var nearestWall = side == EntitySide.LEFT ? stageData.leftWallPosition : stageData.rightWallPosition;
            return Mathf.Min(Mathf.Abs(stageData.leftWallPosition - transform.position.x), Mathf.Abs(stageData.rightWallPosition - transform.position.x));
        }
    }
    public bool burstDisabled => stateFlags.HasFlag(CharacterStateFlag.DISABLE_BURST);
    public float cameraGroupWeight {
        get {
            if (stateFlags.HasFlag(CharacterStateFlag.NO_CAMERA_WEIGHT)) {
                return 0;
            }
            return 1f;
        }
    }
    
    private float airHitstunRotation = 0f;
    private float yRotationTarget = 0f;
    public int backdashCooldown { get; set; }

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
    }
    
    protected override void Start() {
        base.Start();
        owner = this;
        comboCounter = new(this);
        frameData = new(this);
        
        pushboxManager = GetComponentInChildren<PushboxManager>();
        fxManager = GetComponentInChildren<CharacterFXManager>();
        meter = GetComponent<PlayerMeterGauge>();
        burst = GetComponent<PlayerBurstGauge>();
        
        pushboxManager.onGroundContact.AddListener(OnGroundContact);
        
        AssetManager.inst.PreloadAll($"chr/{config.id}/battle/**");
    }

    protected override void Update() {
        base.Update();
    }

    protected override void OnTick() {
        base.OnTick();
        frameData.Tick();
        UpdateInput();
        UpdateFacing();
        UpdatePosition();
        UpdateGravity();
        UpdateRotation();
        if (backdashCooldown > 0) {
            --backdashCooldown;
        }

        if (applyGroundedFrictionFrames > 0) {
            --applyGroundedFrictionFrames;
            groundedFrictionAlpha = Mathf.Lerp(groundedFrictionAlpha, 1, Time.fixedDeltaTime);
            rb.linearVelocityX = Mathf.Lerp(rb.linearVelocityX, 0, Time.fixedDeltaTime * 20f * groundedFrictionAlpha);
        }
        
    }

    private void UpdateGravity() {
        var ret = characterConfig.baseGravityFinal;
        if (comboCounter.inCombo) {
            var decay = comboCounter.comboDecay;
            var data = opponent.comboDecayData;
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
            airHitstunRotation = Mathf.Clamp(airHitstunRotation + 0.5f, -55f, 55f);

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
    }
    
    private void UpdateInput() { 
        if (inputProvider.inputBuffer == null) return;
        
        // get priority sorted list
        var li = (from state in states.Values
            orderby ((CharacterState)state).inputPriority descending
            select (CharacterState)state).ToList();
        
        foreach (var state in li) {
            if (state == activeState && !activeState.isSelfCancellable) continue;
            
            if (activeState.stateData.cancelOptions.Contains(state) || BitUtil.CheckFlag((ulong)activeState.stateData.cancelFlag, (ulong)state.type)) {
                // state is valid
                if (state.IsInputValid(inputProvider.inputBuffer) && state.mayEnterState) {
                    // check cancel state
                
                    BeginState(state);
                    break;
                }
            }
            
        }
        
    }

    private void UpdatePosition() {
        var x = transform.position.x;
        var stageData = GameManager.inst.stageData;
        if (x <= stageData.leftWallPosition || x >= stageData.rightWallPosition) {
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
            if (Mathf.Abs(pos - opponentPos) < pushboxManager.pushbox.size.x) return;
            if ((airborne && activeState is State_CmnHitStunAir) || (opponent.airborne && opponent.activeState is State_CmnHitStunAir)) return;
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
        
        if (!EntityStateRegistry.inst.CreateInstance(name, out var ret, this))
            throw new Exception("Default state [CmnNeutral] not assigned");
        
        return ret;
    }

    public override void OnRoundInit() {
        base.OnRoundInit();
        // position
        
        // TODO: Z index management
        transform.position = new(playerIndex == 0 ? -1.5f : 1.5f, 0f, 0f);
        onRoundInit.Invoke();
        airActionPerformed = false;
        
        // input provider
        inputProvider = LocalInputManager.inst.GetInputProvider(this);
        
        airOptions = characterConfig.airOptionsFinal;
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
        bool addFreezeFrames = true;
        bool knockedDown = activeState.type.HasFlag(EntityStateType.CHR_HARD_KNOCKDOWN);
        
        // Debug.Log($"inbound hit 1, neutral: {neutral}, blockheld: {blockHeld}, blockType: {blockType} blocked: {blocked}, framesRemaining: {framesRemaining}, blockstun {framesRemaining + move.frameData.onBlock}");
        ApplyGroundedFriction();
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

            var chipDamage = attack.GetChipDamagePercentage(this);
            if (chipDamage > 0) ApplyDamage(attack.GetUnscaledDamage(this) * chipDamage, data, DamageSpecialProperties.SKIP_REGISTER, true);

            foreach (var summon in summons.ToArray()) {
                if (summon is Token token) {
                    if (token.flags.HasFlag(TokenFlag.DESTROY_ON_OWNER_BLOCK)) {
                        DestroySummon(token);
                    }
                }
            }

        } else {
            // this.frameData.SetHitstunFrames(framesRemaining + frameData.onHit, Mathf.Max(frameData.total - attack.GetCurrentFrame(this), 0));
            this.frameData.SetHitstunFrames(attack.GetStunFrames(this, false), 0);
            
            // hit state select
            HandleOnHitStateTransition(attack, crouching, out addFreezeFrames);
            
            attack.OnHit(this);
            airHitstunRotation = 0f;
            
            ApplyDamage(attack.GetUnscaledDamage(this), data);
            
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
        
        // pushback
        {
            var amount = attack.GetPushback(this, airborne, blocked);
            if (blocked) amount.y = 0f;
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
        if (addFreezeFrames) TimeManager.inst.Schedule(4, freezeFrames);
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
                SimpleCameraShakeData shakeData;
                if (flag.HasFlag(LandingRecoveryFlag.HARD_LAND_COSMETIC)) shakeData = fxManager.fxLibrary.cameraShakeOnLandLarge;
                else if (flag.HasFlag(LandingRecoveryFlag.HARD_KNOCKDOWN_LAND)) shakeData = fxManager.fxLibrary.cameraShakeOnLandMedium;
                else if (activeState is State_CmnHitStunAir) shakeData = fxManager.fxLibrary.cameraShakeOnLandSmall;
                else shakeData = fxManager.fxLibrary.cameraShakeOnLandNormal;
                
                SimpleCameraShakePlayer.inst.Play(shakeData);
            }
            
            activeState.OnLand(frameData.landingFlag, frameData.landingRecoveryFrames);
            frameData.landingFlag = LandingRecoveryFlag.NONE;
        }

        airOptions = characterConfig.airOptionsFinal;
    }

    public void ApplyCarriedPushback(Vector2 vec, Vector2 carriedMomentum, float atWallMultiplier = 1f) {
        float direction = side == EntitySide.LEFT ? -1 : 1;
        
        if (pushboxManager.atWall) {
            opponent.rb.linearVelocity *= carriedMomentum;
            opponent.rb.AddForceX(vec.x * -direction * atWallMultiplier, ForceMode2D.Impulse);
            opponent.groundedFrictionAlpha = 0;
            // Debug.Log("at wall");
            // Debug.Log("add opponent force");
            
        } else {
            rb.linearVelocity *= carriedMomentum;
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

    public void ApplyDamage(float rawDamage, [CanBeNull] AttackData data, DamageSpecialProperties flags = DamageSpecialProperties.NONE, bool blocked = false) {
        var attack = data == null ? null : data.attack;
        var dmg = rawDamage;

        var skipRegister = flags.HasFlag(DamageSpecialProperties.SKIP_REGISTER);
        if (attack != null || skipRegister) {
            comboCounter.RegisterAttack(
                attack,
                this, 
                skipRegister, 
                data != null && attack != null ? attack.GetComboDecayIncreaseMultiplier(data.to) : 1f,
                data != null && attack != null ? attack.ShouldCountSameMove(data.to) : false,
                blocked
            );
        }
        
        if (!flags.HasFlag(DamageSpecialProperties.IGNORE_COMBO)) {
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
        burst.AddDeltaTotal(dmg / 2.5f, 60);
    }

    public bool MatchesAirState(AttackAirOkType flag) {
        if (flag.HasFlag(AttackAirOkType.ALL)) return true;
        if (!flag.HasFlag(AttackAirOkType.GROUND) && !airborne) return false;
        if (!flag.HasFlag(AttackAirOkType.AIR) && airborne) return false;
        return true;
    }
    
}

public abstract class RuntimeCharacterDataRegister {
    public PlayerCharacter owner { get; private set; }
    public RuntimeCharacterDataRegister(PlayerCharacter owner) {
        this.owner = owner;
    }
}
}
