using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RSP2
{
    public class ActionStateMachineForEnemy : StateMachine
    {
        private Enemy enemy;
        private MoverForEnemy mover;
        private Animator animator;

        private readonly int instantHitHash = Animator.StringToHash("Hit");
        private readonly int instantDyingHash = Animator.StringToHash("Dying");

        #region Action States

        public IdlingStateForEnemy IdlingState { get; private set; }
        public ChasingState ChasingState { get; private set; }

        // temporary class type, change later to exact class

        private MeleeAttackingStateForEnemy MeleeAttackingState { get; set; }
        //private RangeAttackingStateForEnemy RangeAttackingState { get; set; }

        #endregion

        //public bool IsInAttackingState { get; set; }
        private AttackType BasicAttackType { get; set; }
        public event Action<bool> AttackingEvent;


        public ActionStateMachineForEnemy(Enemy _enemy)
        {
            enemy = _enemy;

            mover = _enemy.Mover;

            animator = _enemy.Animator;

            enemy.RuntimeData.isChasingStartEvent += SetDefaultState;

            IdlingState = new IdlingStateForEnemy(_enemy, this);

            ChasingState = new ChasingState(_enemy, this);

            BasicAttackType = enemy.AttackDataArray[0].AttackType;

            if (BasicAttackType == AttackType.MeleeAttackSkill)
            {
                MeleeAttackingState = new MeleeAttackingStateForEnemy(_enemy, this);
            }
            else
            {
                // TODO :: Add RangeAttackingState
                //RangeAttackingState = new RangeAttackingStateForEnemy(_enemy, this);
            }

            SetDefaultState();
        }

        public override void ChangeState(IState nextState)
        {
            if (!enemy.RuntimeData.IsHostile) return;

            base.ChangeState(nextState);
        }

        public override void SetDefaultState()
        {
            base.ChangeState(IdlingState);
            //IsInAttackingState = false;
        }

        public void OnHit()
        {
            // TODO :: Add force
            animator.CrossFadeInFixedTime(instantHitHash, 0.25f);
        }

        public void OnDie()
        {
            //enemy.Mover.UpdateNextHorizontalMovementVector(Vector3.zero);
            
            enemy.Animator.CrossFadeInFixedTime(instantDyingHash, 0.25f);
            currentState.Exit();
            currentState = null; // TODO :: Add dyingState
        }


        public void ChangeToBasicAttackState()
        {
            switch (BasicAttackType)
            {
                case AttackType.MeleeAttackSkill:
                    {
                        ChangeState(MeleeAttackingState);
                        break;
                    }
                case AttackType.RangeAttackSkill:
                    {
                        //ChangeState(RangeAttackingState);
                        break;
                    }
            }
        }
    }
}
