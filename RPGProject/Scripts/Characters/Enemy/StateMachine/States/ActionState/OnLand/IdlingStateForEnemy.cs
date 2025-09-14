using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class IdlingStateForEnemy : OnLandStateForEnemy
    {
        private readonly int isIdlingHash = Animator.StringToHash("IsIdling");
        private readonly int instantIdlingHash = Animator.StringToHash("OnLand.Idling");


        public IdlingStateForEnemy(Enemy _enemy, ActionStateMachineForEnemy _stateMachine) : base(_enemy, _stateMachine)
        {
        }


        #region IState Methods

        public override void Enter()
        {
            base.Enter();

            SetAnimatorSelfStateParameter(true);

            moveDir = Vector3.zero;
            //mover.UpdateNextHorizontalMovementVector(moveDir);

        }
        public override void Exit()
        {
            base.Exit();
        }

        public override void CallUpdate()
        {
            if (!runtimeData.IsHostile) return;

            //if (FallingCalculator.CheckFalling(new Vector3(0, controller.velocity.y, 0), Vector3.down, controller))
            //{
            //    SetAnimatorOnLandParameter(false);
            //    stateMachine.ChangeState(stateMachine.FallingState); // TO DO :: Add fallingstate
            //    return;
            //}

            if (IsInAttackRange())
            {
                if (IsInSight())
                {
                    SetAnimatorOnLandParameter(false);
                    stateMachine.ChangeToBasicAttackState();
                    return;
                }
                else
                {
                    base.CallUpdate();
                    return;
                }
            }
            else if (SearchForTarget())
            {
                stateMachine.ChangeState(stateMachine.ChasingState);
                return;
            }
        }

        #endregion


        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            //base.SetAnimatorSelfStateParameter(isOn);
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantIdlingHash, 0.25f);
            }
            animator.SetBool(isIdlingHash, isOn);
        }
    }
}
