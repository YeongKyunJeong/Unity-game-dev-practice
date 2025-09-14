using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class HorizontalMovingStateForPlayer : OnLandStateForPlayer
    {
        protected Vector3 horizontalMovementVector;


        //protected Vector2 moveInput;

        public HorizontalMovingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }


        #region IStateMethods

        public override void Enter()
        {
            base.Enter();

            moveInput = runtimeData.MoveInput;
        }

        public override void CallUpdate()
        {
            base.CallUpdate();
            runtimeData.VerticalVelocityVector = new Vector3(0, controller.velocity.y, 0);

            slopeNormalVector = FallingCalculator.CheckIsSlope(player.transform);

            if (FallingCalculator.CheckFalling(runtimeData.VerticalVelocityVector, slopeNormalVector, controller))
            {
                animator.SetBool(onLandHash, false);

                stateMachine.ChangeState(stateMachine.FallingState);
                return;
            }

            if (moveInput == Vector2.zero)
            {
                stateMachine.ChangeState(stateMachine.IdlingState);

                return;
            }

            if (slopeNormalVector.y > 0.98f)
            {
                horizontalMovementVector = ApplySpeedModifierToMovementVector() * InputToDirectionVectorConverter.ConvertInputToMovementDirectionVector(moveInput);

            }
            else
            {
                horizontalMovementVector = ApplySpeedModifierToMovementVector() * InputToDirectionVectorConverter.ConvertInputToMovementDirectionVectorOnSlope(moveInput, slopeNormalVector);

            }

            runtimeData.HorizontalMovementVector = horizontalMovementVector;

            mover.UpdateNextHorizontalMovementVector(horizontalMovementVector);
        }

        #endregion

        protected virtual float ApplySpeedModifierToMovementVector()
        {
            return 1;
        }

    }

}


