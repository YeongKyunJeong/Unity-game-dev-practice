using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 70f;
        [SerializeField] private GameObject impactEffect;
        private int damage;

        [SerializeField] bool isExplosive = false;

        private Transform target;
        private Vector3 dir;
        private float distanceThisFrame;
        private LayerMask enemyLayerMask;

        private GameObject impactEffectGO;
        #region Explosion
        [SerializeField] float explosionRadius;
        private Collider[] swallowedColldiers;
        #endregion

        public void Seek(Transform _target, int setDamage)
        {
            damage = setDamage;
            enemyLayerMask = LayerMask.GetMask("Enemy");
            target = _target;
        }

        private void Update()
        {
            if (target == null)
            {
                Destroy(gameObject);
                return;
            }

            dir = target.position - transform.position;
            distanceThisFrame = speed * Time.deltaTime;

            if (dir.magnitude <= distanceThisFrame) // hit
            {
                HitTarget();
                return;
            }

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
        }

        private void HitTarget()
        {
            impactEffectGO = Instantiate(impactEffect, transform.position, transform.rotation);
            Destroy(impactEffectGO, 2f);

            if (isExplosive)
            {
                Explode();
            }
            else
            {
                Damage(target.gameObject);
            }
            Destroy(gameObject);

        }

        private void Explode()
        {
            swallowedColldiers = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayerMask);
            foreach (Collider collider in swallowedColldiers)
            {
                Damage(collider.gameObject);
            }
        }

        void Damage(GameObject hitEnemyGO)
        {
            hitEnemyGO.GetComponent<Enemy>()?.TakeDamage(damage);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
