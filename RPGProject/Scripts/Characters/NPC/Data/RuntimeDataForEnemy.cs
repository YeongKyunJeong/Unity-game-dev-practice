using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class RuntimeDataForEnemy
    {
        public Vector3 HorizontalMovementVector { get; set; }
        public Vector3 VerticalVelocityVector { get; set; }
        public Vector3 AttackPositionModifier { get; set; }


        public float SearchingDistance { get; set; }
        public float SearchingDistanceSqr { get; set; }
        public float MinChasingDistance { get; set; }
        public float MinChasingDistanceSqr { get; set; }
        public float MaxAttackAngle { get; set; }

        public LayerMask SearchingLayerMask { get; set; }
        public float FieldOfView { get; set; }
        public ChasingTargetType ChasingTargetType { get; set; }

        public float RotationSpeedModifier { get; set; }
        public float AttackRange { get; set; }
        public float AttackRangeSqr { get; set; }


        public bool IsAttackReady { get; set; }

        protected bool isHostile;
        public bool IsHostile
        {
            get { return isHostile; }
            set
            {
                isHostile = value;
                isChasingStartEvent?.Invoke();
            }
        }
        public CombatSystem Target { get; set; }

        public event Action isChasingStartEvent;


        public RuntimeDataForEnemy()
        {
            RotationSpeedModifier = 6;
            IsAttackReady = true;
        }


    }
}
