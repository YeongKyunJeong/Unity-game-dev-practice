using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class InAirStateForPlayer : ActionStateForPlayer
    {

        protected Vector3 horizontalMomentum;
        protected Vector3 verticalVelocityVector;
        protected bool isFirstFixedUpdate;

        protected readonly int inAirHash = Animator.StringToHash("@InAir");

        public InAirStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {

        }


        #region IStateMethods

        public override void Enter()
        {
            base.Enter();

            SetAnimatorInAirParameter(true);

            horizontalMomentum = runtimeData.HorizontalMovementVector;

            verticalVelocityVector = Vector3.zero;
            isFirstFixedUpdate = true;
        }

        #endregion


        protected virtual void SetAnimatorInAirParameter(bool isOn)
        {
            animator.SetBool(inAirHash, isOn);
        }


    }
}
