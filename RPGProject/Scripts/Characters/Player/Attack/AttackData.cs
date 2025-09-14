using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public enum AttackType
    {
        Basic,
        MeleeAttackSkill,
        RangeAttackSkill
    }

    public enum DetectionType
    {
        SphereCollider,
        BoxCollider,
        SphereRaycast,
        BoxRaycast
    }

    public enum MomentumDampingMode
    {
        DefaultDamping,
        SoftDamping,
        HardDamping,
        NoDamping,
        InstantStop
    }

    [System.Serializable]
    public struct ForceWithTime
    {
        public MomentumDampingMode MomentumDamping;
        public float NormalizedTime;
        public Vector3 Force;
    }

    [CreateAssetMenu(fileName = "Attack", menuName = "Custom/New Attack Data")]
    public class AttackData : ScriptableObject
    {
        [field: Header("Basic Attack Data Setting")]
        [field: SerializeField] public int ID;
        [field: SerializeField] public int AnimationKey;
        [field: SerializeField] private AttackType attackType = AttackType.Basic;
        public AttackType AttackType { get => attackType; }
        [field: SerializeField] public bool IsComboSkill;
        [field: SerializeField] public ChasingTargetType Target { get; private set; }
        [field: SerializeField] public int hitNumberLimit { get; private set; }
        [field: SerializeField] public string AttackName { get; private set; } = "BasicMeleeAttack";
        [field: SerializeField] public string VFXName { get; private set; }
        [field: SerializeField][Range(0, 1f)] public float VFXStartTime;
        public int AnimatorStateNameHash { get; private set; }
        [field: SerializeField] public LayerMask TargetLayerMask { get; private set; } = 1 << 9;
        //1 << LayerMask.NameToLayer("Combat Unit"); 
        [field: Header("Reference Data")]
        [field: SerializeField] public AudioClip AttackSoundClip;

        [field: Header("General Parameter Data")]
        [field: SerializeField] public float CoolTime { get; private set; }
        [field: SerializeField] public DamageType DamageType { get; private set; }
        [field: SerializeField] public int MPCost { get; private set; }
        [field: SerializeField] public int StaminaCost { get; private set; }
        [field: SerializeField] public int Damage { get; private set; } = 3;
        [field: SerializeField] public int Intensity { get; private set; } = 5;

        [field: Header("Force Settings")]
        [field: SerializeField] public ForceWithTime[] SelfForces { get; private set; }
        [field: SerializeField][field: Range(-50f, 50f)] public float PushForce { get; private set; }

        [field: Header("Time Data Setting")]
        [field: SerializeField][Range(0.1f, 10f)] public float AttackSpeed = 1f;

        [field: SerializeField][Range(0, 1f)] private float hitBoxActivationTime = 0.3f;
        [field: SerializeField][Range(0, 1f)] private float hitBoxDeactivationTime = 0.7f;
        [field: SerializeField][Range(0, 1f)] private float attackRecoveryTime = 0.7f;

        public float HitBoxActivationTime { get => hitBoxActivationTime; }
        public float HitBoxDeactivationTime { get => hitBoxDeactivationTime; }
        public float AttackRecoveryTime { get => attackRecoveryTime; }


        [field: Header("Detection Settings")]
        [field: SerializeField] private DetectionType detectionType = DetectionType.SphereCollider;
        [field: SerializeField] private Vector3 colliderSize = new Vector3(0.8f, 0.8f, 0.8f);
        [field: SerializeField] private Vector3 colliderPosition = new Vector3(0, 1.2f, 1);

        public DetectionType DetectionType { get => detectionType; }
        public Vector3 ColliderSize { get => colliderSize; }
        public Vector3 ColliderPosition { get => colliderPosition; }


        [field: Header("Projectiles")]
        [field: SerializeField] private ProjectileData[] projectiles;

        public ProjectileData[] Projectiles { get => projectiles; }
        //public void GenerateHash()
        //{
        //    AnimatorStateNameHash = Animator.StringToHash(AttackName);
        //}
    }


}
