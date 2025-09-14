using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class WalkingStateForPlayer : HorizontalMovingStateForPlayer
    {

        private readonly int isWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int instantWalkingHash = Animator.StringToHash("OnLand.Walking");

        public WalkingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }


        #region IStateMethods

        public override void Enter()
        {
            base.Enter();

            SetAnimatorSelfStateParameter(true);
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorSelfStateParameter(false);
        }

        #endregion


        protected override void OnWalkToggleInput()
        {
            base.OnWalkToggleInput();

            stateMachine.ChangeState(stateMachine.RunningState);
        }


        protected override float ApplySpeedModifierToMovementVector()
        {
            return movementStateData.WalkingSpeedModifier;
        }

        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            //base.SetAnimatorSelfStateParameter(isOn);
            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantWalkingHash, 0.25f);
            }
            animator.SetBool(isWalkingHash, isOn);
        }
    }
}
