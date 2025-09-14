using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class MeleeAttackingStateForEnemy : BaseAttackingStateForEnemy
    {
        protected AttackHitBox attackHitBox;
        protected float vFXStartTime;
        protected float hitBoxEnableTime;
        protected float hitBoxDisableTime;

        protected Ray ray;
        protected RaycastHit[] hits;
        protected int count;
        protected bool useRaycast;
        protected DetectionType detectionType;
        protected GizmosDrawer gizmosDrawer;
        protected Vector3 attackSize;

        protected bool vFXStarted;
        protected bool isEnabled;
        protected bool isDisabled;

        public MeleeAttackingStateForEnemy(Enemy _enemy, ActionStateMachineForEnemy _stateMachine) : base(_enemy, _stateMachine)
        {
            attackHitBox = _enemy.AttackHitBox;
            gizmosDrawer = enemy.GetComponent<GizmosDrawer>();
            hits = new RaycastHit[50];
        }

        public override void Enter()
        {
            base.Enter();

            attackHitBox.Deactivate();
            attackHitBox.EnterEvent += OnAttack;
            SetAttackDetectorShape();
            isDisabled = false;
            isEnabled = false;

            if (attackData.VFXName.Length > 0)
            {
                vFXStartTime = attackData.VFXStartTime;
                vFXStarted = false;
            }
            else vFXStarted = true;

            hitBoxEnableTime = attackData.HitBoxActivationTime;
            hitBoxDisableTime = Mathf.Min(attackData.HitBoxDeactivationTime, attackData.AttackRecoveryTime);
        }

        public override void Exit()
        {
            base.Exit();
            attackHitBox.Deactivate();
            attackHitBox.EnterEvent -= OnAttack;
        }

        public override void CallUpdate()
        {
            base.CallUpdate();

            if (isDisabled) { return; }

            if (!vFXStarted)
            {
                if (normalizedPassedTime >= vFXStartTime)
                {
                    VFXManager.PlayVFXEffect(attackData.VFXName, enemy.transform.position + runtimeData.AttackPositionModifier, enemy.transform.forward);
                    vFXStarted = true;
                }
            }

            switch (useRaycast)
            {
                case true:
                    {
                        if (normalizedPassedTime >= hitBoxDisableTime)
                        {
                            gizmosDrawer.UpdateParameter(Vector3.zero, Vector3.zero, DetectionType.SphereCollider);
                            isDisabled = true;
                        }
                        else if (normalizedPassedTime >= hitBoxEnableTime)
                        {
                            CastRayAndSendResults();
                            isEnabled = true;
                            return;
                        }
                        break;
                    }
                default:
                    {
                        if (normalizedPassedTime >= hitBoxDisableTime)
                        {
                            isDisabled = true;
                            attackHitBox.Deactivate();
                            return;
                        }

                        if (isEnabled) return;

                        if (normalizedPassedTime >= hitBoxEnableTime)
                        {
                            combatSystem.ChangeStamina(-attackData.StaminaCost);
                            combatSystem.ChangeMana(-attackData.MPCost);
                            isEnabled = true;
                            attackHitBox.Activate();
                            return;
                        }

                        break;
                    }
            }
        }

        private void CastRayAndSendResults()
        {
            if (!isEnabled)
            {
                combatSystem.ChangeStamina(-attackData.StaminaCost);
                combatSystem.ChangeMana(-attackData.MPCost);
                attackHitBox.StartRayCasting();
            }

            attackSize = attackData.ColliderSize;
            switch (detectionType)
            {
                case DetectionType.SphereRaycast:
                    {
                        count = Physics.SphereCastNonAlloc(
                            enemy.transform.position + enemy.transform.TransformDirection(attackData.ColliderPosition),
                            attackSize.x, enemy.transform.position, hits, 1f, attackHitBox.TargetLayerMask);
                    }
                    break;
                case DetectionType.BoxRaycast:
                    {
                        count = Physics.BoxCastNonAlloc(
                            enemy.transform.position + enemy.transform.TransformDirection(attackData.ColliderPosition),
                            attackSize, enemy.transform.forward, hits, enemy.transform.rotation, 1f, attackHitBox.TargetLayerMask);
                    }
                    break;
                default:
                    break;
            }

            attackHitBox.SendRaycastHitsResults(hits, count);
            gizmosDrawer.UpdateParameter(enemy.transform.position + enemy.transform.TransformDirection(attackData.ColliderPosition), attackSize, detectionType);
        }


        protected virtual void OnAttack(CombatSystem hitCombatSystem, Collider hitCollider)
        {
            if (!CheckTargetFaction(hitCombatSystem)) return;

            hitCombatSystem.TakeDamage(statHandler.CurrentStatistics.Attack + attackData.Damage,
                    attackData.DamageType);
            Vector3 attackPosition = enemy.transform.position + runtimeData.AttackPositionModifier;
            Vector3 hitPosition = hitCollider.ClosestPoint(attackPosition);
            VFXManager.PlayHitEffect(attackData.DamageType, hitCombatSystem.MyUnit, hitPosition, (attackPosition - hitPosition).normalized);

        }


        protected virtual void SetAttackDetectorShape()
        {
            detectionType = attackData.DetectionType;
            switch (detectionType)
            {
                case DetectionType.SphereCollider:
                    {
                        useRaycast = false;
                        SphereCollider sphereCollider = attackHitBox.HitBoxCollider as SphereCollider;
                        sphereCollider.radius = attackData.ColliderSize.x;
                        sphereCollider.center = attackData.ColliderPosition;

                        runtimeData.AttackPositionModifier = new Vector3(0, sphereCollider.center.y, 0);
                        break;
                    }

                case DetectionType.BoxCollider:
                    {
                        useRaycast = false;
                        BoxCollider BoxCollider = attackHitBox.HitBoxCollider as BoxCollider;
                        BoxCollider.size = attackData.ColliderSize;
                        BoxCollider.center = attackData.ColliderPosition;

                        runtimeData.AttackPositionModifier = new Vector3(0, BoxCollider.center.y, 0);
                        break;
                    }
                case DetectionType.SphereRaycast:
                    {
                        useRaycast = true;
                        runtimeData.AttackPositionModifier = new Vector3(0, attackData.ColliderPosition.y, 0);
                        break;
                    }

                default:
                    {
                        useRaycast = true;
                        runtimeData.AttackPositionModifier = new Vector3(0, attackData.ColliderPosition.y, 0);
                        break;
                    }
            }
        }

        protected override void SetAnimatorPlayingSpeed(bool isExit = false)
        {
            base.SetAnimatorPlayingSpeed(isExit);

            if (isExit)
            {
                return;
            }
            animator.speed = attackData.AttackSpeed * enemy.StatHandler.CurrentStatistics.AttackSpeed / 5;
        }
    }
}
