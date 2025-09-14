using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class OnLandStateForPlayer : ActionStateForPlayer
    {
        protected int fallingThresholdCount = 5;
        protected float fallingThreshold;
        protected Vector3 slopeNormalVector;

        protected readonly int onLandHash = Animator.StringToHash("@OnLand");



        public OnLandStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }


        #region IStateMethods

        public override void Enter()
        {
            base.Enter();

            SetOnLandParameter(true);
        }

        public override void Exit()
        {
            base.Exit();

            SetAnimatorSelfStateParameter(false);
        }

        #endregion


        protected override void OnJumpInput()
        {
            base.OnJumpInput();

            SetOnLandParameter(false);

            stateMachine.ChangeState(stateMachine.JumpingState);
        }

        protected override void OnDashInput()
        {
            base.OnDashInput();

            stateMachine.ChangeState(stateMachine.LandDashingState);
        }

        protected override void OnAttackInput()
        {
            base.OnAttackInput();

            if (player.CurrentWeapon != null)
            {

                if (stateMachine.CheckAttackResources(attackDataLibrary.BaseAttackData, player.CombatSystem))
                {
                    SetOnLandParameter(false);

                    stateMachine.ChangeState(stateMachine.BasicMeleeAttackingState);
                    return;

                }
            }
        }

        protected override void OnQSkillInput()
        {
            base.OnQSkillInput();

            if (player.CurrentWeapon != null)
            {

                if (stateMachine.CheckAttackResources(attackDataLibrary.GetRangeAttackInfo(0), player.CombatSystem))
                {
                    SetOnLandParameter(false);

                    stateMachine.ChangeStateWithAttackData(stateMachine.RangeAttackingStates[0], 0);
                    return;

                }
            }

        }

        protected virtual void SetOnLandParameter(bool isOn)
        {
            stateMachine.OnLand(isOn);
            animator.SetBool(onLandHash, isOn);
        }






    }
}
