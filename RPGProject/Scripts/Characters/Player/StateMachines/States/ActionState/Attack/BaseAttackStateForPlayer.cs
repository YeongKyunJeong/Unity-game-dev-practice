using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

namespace RSP2
{
    public class BaseAttackStateForPlayer : ActionStateForPlayer
    {
        protected string animatorAttackStateTag = "Attack State";
        protected CombatSystem combatSystem;
        protected AttackData attackData;

        protected int targetNumberLimit;
        protected ForceWithTime[] selfForces;
        protected int forceIndex;
        protected int maxForceIndex;
        protected MomentumDampingMode momentumDampingMode;
        protected bool isAddingForce;
        protected bool isMomentumUpdateFrame;

        protected float minimumDuration;

        protected Vector3 horizontalMomentum;
        //protected float passedTime;
        protected float normalizedPassedTime;

        protected bool isFirstFrame;
        protected bool isCancelable;
        protected bool isAnimationEnd;

        protected readonly int attackHash = Animator.StringToHash("@Attack");

        public BaseAttackStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
            combatSystem = _player.CombatSystem;
        }


        #region IState Methods

        public override void Enter()
        {
            base.Enter();

            SetAnimatorIsAttackingParameter(true);
            SetAnimatorSelfStateParameter(true);
            SetAnimatorPlayingSpeed();

            horizontalMomentum = runtimeData.HorizontalMovementVector;

            targetNumberLimit = attackData.hitNumberLimit;
            selfForces = attackData.SelfForces;
            minimumDuration = attackData.AttackRecoveryTime;

            isMomentumUpdateFrame = false;
            switch (selfForces.Length)
            {
                case 0:
                    {
                        isAddingForce = false;
                        momentumDampingMode = MomentumDampingMode.DefaultDamping;
                        break;
                    }
                default:
                    {
                        isAddingForce = true;
                        forceIndex = 0;
                        maxForceIndex = selfForces.Length;
                        break;
                    }
            }

            isFirstFrame = true;
            isCancelable = false;
            isAnimationEnd = false;

            PlaySFXbyDamageType();
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorSelfStateParameter(false);
            SetAnimatorIsAttackingParameter(false);
            SetAnimatorPlayingSpeed(true);

        }

        public override void CallUpdate()
        {
            base.CallUpdate();

            UpdateNormalizedPassedTime();
            if (isFirstFrame)
            {
                normalizedPassedTime = 0;
                isFirstFrame = false;
            }

            if (isAnimationEnd)
            {
                EndAttackState();
                return;
            }

            if (!isCancelable)
            {
                isCancelable = CheckIsCancelable();
            }

            CalculateThisUpdateMomentum(momentumDampingMode);

            runtimeData.HorizontalMovementVector = horizontalMomentum;

            mover.UpdateNextHorizontalMovementVector(horizontalMomentum);
        }


        #endregion


        #region Input Methods

        protected override void OnDashInput()
        {
            if (isCancelable)
            {
                base.OnDashInput();

                stateMachine.ChangeState(stateMachine.LandDashingState);
            }
        }

        protected override void OnJumpInput()
        {
            if (isCancelable)
            {
                base.OnJumpInput();

                stateMachine.ChangeState(stateMachine.JumpingState);
                return;
            }

        }

        #endregion


        public virtual bool CheckAttackResources(AttackType attackType, int key)
        {
            switch (attackType)
            {
                case AttackType.Basic:
                    {
                        attackData = attackDataLibrary.BaseAttackData;
                        break;
                    }
                case AttackType.MeleeAttackSkill:
                    {
                        attackData = attackDataLibrary.GetMeleeAttackInfo(key);
                        break;
                    }
            }

            if (combatSystem.CurrentMP >= attackData.MPCost)
            {
                if (combatSystem.CurrentStamina >= attackData.StaminaCost) return true;
            }

            return false;
        }


        private void EndAttackState()
        {
            if (FallingCalculator.CheckIsSlope(player.transform).y < -0.98) // No collider detected
            {
                stateMachine.ChangeState(stateMachine.FallingState);
                return;
            }

            if (moveInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);
                return;
            }

