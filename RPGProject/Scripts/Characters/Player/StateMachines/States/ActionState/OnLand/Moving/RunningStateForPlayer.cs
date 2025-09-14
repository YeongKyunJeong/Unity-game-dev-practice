using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class RunningStateForPlayer : HorizontalMovingStateForPlayer
    {
        private readonly int instantRunnigHash = Animator.StringToHash("OnLand.Running");

        public RunningStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantRunnigHash, 0.25f);
            }
        }

        protected override void OnWalkToggleInput()
        {
            base.OnWalkToggleInput();

            stateMachine.ChangeState(stateMachine.WalkingState);
        }


        protected override float ApplySpeedModifierToMovementVector()
        {
            return movementStateData.RunningSpeedModifier;
        }

    }
}
