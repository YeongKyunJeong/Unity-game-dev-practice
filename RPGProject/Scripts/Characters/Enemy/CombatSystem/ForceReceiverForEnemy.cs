using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class ForceReceiverForEnemy : ForceReceiver
    {
        private Enemy enemy;
        private MoverForEnemy mover;
        
        protected override void Awake()
        {
            base.Awake();
            enemy = GetComponent<Enemy>();
            mover = GetComponent<MoverForEnemy>();
        }
        
        public override void CallUpdate()
        {
            base.CallUpdate();
            if (isForced)
                mover.UpdateNextForceVector(force);
        }

        public override void AddForce(Vector3 _force, MomentumDampingMode _momentumDampingMode = MomentumDampingMode.DefaultDamping)
        {
            base.AddForce(_force, _momentumDampingMode);
        }
    }
}