            if (runtimeData.IsWalking)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }
            stateMachine.ChangeState(stateMachine.RunningState);
            return;
        }

        protected virtual bool CheckIsCancelable()
        {
            if (normalizedPassedTime > minimumDuration)
            {
                return true;
            }

            return false;
        }

        protected virtual void SetAnimatorPlayingSpeed(bool isExit = false)
        {
            if (isExit)
            {
                animator.speed = 1;
                return;
            }
            animator.speed = player.CurrentWeapon.WeaponData.SpeedModifier * attackData.AttackSpeed * player.StatHandler.CurrentStatistics.AttackSpeed / 5;
        }


        protected virtual void UpdateNormalizedPassedTime()
        {
            normalizedPassedTime = GetNormalizedTime(animator, animatorAttackStateTag);

            if (normalizedPassedTime >= minimumDuration)
            {
                isCancelable = true;
            }
            if (normalizedPassedTime >= 1)
            {
                isAnimationEnd = true;
            }
        }

        protected virtual bool CheckTargetFaction(CombatSystem hitCombatSystem)
        {
            switch (attackData.Target)
            {
                case ChasingTargetType.PlayerOnly:
                    {
                        if (hitCombatSystem.MyFaction == Faction.Player) { return true; }
                    }
                    break;
                case ChasingTargetType.EnemyOnly:
                    {
                        if (hitCombatSystem.MyFaction == Faction.Enemy) { return true; }
                    }
                    break;
                case ChasingTargetType.AllFaction: return true;
                case ChasingTargetType.NotMyFaction:
                    {
                        if (hitCombatSystem.MyFaction != combatSystem.MyFaction) { return true; }
                    }
                    break;
                default:
                    break;
            }

            return false;
        }

        protected virtual void PlaySFXbyDamageType()
        {
            switch (attackData.DamageType)
            {
                case DamageType.ByMainWeapon:
                    {
                        SFXManager.PlayClip(player.CurrentWeapon.WeaponData.AttackSoundClip, player.transform.position, speedMultipliyer: 0.8f);
                        break;
                    }
                default:
                    {
                        SFXManager.PlayClip(attackData.AttackSoundClip, player.transform.position, speedMultipliyer: 1);
                        break;
                    }
            }
        }

        protected virtual void PlayVFXbyDamageType(CombatUnit hitCombatUnit, Vector3 playPosition, Vector3 playDir)
        {
            switch (attackData.DamageType)
            {
                case DamageType.ByMainWeapon:
                    {
                        VFXManager.PlayHitEffect(player.CurrentWeapon.WeaponData.DamageType, hitCombatUnit, playPosition, playDir.normalized);
                        break;
                    }
                default:
                    {
                        VFXManager.PlayHitEffect(attackData.DamageType, hitCombatUnit, playPosition, playDir.normalized);
                        break;
                    }
            }
        }

        protected virtual void ApplyDamage(CombatSystem targetCombatSystem, Vector3 forceDir)
        {
            switch (attackData.DamageType)
            {
                case DamageType.ByMainWeapon:
                    {
                        targetCombatSystem.TakeDamage(statHandler.CurrentStatistics.Attack + attackData.Damage
                                                        + player.CurrentWeapon.WeaponData.DamageBonus[player.CurrentWeapon.ItemInstance.Upgrade],
                                                        player.CurrentWeapon.WeaponData.DamageType);
                        break;
                    }
                default:
                    {
                        targetCombatSystem.TakeDamage(statHandler.CurrentStatistics.Attack + attackData.Damage
                                                        + player.CurrentWeapon.WeaponData.DamageBonus[player.CurrentWeapon.ItemInstance.Upgrade],
                                                        attackData.DamageType);
                        break;
                    }
            }

            targetCombatSystem.TakeForce(forceDir.normalized * attackData.PushForce);
        }

        protected virtual void CalculateThisUpdateMomentum(MomentumDampingMode _momentumDampingMode = MomentumDampingMode.InstantStop)
        {
            if (isAddingForce)
            {
                if (normalizedPassedTime > selfForces[forceIndex].NormalizedTime)
                {
                    momentumDampingMode = selfForces[forceIndex].MomentumDamping;
                    _momentumDampingMode = momentumDampingMode;
                    forceReceiver.AddForce(player.transform.TransformDirection(selfForces[forceIndex].Force), momentumDampingMode);
                    //AddForce(selfForces[forceIndex].Force);
                    isMomentumUpdateFrame = true;
                    forceIndex++;
                    if (forceIndex >= maxForceIndex)
                    {
                        isAddingForce = false;
                    }
                }
            }

            if (isMomentumUpdateFrame) { isMomentumUpdateFrame = false; return; }

            switch (_momentumDampingMode)
            {
                case MomentumDampingMode.DefaultDamping:
                    {
                        horizontalMomentum = Vector3.Lerp(horizontalMomentum, Vector3.zero, 1 - Mathf.Exp(-5 * Time.deltaTime));
                        return;
                    }
                case MomentumDampingMode.SoftDamping:
                    {
                        horizontalMomentum = Vector3.Lerp(horizontalMomentum, Vector3.zero, 1 - Mathf.Exp(-2 * Time.deltaTime));
                        return;
                    }
                case MomentumDampingMode.HardDamping:
                    {
                        horizontalMomentum = Vector3.Lerp(horizontalMomentum, Vector3.zero, 1 - Mathf.Exp(-10 * Time.deltaTime));
                        return;
                    }
                case MomentumDampingMode.InstantStop:
                    {
                        horizontalMomentum = Vector3.zero;
                        return;
                    }
                case MomentumDampingMode.NoDamping:
                    {
                        return;
                    }
                default:
                    {
                        horizontalMomentum = Vector3.zero;
                        return;
                    }
            }
        }

        protected virtual void ChangeMomentum(Vector3 delta)
        {
            horizontalMomentum = player.transform.TransformDirection(delta);
        }

        protected virtual void AddForce(Vector3 delta, MomentumDampingMode momentumDampingMode = MomentumDampingMode.DefaultDamping)
        {
            forceReceiver.AddForce(player.transform.TransformDirection(delta), momentumDampingMode);
        }

        protected virtual void SetAnimatorIsAttackingParameter(bool isOn)
        {
            animator.SetBool(attackHash, isOn);
        }

    }





}
