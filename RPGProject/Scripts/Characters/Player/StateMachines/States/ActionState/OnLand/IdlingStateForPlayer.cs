using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class IdlingStateForPlayer : OnLandStateForPlayer
    {
        private readonly int isIdlingHash = Animator.StringToHash("IsIdling");
        private readonly int instantIdlingHash = Animator.StringToHash("OnLand.Idling");

        public IdlingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }

        #region IStateMethods

        public override void Enter()
        {
            base.Enter();

            SetAnimatorSelfStateParameter(true);

            moveInput = Vector3.zero;
            runtimeData.HorizontalMovementVector = moveInput;
            mover.UpdateNextHorizontalMovementVector(moveInput);

            //player.RuntimeData.MovementSpeedModifier = defaultSpeedModifier;
            //player.RuntimeData.RotationSpeedModifier = rotationSpeedModifier;

            //player.RuntimeData.TimeToReachTargetYRotation.y = rotationTime;
            //player.RuntimeData.RotationLerpUpdate = Time.fixedDeltaTime/(rotationTime);
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorSelfStateParameter(false);
        }

        public override void CallUpdate()
        {
            base.CallUpdate();
            runtimeData.VerticalVelocityVector = new Vector3(0, controller.velocity.y, 0);
            if (FallingCalculator.CheckFalling(runtimeData.VerticalVelocityVector, Vector3.down, controller))
            {
                SetOnLandParameter(false);
                stateMachine.ChangeState(stateMachine.FallingState);
                return;
            }
        }


        protected override void OnMoveInput(Vector2 moveInput)
        {
            base.OnMoveInput(moveInput);
            //runtimeData.MoveInput = moveInput;


            if (runtimeData.IsWalking)
            {
                stateMachine.ChangeState(stateMachine.WalkingState);
                return;
            }

            stateMachine.ChangeState(stateMachine.RunningState);
            return;
        }

        #endregion


        protected override void OnJumpInput()
        {
            base.OnJumpInput();

            SetOnLandParameter(false);
        }

        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantIdlingHash, 0.25f);
            }
            animator.SetBool(isIdlingHash, isOn);
        }
    }
}
