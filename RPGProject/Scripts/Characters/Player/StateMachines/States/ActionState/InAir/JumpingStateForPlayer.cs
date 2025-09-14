using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class JumpingStateForPlayer : InAirStateForPlayer
    {
        private readonly int isJumpingHash = Animator.StringToHash("IsJumping");
        private readonly int instantJumpingHash = Animator.StringToHash("InAir.Jumping");

        public JumpingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();

            SetAnimatorSelfStateParameter(true);

            verticalVelocityVector += new Vector3(0, movementStateData.JumpForceModifier, 0);

            ApplyHorizontalMomentum();
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorSelfStateParameter(false);
        }

        public override void CallUpdate()
        {
            base.CallUpdate();

            if (isFirstFixedUpdate)
            {
                isFirstFixedUpdate = false;
            }
            else
            {
                FallingCalculator.ApplyFallingToVector(ref verticalVelocityVector, Time.deltaTime);
            }

            runtimeData.VerticalVelocityVector = verticalVelocityVector;

            mover.UpdateNextHorizontalMovementVector(horizontalMomentum);
            mover.UpdateNextVerticalVelocityVector(verticalVelocityVector);

            if (verticalVelocityVector.y <= 0)
            {
                runtimeData.VerticalVelocityVector = Vector3.zero;
                stateMachine.ChangeState(stateMachine.FallingState);
                return;
            }
        }

        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            //base.SetAnimatorSelfStateParameter(isOn);
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantJumpingHash, 0.25f);
            }
            animator.SetBool(isJumpingHash, isOn);
        }

        private void ApplyHorizontalMomentum() // To Do : Find the way to unite the Method in HorizontalMovingStateForPlayer 
        {
            horizontalMomentum = ApplySpeedModifierToMovementVector() * InputToDirectionVectorConverter.ConvertInputToMovementDirectionVector(moveInput);
            runtimeData.HorizontalMovementVector = horizontalMomentum;
        }

        private float ApplySpeedModifierToMovementVector()
        {
            if (runtimeData.IsWalking)
            {
                return movementStateData.WalkingSpeedModifier;

            }
            return movementStateData.RunningSpeedModifier;
        }
    }
}
