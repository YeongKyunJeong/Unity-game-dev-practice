using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace RSP2
{
    public class AttackHitBox : MonoBehaviour
    {
        protected Collider hitBoxCollider;
        public Collider HitBoxCollider
        {
            get
            {
                return hitBoxCollider;
            }

            private set
            {
                hitBoxCollider = value;
            }
        }

        protected Transform hitBoxTransform;
        public Transform HitBoxTransform
        {
            get
            {
                return hitBoxTransform;
            }

            private set
            {
                hitBoxTransform = value;
            }
        }
        public event Action<CombatSystem, Collider> EnterEvent;


        protected LayerMask targetLayerMask;
        public LayerMask TargetLayerMask { get { return targetLayerMask; } }

        protected HashSet<Collider> detectedTarget;
        protected CombatSystem hitCombatSystem;
        private Collider hitCollider;

        public bool IsEnabled { get { return hitBoxCollider.enabled; } }



        public virtual void Initialize(LayerMask _targetLayerMask)
        {
            hitBoxCollider = GetComponent<Collider>();
            Deactivate();
            hitBoxTransform = transform;
            detectedTarget = new HashSet<Collider>();
            if (GetComponent<Player>() == null && GetComponent<Enemy>() == null)
                targetLayerMask = 1 << LayerMask.NameToLayer("Combat Unit");
            targetLayerMask = _targetLayerMask;
        }

        public void Activate()
        {
            if (!hitBoxCollider.enabled)
            {
                detectedTarget.Clear();
                hitBoxCollider.enabled = true;
            }
        }

        public void Deactivate()
        {
            if (hitBoxCollider.enabled)
                hitBoxCollider.enabled = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & targetLayerMask.value) == 0) return;

            FindCombatSystemAndCallEvent(other);
        }

        private void FindCombatSystemAndCallEvent(Collider other)
        {
            if (detectedTarget.Contains(other)) return;

            detectedTarget.Add(other);
            hitCombatSystem = other.GetComponent<CombatSystem>();

            if (hitCombatSystem == null) return;

            EnterEvent?.Invoke(hitCombatSystem, other);
            return;
        }

        public void SendRaycastHitsResults(RaycastHit[] raycastHits, int count)
        {
            for (int i = 0; i < count; i++)
            {
                hitCollider = raycastHits[i].collider;
                FindCombatSystemAndCallEvent(hitCollider);
            }

            //foreach (var hit in raycastHits)
            //{
            //    hitCollider = hit.collider;
            //    FindCombatSystemAndCallEvent(hit.collider);
            //}
        }

        public void StartRayCasting()
        {
            detectedTarget.Clear();
        }

    }
}
