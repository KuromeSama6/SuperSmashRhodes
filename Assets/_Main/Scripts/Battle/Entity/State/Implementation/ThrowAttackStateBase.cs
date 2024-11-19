using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Enums;
using SuperSmashRhodes.Battle.FX;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Input;
using SuperSmashRhodes.Util;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class ThrowAttackStateBase : CharacterAttackStateBase {
    protected ThrowAttackStateBase(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_THROW;
    protected override EntityStateType commonCancelOptions => 0;
    protected override InputFrame[] requiredInput => null;
    protected override int normalInputBufferLength => 1;
    
    public bool connected { get; private set; }
    protected bool hasHit { get; private set; }
    private int hitAnimationStartFrame;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        stateData.ClearCancelOptions();
        connected = false;
        hitAnimationStartFrame = 0;
        hasHit = false;
    }

    public override IEnumerator MainRoutine() {
        phase = AttackPhase.STARTUP;
        var opponent = player.opponent;
        
        OnStartup();
        
        yield return frameData.startup;
        
        // proximity check
        phase = AttackPhase.ACTIVE;
        OnActive();
        connected = false;
        for (int i = 0; i < frameData.active; i++) {
            if (player.opponentDistance < distanceRequirement) {
                if (!connected) {
                    opponent.fxManager.PlayGameObjectFX(
                        "cmn/battle/fx/prefab/common/throw_circle/0", 
                        CharacterFXSocketType.SELF
                    );
                }
                connected = true;
            }
            yield return 1;
        }
        
        // process hit
        bool mayHit = MayHit(player.opponent);

        bool clash = false;
        // clash
        if (connected) {
            var otherState = opponent.activeState;
            if (otherState is ThrowAttackStateBase th) {
                if (th.phase < AttackPhase.RECOVERY && ClashableWith(th)) clash = true;
            }
        }
        
        if (connected && mayHit) {
            if (clash) {
                player.BeginState("CmnSoftKnockdown");
                opponent.BeginState("CmnSoftKnockdown");

                Vector2 force = new(5, 0);
                player.ApplyCarriedPushback(force, new(0.5f, 0));
                opponent.ApplyCarriedPushback(force, new(0.5f, 0));
                
                player.fxManager.PlayGameObjectFX(
                    "cmn/batte/fx/prefab/common/throw_clash/0", 
                    CharacterFXSocketType.WORLD_UNBOUND,
                    new(Mathf.Lerp(player.transform.position.x, opponent.transform.position.x, .5f), 1, 0)
                );
                player.audioManager.PlaySound("cmn/battle/sfx/throw_break/1");
                yield break;
            }

            // successful hit
            hasHit = true;
            bool switchSides = ShouldSwitchSides(opponent);
            player.audioManager.PlaySound("cmn/battle/sfx/throw/1");
            
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
            // process throw hit
            opponent.BeginState("CmnHitStunGround");
            opponent.unmanagedTime = new() {
                frames = animationLength,
                flags = UnmanagedTimeSlotFlags.TIME_THROW,
            };

            var socket = new CinematicCharacterSocket(opponent, player, throwSocketBoneName, new(0, -1, 0));
            socket.Attach();
            hitAnimationStartFrame = frame;
            player.animation.AddUnmanagedAnimation(mainAnimation, false);
            yield return animationLength;
            OnFinalHit();
            player.opponent.ApplyDamage(GetUnscaledDamage(player.opponent) - GetCosmeticHitFrames(player.opponent).Length, CreateAttackData());
            socket.Release();

            opponent.unmanagedTime = default;
            opponent.BeginState("CmnHardKnockdown");

        } else {
            phase = AttackPhase.RECOVERY;
            owner.animation.AddUnmanagedAnimation(whiffAnimation, false);
            
            OnThrowTech(opponent);
            yield return frameData.recovery;
        }

    }

    protected override void OnTick() {
        base.OnTick();
        if (hasHit) {
            // cosmetic animation
            var hitAnimationFrame = frame - hitAnimationStartFrame;
            var cosmeticFrames = GetCosmeticHitFrames(player.opponent);
            if (cosmeticFrames.Contains(hitAnimationFrame)) {
                player.opponent.ApplyDamage(1, null, DamageSpecialProperties.REAL_DAMAGE | DamageSpecialProperties.SKIP_REGISTER);
                OnCosmeticHit();
            }
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

    protected AttackData CreateAttackData() {
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
    protected virtual int[] GetCosmeticHitFrames(PlayerCharacter to) {
        return new int[0];
    }
    protected virtual bool ShouldSwitchSides(PlayerCharacter other) {
        return false;
    }
    protected virtual bool MayHit(PlayerCharacter other) {
        if (other.frameData.throwInvulnFrames > 0) return false;
        if (player.airborne != other.airborne) return false;
        if (other.inUnmanagedTime) return false;
        if (other.activeState.fullyInvincible) return false;
        if (other.activeState.type.CheckFlag((ulong)EntityStateType.CHR_STUN)) return false;
        if (other.activeState.type.CheckFlag((ulong)EntityStateType.CHR_KNOCKDOWN)) return false;

        return true;
    }
    
    protected virtual void OnThrowHit(PlayerCharacter other) {
    }
    protected virtual void OnThrowWhiff(PlayerCharacter other) {
        
    }
    protected virtual void OnThrowTech(PlayerCharacter other) {
        
    }
    protected virtual void OnCosmeticHit() {
        player.audioManager.PlaySound(GetHitSfx(player.opponent), .6f);
        player.opponent.fxManager.NotifyHit(CreateAttackData());
    }
    protected virtual void OnFinalHit() {
        OnHit(player.opponent);
        player.opponent.fxManager.NotifyHit(CreateAttackData()); 
    }
}
}
