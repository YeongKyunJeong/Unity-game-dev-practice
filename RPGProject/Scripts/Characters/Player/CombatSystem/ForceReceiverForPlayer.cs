using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RSP2
{
    public class ForceReceiverForPlayer : ForceReceiver
    {
        private Player player;
        private MoverForPlayer mover;

        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<Player>();
            mover = GetComponent<MoverForPlayer>();
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
