using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class BasicMeleeAttackingStateForPlayer : MeleeAttackingStateForPlayer
    {
        //protected readonly int landAttackingHash = Animator.StringToHash("IsLandAttacking");
        private readonly int instantBasicMeleeAttackHash = Animator.StringToHash("Attack.BasicMeleeAttack");
        // To Do : Add combo attack

        public BasicMeleeAttackingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
            attackData = player.SOData.AttackDataLibrary.BaseAttackData;
        }


        #region IState Methods

        public override void Enter()
        {
            base.Enter();
            //base.SetAnimatorSelfStateParameter(isOn);

            if (animator.IsInTransition(0))
            {
                animator.CrossFadeInFixedTime(instantBasicMeleeAttackHash, 0.25f);
            }
        }

        #endregion

        protected override void OnAttackInput()
        {
            if (normalizedPassedTime >= minimumDuration)
            {
                if (stateMachine.SkillMeleeAttackingStates[0].CheckAttackResources(AttackType.MeleeAttackSkill, 0))
                {
                    stateMachine.ChangeStateWithAttackData(stateMachine.SkillMeleeAttackingStates[0], 0);
                }

                return;
            }
        }

        protected override void OnAttack(CombatSystem system, Collider hitCollider)
        {
            base.OnAttack(system, hitCollider);
        }
    }
}
