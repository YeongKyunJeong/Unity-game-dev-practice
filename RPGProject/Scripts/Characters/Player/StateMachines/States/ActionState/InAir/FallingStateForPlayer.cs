using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class FallingStateForPlayer : InAirStateForPlayer
    {

        //protected readonly int isJumpingHash = Animator.StringToHash("IsJumping");
        private readonly int instantFallingHash = Animator.StringToHash("InAir.Falling");

        public FallingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
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
            mover.UpdateNextVerticalVelocityVector(verticalVelocityVector);
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

                if (moveInput == Vector2.zero)
                {
                    //Debug.Log("No Input : Falling State");
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
        }

        private void SendMovementData()
        {
            runtimeData.VerticalVelocityVector = verticalVelocityVector;

            mover.UpdateNextHorizontalMovementVector(horizontalMomentum);
            mover.UpdateNextVerticalVelocityVector(verticalVelocityVector);
        }


        #region Movement Input Method

        //protected override void OnMoveInput(Vector2 _moveInput)
        //{
        //    base.OnMoveInput(_moveInput);
        //}

        #endregion

    }
}
