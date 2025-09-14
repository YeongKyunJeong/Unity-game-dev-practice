using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace RSP2
{
    public class BaseAttackingStateForEnemy : ActionStateForEnemy
    {
        protected readonly int attackHash = Animator.StringToHash("@Attack");
        protected string animatorAttackStateTag = "Attack State";
        private readonly int instantAttackHash = Animator.StringToHash("Attack.BaseAttack");
        //private readonly int isAttackingHash = Animator.StringToHash("isAttacking");

        protected AttackData attackData;
        protected CombatSystem combatSystem;
        protected ForceWithTime[] selfForces;
        protected int forceIndex;
        protected int maxForceIndex;
        protected MomentumDampingMode momentumDampingMode;
        protected bool isAddingForce;
        protected bool isMomentumUpdateFrame;

        protected Vector3 horizontalMomentum;
        protected float normalizedPassedTime;

        protected bool isFirstFrame;
        protected bool isAnimationEnd;


        public BaseAttackingStateForEnemy(Enemy _enemy, ActionStateMachineForEnemy _stateMachine) : base(_enemy, _stateMachine)
        {
            isOnLandState = false;
            isAttackingState = true;
            combatSystem = _enemy.CombatSystem;
            attackData = _enemy.AttackDataArray[0];
        }

        public override void Enter()
        {
            base.Enter();

            SetAnimatorIsAttackingParameter(true);
            SetAnimatorSelfStateParameter(true);
            SetAnimatorPlayingSpeed();

            selfForces = attackData.SelfForces;
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
            isAnimationEnd = false;

            //mover.UpdateNextHorizontalMovementVector(Vector3.zero);

            SFXManager.PlayClip(attackData.AttackSoundClip, enemy.transform.position);
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorIsAttackingParameter(false);
            //SetAnimatorSelfStateParameter(false);
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

            //if (normalizedPassedTime > 0.8)
            //{
            //    attackHitBox.Deactivate();
            //    return;
            //}
            //else if (normalizedPassedTime > 0.3)
            //{
            //    attackHitBox.Activate();
            //    return;
            //}

            CalculateThisUpdateMomentum(momentumDampingMode);

            //mover.UpdateNextHorizontalMovementVector(horizontalMomentum);
        }


        private void EndAttackState()
        {
            SearchForTarget();
            if ((runtimeData.Target == null) || (TargetDistanceSqr >= runtimeData.SearchingDistanceSqr * 1.2f))
            {
                    stateMachine.ChangeState(stateMachine.IdlingState);
                    return;
              }

            //if (runtimeData.IsAttackReady)
            //{
            //    if (TargetDistanceSqr <= runtimeData.AttackRangeSqr)
            //    {
            //        if (CalculateAngleToPlayer() <= runtimeData.MaxAttackAngle)
            //        {
            //            stateMachine.ChangeToBasicAttackState();
            //            return;
            //        }
            //    }
            //}

            stateMachine.ChangeState(stateMachine.ChasingState);
            return;

        }


        protected virtual void SetAnimatorPlayingSpeed(bool isExit = false)
        {
            if (isExit)
            {
                animator.speed = 1;
                return;
            }

        }

        protected virtual void UpdateNormalizedPassedTime()
        {
            normalizedPassedTime = GetNormalizedTime(animator, animatorAttackStateTag);
            //if (normalizedPassedTime >= minimumDuration)
            //{
            //    isCancelable = true;
            //}
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
        protected virtual void CalculateThisUpdateMomentum(MomentumDampingMode _momentumDampingMode = MomentumDampingMode.InstantStop)
        {
            if (isAddingForce)
            {
                if (normalizedPassedTime > selfForces[forceIndex].NormalizedTime)
                {
                    momentumDampingMode = selfForces[forceIndex].MomentumDamping;
                    _momentumDampingMode = momentumDampingMode;
                    AddForce(selfForces[forceIndex].Force);
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

        protected virtual void AddForce(Vector3 delta) { horizontalMomentum += enemy.transform.TransformDirection(delta); }

        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            //base.SetAnimatorSelfStateParameter(isOn);
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantAttackHash, 0.25f);
            }
        }

        protected virtual void SetAnimatorIsAttackingParameter(bool isOn)
        {
            animator.SetBool(attackHash, isOn);
        }

    }
}
