﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Animation;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Runtime.State;
using SuperSmashRhodes.UI.Battle.AnnouncerHud;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class ThrowAttackStateBase : CharacterAttackStateBase {
    protected ThrowAttackStateBase(Entity entity) : base(entity) { }
    protected override EntityStateType commonCancelOptions => 0;
    protected override InputFrame[] requiredInput => null;
    protected override int normalInputBufferLength => 1;
    public override AttackType attackType => AttackType.THROW;

    public bool connected { get; private set; }
    protected bool hasHit { get; private set; }
    public override StateIndicatorFlag stateIndicator => connected ? StateIndicatorFlag.THROW : StateIndicatorFlag.NONE;
    public override Hitstate hitstate => Hitstate.COUNTER;
    private int hitAnimationStartFrame;
    private CinematicCharacterSocket socket;
    private bool mayHit, clash;
    private int contactCheckCounter;

    protected override void OnStateBegin() {
        base.OnStateBegin();
        stateData.ClearCancelOptions();
        connected = false;
        hitAnimationStartFrame = 0;
        hasHit = false;
        if (socket != null) {
            socket.Release();
            socket = null;
        }
        mayHit = false;
        clash = false;
        contactCheckCounter = 0;
    }

    public override EntityStateSubroutine BeginMainSubroutine() {
        return Sub_ThrowStartup;
    }

    protected virtual void Sub_ThrowStartup(SubroutineContext ctx) {
        phase = AttackPhase.STARTUP;
        // Debug.Log("start");
        player.animation.AddUnmanagedAnimation(mainAnimation, false);
        OnStartup();
        ctx.Next(frameData.startup, Sub_ThrowActive);
    }

    protected virtual void Sub_ThrowActive(SubroutineContext ctx) {
        phase = AttackPhase.ACTIVE;
        OnActive();
        connected = false;
        ctx.Next(frameData.active, Sub_CheckWaitLoop);
    }

    protected virtual void Sub_CheckWaitLoop(SubroutineContext ctx) {
        if (player.opponentDistance < distanceRequirement) {
            if (!connected) {
                opponent.fxManager.PlayGameObjectFX(
                    "cmn/battle/fx/prefab/common/throw_circle/0", 
                    CharacterFXSocketType.SELF
                );
            }
            connected = true;
        }
        
        ++contactCheckCounter;
        if (contactCheckCounter >= frameData.active) {
            ctx.Next(0, Sub_ThrowCheckHit);
        } else {
            ctx.Repeat();
        }
    }

    protected virtual void Sub_ThrowCheckHit(SubroutineContext ctx) {
        mayHit = MayHit(player.opponent);

        clash = false;
        // clash
        if (connected) {
            var otherState = opponent.activeState;
            if (otherState is ThrowAttackStateBase th) {
                if (th.phase < AttackPhase.RECOVERY && ClashableWith(th)) clash = true;
            }
        }

        if (connected && mayHit) {
            if (clash) {
                player.fxManager.PlayGameObjectFX(
                    "cmn/battle/fx/prefab/common/throw_clash/0", 
                    CharacterFXSocketType.WORLD_UNBOUND,
                    new(
                        Mathf.Lerp(player.transform.position.x, opponent.transform.position.x, .5f), 
                        Mathf.Lerp(player.transform.position.y, opponent.transform.position.y, .5f) + 1, 
                        0)
                );
                entity.PlaySound("cmn/battle/sfx/throw_break/1");
                OnThrowTech(opponent);
                ctx.Exit();
                return;
            }
            
            // successful hit
            // Debug.Log("hit");
            hasHit = true;
            bool switchSides = ShouldSwitchSides(opponent);
            entity.PlaySound("cmn/battle/sfx/throw/1");
            stateData.ClearCancelOptions();
            
            var dist = player.opponentDistance * 2;
            if (switchSides) {
                var atWall = player.pushboxManager.atWall;
                opponent.transform.position -= new Vector3(opponent.side == EntitySide.RIGHT ? 1 : -1 * dist, 0, 0);
                (player.side, opponent.side) = (opponent.side, player.side);
                
                if (atWall) {
                    player.transform.position -= new Vector3(dist * (player.side == EntitySide.LEFT ? 1 : -1), 0, 0);
                }
            } else {
                if (opponent.pushboxManager.atWall) {
                    
                    player.transform.position -= new Vector3(dist * (player.side == EntitySide.LEFT ? 1 : -1), 0, 0);
                }
            }
            
            OnThrowHit(opponent);
            opponent.bufferClearRequested = player.bufferClearRequested = true;
            
            // process throw hit
            opponent.BeginState("CmnHitStunGround");
            opponent.stateFlags = CharacterStateFlag.TIME_THROW | CharacterStateFlag.DEATH_HOLD;
            stateData.cameraData.cameraFovModifier = -5;

            socket = new CinematicCharacterSocket(opponent, player, throwSocketBoneName, new(0, -1, 0));
            socket.Attach();
            hitAnimationStartFrame = frame;
            
            ctx.Next(animationLength - frameData.startup, Sub_ThrowHitEnd);
            
        } else {
            phase = AttackPhase.RECOVERY;
            OnThrowWhiff(opponent);
            entity.animation.AddUnmanagedAnimation(whiffAnimation, false);
            ctx.Exit(frameData.recovery);
        }
    }

    protected virtual void Sub_ThrowHitEnd(SubroutineContext ctx) {
        OnFinalHit();
        socket.Release();
        socket = null;
        opponent.stateFlags = default;
        CancelInto(player.airborne ? "CmnAirNeutral" : "CmnNeutral");
    }

    protected override void OnStateEnd(EntityState nextState) {
        base.OnStateEnd(nextState);
        if (socket != null) {
            socket.Release();
            socket = null;
            opponent.stateFlags = default;
        }
    }

    protected override void OnTick() {
        base.OnTick();
        
        if (socket != null && socket.attached) {
            socket.Tick();
        }
    }

    public override float GetChipDamagePercentage(Entity to) {
        return 0;
    }
    public override float GetOtgDamagePercentage(Entity to) {
        return 0;
    }
    public override Vector2 GetPushback(Entity to, bool airborne, bool blocked) {
        return Vector2.zero;
    }
    public override float GetComboProration(Entity to) {
        return 1;
    }
    public override float GetFirstHitProration(Entity to) {
        return .5f;
    }
    public override AttackGuardType GetGuardType(Entity to) {
        return AttackGuardType.THROW;
    }
    public override int GetFreezeFrames(Entity to) {
        return 0;
    }
    public override int GetAttackLevel(Entity to) {
        return 3;
    }

    public AttackData CreateAttackData() {
        return new() {
            attack = this,
            from = player,
            interactionData = default,
            result = AttackResult.HIT,
            to = opponent
        };
    }
    
    protected abstract string whiffAnimation { get; }
    protected abstract float distanceRequirement { get; }
    protected abstract int animationLength { get; }
    protected virtual string throwSocketBoneName => "throw_opponent";

    protected abstract bool ClashableWith(ThrowAttackStateBase other);
    protected virtual bool ShouldSwitchSides(PlayerCharacter other) {
        return false;
    }
    protected virtual bool MayHit(PlayerCharacter other) {
        if (other.frameData.throwInvulnFrames > 0) return false;
        // if (player.airborne != other.airborne) return false;
        if (other.activeState.invincibility.HasFlag(AttackType.THROW)) return false;
        if (other.activeState.type.CheckFlag((ulong)EntityStateType.CHR_STUN)) return false;
        if (other.activeState.type.CheckFlag((ulong)EntityStateType.CHR_KNOCKDOWN)) return false;

        return true;
    }
    
    protected virtual void OnThrowHit(PlayerCharacter other) {
        if (other.activeState is CharacterState attack) {
            if (attack.hitstate == Hitstate.COUNTER) player.activeState.stateData.extraIndicatorFlag |= StateIndicatorFlag.COUNTER;
            if (attack.hitstate == Hitstate.PUNISH) player.activeState.stateData.extraIndicatorFlag |= StateIndicatorFlag.PUNISH;
        }
    }
    protected virtual void OnThrowWhiff(PlayerCharacter other) {
        
    }
    protected virtual void OnThrowTech(PlayerCharacter other) {
        Vector2 force = new(5, 0);
        player.ApplyCarriedPushback(force, new(0.5f, 0));
        opponent.ApplyCarriedPushback(force, new(0.5f, 0));
        
        player.SetCarriedStateVariable("_fromThrowTech", "CmnSoftKnockdown", true);
        opponent.SetCarriedStateVariable("_fromThrowTech", "CmnSoftKnockdown", true);
        
        player.BeginState("CmnSoftKnockdown");
        opponent.BeginState("CmnSoftKnockdown");
    }
    protected virtual void OnCosmeticHit() {
        entity.PlaySound(GetHitSfx(player.opponent), .6f);
        player.opponent.fxManager.NotifyHit(CreateAttackData());
    }
    protected virtual void OnFinalHit() {
        // OnHit(player.opponent);
        // Debug.Log("hitstun hard");
        opponent.BeginState("CmnHardKnockdown");
        player.meter.AddMeter(GetMeterGain(opponent, false));
    }
    
    public override void OnApplyCinematicDamage(AnimationEventData data) {
        player.opponent.fxManager.NotifyHit(CreateAttackData()); 
        player.opponent.ApplyDamage(data.integerValue, CreateAttackData(), DamageProperties.SKIP_REGISTER);
        // Debug.Log("ThrowAttackStateBase.OnApplyCinematicDamage");
    }
    
    public override CounterHitType GetCounterHitType(Entity to) {
        return CounterHitType.EXSMALL;
    }
}
}
