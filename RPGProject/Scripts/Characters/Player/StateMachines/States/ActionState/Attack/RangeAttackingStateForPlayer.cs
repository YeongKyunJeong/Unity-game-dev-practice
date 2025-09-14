using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class RangeAttackingStateForPlayer : BaseAttackStateForPlayer
    {
        private readonly int instantRangeSkillHash = Animator.StringToHash("Attack.RangeSkill");
        private readonly int isRangeSkillHash = Animator.StringToHash("IsRangeSkill");
        private readonly int rangeSkillIndexHash = Animator.StringToHash("RangeSkillIndex");

        protected Projectile skillProjectile;
        protected float ShootTime;
        protected bool isShot;
        protected float vFXStartTime;
        protected bool vFXStarted;

        public RangeAttackingStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine) : base(_player, _stateMachine)
        {
        }

        //public override void Enter()
        //{
        //    base.Enter();

        //}

        public override void Enter(int dataKey)
        {
            attackData = attackDataLibrary.GetRangeAttackInfo(dataKey);
            base.Enter();

            ShootTime = attackData.Projectiles[0].ShootStartTime;

            if (attackData.VFXName.Length > 0)
            {
                vFXStartTime = attackData.VFXStartTime;
                vFXStarted = false;
            }
            else vFXStarted = true;
            isShot = false;
            // TODO :: Add resource using logic

        }
        public override void CallUpdate()
        {
            base.CallUpdate();
            if (!vFXStarted)
            {
                if (normalizedPassedTime >= vFXStartTime)
                {
                    VFXManager.PlayVFXEffect(attackData.VFXName, player.transform.position + runtimeData.AttackPositionModifier, player.transform.forward);
                    vFXStarted = true;
                }
            }


            if (isShot) return;

            if (normalizedPassedTime >= ShootTime)
            {
                combatSystem.ChangeStamina(-attackData.StaminaCost);
                combatSystem.ChangeMana(-attackData.MPCost);
                skillProjectile = ProjectileManager.ShootProjectile(attackData, statHandler.CurrentStatistics.Attack,
                                        player.CurrentWeapon.WeaponData.DamageBonus[player.CurrentWeapon.ItemInstance.Upgrade],
                                        combatSystem, attackData.Projectiles[0], player.transform.position, player.transform.forward);
                //skillProjectile.EnterEvent += OnProjectileHit;
                isShot = true;
            }
        }

        public override void Exit()
        {
            //skillProjectile.EnterEvent -= OnProjectileHit;
            base.Exit();
        }

        protected override void SetAnimatorSelfStateParameter(bool isOn)
        {
            if (isOn)
            {
                animator.SetFloat(rangeSkillIndexHash, attackData.AnimationKey);
                if (animator.IsInTransition(0))
                {
                    animator.CrossFadeInFixedTime(instantRangeSkillHash, 0.25f);
                }
            }
            animator.SetBool(isRangeSkillHash, isOn);
        }
    }
}
