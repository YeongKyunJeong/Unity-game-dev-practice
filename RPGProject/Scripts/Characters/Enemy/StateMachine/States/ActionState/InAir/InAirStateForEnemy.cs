using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class InAirStateForEnemy : ActionStateForEnemy
    {

        protected Vector3 horizontalMomentum;
        protected Vector3 verticalVelocityVector;
        protected bool isFirstFixedUpdate;

        protected readonly int inAirHash = Animator.StringToHash("@InAir");

        public InAirStateForEnemy(Enemy _enemy, ActionStateMachineForEnemy _stateMachine) : base(_enemy, _stateMachine)
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
