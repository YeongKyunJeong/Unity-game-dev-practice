using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace RSP2
{
    public class SkillMeleeAttackingStateForPlayer : MeleeAttackingStateForPlayer
    {
        private readonly int instantComboAttackHash = Animator.StringToHash("Attack.ComboAttack");
        private readonly int isComboHash = Animator.StringToHash("IsCombo");
        private readonly int comboIndexHash = Animator.StringToHash("ComboIndex");

        private readonly int instantMeleeAttackHash = Animator.StringToHash("Attack.MeleeSkill");
        private readonly int isMeleeSkillHash = Animator.StringToHash("IsMeleeSkill");
        private readonly int meleeSkillIndexHash = Animator.StringToHash("MeleeSkillIndex");

        public SkillMeleeAttackingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {

        }

        public override void CallUpdate()
        {
            base.CallUpdate();
        }

        public override void Enter(int dataKey)
        {
            attackData = attackDataLibrary.GetMeleeAttackInfo(dataKey);
            base.Enter();

        }


        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {

            if (attackData.IsComboSkill)
            {
                if (isOn)
                {
                    animator.SetFloat(comboIndexHash, attackData.AnimationKey);
                    if (animator.IsInTransition(0))
                    {
                        animator.CrossFadeInFixedTime(instantComboAttackHash, 0.25f);
                    }
                }
                animator.SetBool(isComboHash, isOn);
            }
            else
            {
                if (isOn)
                {
                    animator.SetFloat(meleeSkillIndexHash, attackData.AnimationKey);
                    if (animator.IsInTransition(0))
                    {
                        animator.CrossFadeInFixedTime(instantMeleeAttackHash, 0.25f);
                    }
                }
                animator.SetBool(isMeleeSkillHash, isOn);

            }

        }

    }
}
