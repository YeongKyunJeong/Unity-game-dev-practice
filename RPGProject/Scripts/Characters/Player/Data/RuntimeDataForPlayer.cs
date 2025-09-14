using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class RuntimeDataForPlayer
    {
        public bool IsWalking;
        public Vector2 MoveInput { get; set; }
        public Vector3 HorizontalMovementVector { get; set; }
        public Vector3 VerticalVelocityVector { get; set; }
        public Vector3 AttackPositionModifier { get; set; }


        public RuntimeDataForPlayer()
        {
            IsWalking = false;
        }

    }
}
