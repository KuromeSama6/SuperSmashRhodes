using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SuperSmashRhodes.Battle.Game;
using SuperSmashRhodes.Input;
using UnityEngine;

namespace SuperSmashRhodes.Battle.State.Implementation {
public abstract class ThrowAttackStateBase : CharacterAttackStateBase {
    protected ThrowAttackStateBase(Entity owner) : base(owner) { }
    public override EntityStateType type => EntityStateType.CHR_ATK_THROW;
    protected override EntityStateType commonCancelOptions => 0;
    protected override InputFrame[] requiredInput => null;
    protected override int normalInputBufferLength => 1;
    
    public bool connected { get; private set; }
    private int hitAnimationStartFrame;
    
    protected override void OnStateBegin() {
        base.OnStateBegin();
        connected = false;
        hitAnimationStartFrame = 0;
    }

    public override IEnumerator MainRoutine() {
        phase = AttackPhase.STARTUP;
        OnStartup();
        yield return frameData.startup;
        
        // proximity check
        phase = AttackPhase.ACTIVE;
        OnActive();
        connected = false;
        for (int i = 0; i < frameData.active; i++) {
            if (player.opponentDistance < distanceRequirement) {
                connected = true;
                break;
            }
            yield return 1;
        }
        
        // process hit
        bool mayHit = MayHit(player.opponent);
        var opponent = player.opponent;
        
        if (connected && mayHit) {
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
        if (connected) {
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

    protected virtual int[] GetCosmeticHitFrames(PlayerCharacter to) {
        return new int[0];
    }
    protected virtual bool MayHit(PlayerCharacter other) {
        if (other.frameData.throwInvulnFrames > 0) return false;
        if (player.airborne != other.airborne) return false;
        if (other.inUnmanagedTime) return false;
        if (other.activeState.fullyInvincible) return false;
        if (other.activeState.type.HasFlag(EntityStateType.CHR_STUN) || other.activeState.type.HasFlag(EntityStateType.CHR_KNOCKDOWN)) return false;

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
        player.audioManager.PlaySound(GetHitSfx(player.opponent), .6f);
        player.opponent.fxManager.NotifyHit(CreateAttackData()); 
    }
}
}
