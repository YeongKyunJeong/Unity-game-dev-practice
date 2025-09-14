using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class LandDashingStateForPlayer : OnLandStateForPlayer
    {
        private readonly int isDashingHash = Animator.StringToHash("IsDashing");
        private readonly int instantLandDashingHash = Animator.StringToHash("OnLand.Dashing");

        private Vector3 dashMovementVector;
        private Vector3 dampedDashVector;
        private float passedTime; ////////// To Do: Add cool time and chain dash

        private float lerpModifier;
        private float durationTime;
        private float fallingStartTime;
        private Vector3 dampedGravity;
        private Vector3 dampedFallingVelocity;

        public LandDashingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
            //dampedGravity = Physics.gravity*movementStateData.DashFallingModifier;
        }


        #region IState Methods

        public override void Enter()
        {
            base.Enter();

            SetAnimatorSelfStateParameter(true);

            dampedGravity = Physics.gravity * movementStateData.DashFallingModifier;
            dampedFallingVelocity = Vector3.zero;

            lerpModifier = movementStateData.DashingLerpModifier;
            durationTime = movementStateData.DashingDurationTime;
            fallingStartTime = movementStateData.DashFallingDelay * durationTime;

            if (moveInput == Vector2.zero)
            {
                dashMovementVector = movementStateData.DashingSpeedModifier * player.transform.forward.normalized;
            }
            else
            {
                moveInput = runtimeData.MoveInput;
                dashMovementVector = movementStateData.DashingSpeedModifier * InputToDirectionVectorConverter.ConvertInputToMovementDirectionVector(moveInput);
            }

            runtimeData.HorizontalMovementVector = dashMovementVector;
            dampedDashVector = movementStateData.DashingEndSpeedModifier * dashMovementVector;
            passedTime = 0;
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorSelfStateParameter(false);
        }

        public override void CallUpdate()
        {
            base.CallUpdate();

            mover.UpdateNextHorizontalMovementVector(dashMovementVector);
            runtimeData.HorizontalMovementVector = dashMovementVector;


            passedTime += Time.deltaTime;

            if (passedTime < fallingStartTime)
            {
                mover.UpdateNextVerticalVelocityVector(dampedFallingVelocity);

            }
            else
            {
                if (passedTime > durationTime)
                {
                    EndLandDashState();
                    return;
                }

                dashMovementVector = Vector3.Lerp(dashMovementVector, dampedDashVector, lerpModifier);


            }

            //horizontalMovementVector = ApplySpeedModifierToMovementVector() * InputToDirectionVectorConverter.ConvertInputToMovementDirectionVector(moveInput);
            //runtimeData.HorizontalMovementVector = horizontalMovementVector;

            //mover.UpdateNextHorizontalMovementVector(horizontalMovementVector);
        }

        private void EndLandDashState()
        {
            if (FallingCalculator.CheckFalling(dampedFallingVelocity, Vector3.down, controller))
            {
                SetOnLandParameter(false);

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

        #endregion


        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantLandDashingHash, 0.25f);
            }
            animator.SetBool(isDashingHash, isOn);
        }
    }
}
