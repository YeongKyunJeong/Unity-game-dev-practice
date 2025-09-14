using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class AttackHitBoxForEnemy : AttackHitBox
    {
        private Enemy enemy;
        private bool isInAttacking;

        public float GetNowColliderAttackRange
        {
            get
            {
                SphereCollider thisCollider = hitBoxCollider as SphereCollider;
                return (thisCollider.center.z + thisCollider.radius) * 0.9f;
            }
        }

        public event Action<CombatSystem> TargetDetectingEvent;

        public override void Initialize(LayerMask _targetLayerMask)
        {
            base.Initialize(_targetLayerMask);
            enemy = GetComponentInParent<Enemy>();
            //SphereCollider thisCollider = hitBoxCollider as SphereCollider;
            //enemy.StatHandler.SetAttackRange(thisCollider.center.z + thisCollider.radius);

            isInAttacking = false;
            //enemy.ActionStateMachine.AttackingEvent += OnAttacking;

        }


        //protected void OnAttacking(bool isStart)
        //{
        //    isInAttacking = isStart;
        //}

        //protected override void OnTriggerEnter(Collider other)
        //{
        //    base.OnTriggerEnter(other);
        //    return;
        //}

    }
}
