using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class FallingStateForEnemy : InAirStateForEnemy
    {
        //protected readonly int isJumpingHash = Animator.StringToHash("IsJumping");
        private readonly int instantFallingHash = Animator.StringToHash("InAir.Falling");

        public FallingStateForEnemy(Enemy _enemy, ActionStateMachineForEnemy _stateMachine) : base(_enemy, _stateMachine)
        {

        }


        #region IStateMethods

        public override void Enter()
        {
            base.Enter();

            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantFallingHash, 0.25f);
            }

            verticalVelocityVector = runtimeData.VerticalVelocityVector;
            //mover.UpdateNextVerticalVelocityVector(verticalVelocityVector);

        }

        public override void Exit()
        {
            base.Exit();

        }

        public override void CallUpdate()
        {
            base.CallUpdate();

            CheckIsFirstUpdate();

            SendMovementData();

            CheckIsGrounded();
        }

        #endregion


        private void CheckIsFirstUpdate()
        {
            if (isFirstFixedUpdate)
            {
                isFirstFixedUpdate = false;
            }
            else
            {
                FallingCalculator.ApplyFallingToVector(ref verticalVelocityVector, Time.deltaTime);
            }
        }

        private void CheckIsGrounded()
        {
            if (controller.isGrounded)
            {
                runtimeData.VerticalVelocityVector = 5 * Physics.gravity * Time.deltaTime;

                SetAnimatorInAirParameter(false);

                if (!SearchForTarget()) return;

                if (runtimeData.Target == null)
                {
                    //Debug.Log("No Input : Falling State");
                    stateMachine.ChangeState(stateMachine.IdlingState);
                    // Enter idling state at least one frame
                    return;
                }

                stateMachine.ChangeState(stateMachine.ChasingState);
                return;

            }
        }

        private void SendMovementData()
        {
            runtimeData.VerticalVelocityVector = verticalVelocityVector;

            //mover.UpdateNextHorizontalMovementVector(horizontalMomentum);
            mover.UpdateNextVerticalVelocityVector(verticalVelocityVector);
        }



    }
}
