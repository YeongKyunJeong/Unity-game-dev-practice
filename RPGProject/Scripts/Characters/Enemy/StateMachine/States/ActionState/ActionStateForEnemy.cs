using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RSP2
{
    public class ActionStateForEnemy : IState
    {
        protected Enemy enemy;

        protected RuntimeDataForEnemy runtimeData;

        protected ActionStateMachineForEnemy stateMachine;
        protected StatHandlerForEnemy statHandler;
        protected MoverForEnemy mover;
        protected CharacterController controller;
        protected Animator animator;

        protected AnimatorStateInfo animationStateInfo;

        private Transform enemyTransform;

        private Collider[] hitColliders;
        private int count;
        private CombatSystem detectedCombatSystem;
        protected Vector3 moveDir;

        private Vector3 targetVector;
        public Vector3 TargetVector
        {
            get
            {
                if (!isTargetVectorThisFrame)
                {
                    GetAndSaveTargetVector();
                }
                return targetVector;
            }
        }
        private float targetDistanceSqr;
        public float TargetDistanceSqr
        {
            get
            {
                if (!isTargetVectorThisFrame)
                {
                    GetAndSaveTargetVector();
                }
                return targetDistanceSqr;
            }
        }

        protected bool isAttackingState;
        protected bool isOnLandState;
        private bool isTargetVectorThisFrame = false;


        public ActionStateForEnemy(Enemy _enemy, ActionStateMachineForEnemy _stateMachine)
        {
            enemy = _enemy;
            stateMachine = _stateMachine;

            runtimeData = enemy.RuntimeData;
            statHandler = _enemy.StatHandler;
            mover = enemy.Mover;
            controller = enemy.Controller;
            animator = enemy.Animator;

            hitColliders = new Collider[50];
            enemyTransform = enemy.transform;

        }

        #region IState Methods

        public virtual void Enter()
        {

        }

        public void Enter(int dataKey)
        {

        }

        public virtual void Exit()
        {

        }

        public virtual void CallUpdate()
        {
            if (!runtimeData.IsHostile)
            {
                stateMachine.SetDefaultState();
                return;
            }

            isTargetVectorThisFrame = false;
        }

        public virtual void CallPhysicsUpdate()
        {
        }


        public virtual void OnAnimationEnterEvent()
        {
        }

        public virtual void OnAnimationExitEvent()
        {
        }

        public virtual void OnAnimationTransitEvent()
        {
        }

        #endregion


        protected virtual void SetAnimatorSelfStateParameter(bool isOn) { }

        protected float GetNormalizedTime(Animator animator, string tag)
        {
            if (animator.IsInTransition(0))
            {
                animationStateInfo = animator.GetNextAnimatorStateInfo(0);
                return animationStateInfo.IsTag(tag) ? animationStateInfo.normalizedTime : -1f;
            }
            else
            {
                animationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return animationStateInfo.IsTag(tag) ? animationStateInfo.normalizedTime : -1f;
            }
        }

        protected Vector3 GetAndSaveTargetVector()
        {
            targetVector = runtimeData.Target.transform.position - enemy.transform.position;
            targetDistanceSqr = targetVector.sqrMagnitude;
            isTargetVectorThisFrame = true;
            return targetVector;
        }

        protected bool SearchForTarget()
        {
            count = Physics.OverlapSphereNonAlloc(enemyTransform.position,
                runtimeData.SearchingDistance, hitColliders, enemy.SearchingLayerMask);

            for (int i = 0; i < count; i++)
            {
                detectedCombatSystem = hitColliders[i].GetComponent<CombatSystem>();

                if (detectedCombatSystem != null
                    && !detectedCombatSystem.IsDead
                    && detectedCombatSystem.MyFaction != enemy.CombatSystem.MyFaction)
                {
                    switch (runtimeData.ChasingTargetType)
                    {
                        case ChasingTargetType.PlayerOnly:
                            {
                                if (detectedCombatSystem.MyFaction == Faction.Player)
                                {
                                    SetTargetData(detectedCombatSystem);

                                    return true;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        case ChasingTargetType.AllFaction:
                            {
                                SetTargetData(detectedCombatSystem);

                                return true;
                            }
                        case ChasingTargetType.NotMyFaction:
                            {
                                if (detectedCombatSystem.MyFaction != enemy.CombatSystem.MyFaction)
                                {
                                    SetTargetData(detectedCombatSystem);

                                    return true;
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        default:
                            {
                                SetTargetData(null, false);
                                return false;
                            }
                    }

                }
            }

            SetTargetData(null, false);
            return false;
        }

        private void SetTargetData(CombatSystem target, bool isSuccess = true)
        {
            if (isSuccess)
            {
                runtimeData.Target = target;
                mover.SetTarget(target.transform);
                return;
            }
            else
            {
                runtimeData.Target = null;
                mover.SetTarget(null);
                return;
            }
        }

        protected bool IsInAttackRange(bool useDistance = false)
        {
            if (runtimeData.Target == null) return false;

            if (runtimeData.Target.IsDead) return false;

            //if (stateMachine.CurrentAttackInfo == null)
            //    SelectAttack();

            if (useDistance)
            {
                //float playerDistanceSqr = (enemy.Target.transform.position - enemy.transform.position).sqrMagnitude;
                // TODO :: Compare with attack distance;
                if (TargetDistanceSqr <= runtimeData.AttackRangeSqr)
                {
                    return true;
                }

                return false;
                //return false;
            }


            //switch (stateMachine.CurrentAttackInfo.DetectionType)
            //{
            //    case DetectionType.WeaponCollider:
            //        return playerDistanceSqr <= 1.5f;

            //    case DetectionType.BoxCast:
            //        return playerDistanceSqr <= stateMachine.CurrentAttackInfo.BoxCastSize.z * stateMachine.CurrentAttackInfo.BoxCastSize.z;
            //}

            return false;
        }

        protected virtual float CalculateAngleToPlayer()
        {
            if (runtimeData.Target == null) return 179f;

            if (runtimeData.Target.IsDead) return 179f;

            Vector3 directionToTarget = TargetVector;
            directionToTarget.y = 0;
            directionToTarget.Normalize();

            Vector3 forward = enemy.transform.forward;
            forward.y = 0;
            forward.Normalize();

            return Vector3.Angle(forward, directionToTarget);
        }

        protected virtual bool IsInSight()
        {
            if (runtimeData.Target == null) return false;

            if (runtimeData.Target.IsDead) return false;

            Vector3 directionToTarget = TargetVector;
            directionToTarget.y = 0;
            directionToTarget.Normalize();

            Vector3 forward = enemy.transform.forward;
            forward.y = 0;
            forward.Normalize();

            float angleToTarget = Vector3.Angle(forward, directionToTarget);


            if (angleToTarget <= enemy.FieldOfView / 2f)
            {
                return true;
            }

            return false;
        }

    }
}
