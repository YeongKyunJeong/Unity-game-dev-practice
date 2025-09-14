using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class Turret : MonoBehaviour, IPoolableObject
    {
        // To do : Load ans set turret data from data file 
        private Transform target;
        private Enemy targetEnemy;
        private Collider[] detectedColldiers;
        private Coroutine detectingCoroutine;
        private Bullet bulletInitializer;
        private float fireCountDown;

        #region Fixed Parameter
        private float detectionInterval;
        private LayerMask enemyLayerMask; // bitMask
        private WaitForSeconds detectionWaitForSec;
        private float turnSpeed;
        #endregion

        [SerializeField] Transform firePoint;

        #region Turret Object Motion Control

        private float distanceToEnemy;
        private float closestDistance;
        private int closestDistIndex;
        [SerializeField] private Transform rotatingPart;
        private Vector3 dir;
        private Vector3 rotation;
        private Quaternion targetQuaternion;

        #endregion

        #region Turret Data
        [Header("General")]
        [SerializeField] public float range;
        [SerializeField] private int damage;

        [Header("Projectile Turret (Defaults)")]
        [SerializeField] private GameObject bulletPrefab; // To do : Use objectpool
        [SerializeField] private float fireRate;

        [Header("Laser Turret")]
        [SerializeField] private float slowAmount = 0.3f;
        [SerializeField] private bool isLaserTurret = false;    // To do : Apply custom editor to show or hide the fields at inspector;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private ParticleSystem impactEffect;
        [SerializeField] private Light impactLight;
        private Vector3 laserImpactDir;
        #endregion


        // temp
        private void Start()
        {
            FirstPoolingInitialize();
            SetTurretData();
        }
        public void FirstPoolingInitialize()
        {
            detectionInterval = 0.125f;
            enemyLayerMask = LayerMask.GetMask("Enemy");
            detectionWaitForSec = new WaitForSeconds(detectionInterval);
            turnSpeed = 10f;

            EachPoolingInitialize();
        }

        public void EachPoolingInitialize()
        {
            gameObject.SetActive(true);
            fireCountDown = 0;
        }

        public void SetTurretData()
        {
            #region Assigning loaded data
            // range, attackInterval, etc
            //range = 15f;
            //damage = 3;
            //fireRate = 2;
            //isLaserTurret = true;
            //
            if (isLaserTurret)
            {
                if (lineRenderer == null)
                {
                    impactEffect = transform.GetChild(0).GetComponent<ParticleSystem>();
                    lineRenderer = GetComponent<LineRenderer>();
                    impactLight = impactEffect.transform.GetChild(0).GetComponent<Light>();
                }
            }
            #endregion

            detectingCoroutine = null;
            detectingCoroutine = StartCoroutine(DetectEnemyCoroutine());
        }

        private void Update()
        {
            if (target == null)
            {
                if (isLaserTurret)
                {
                    if (lineRenderer.enabled)
                    {
                        lineRenderer.enabled = false;
                        impactEffect.Stop();
                        impactLight.enabled = false;
                    }
                }

                return;
            }

            RotateHead();

            if (isLaserTurret)
            {
                Laser();
            }
            else
            {
                if (fireCountDown <= 0f)
                {

                    Shoot();

                    fireCountDown = 1f / fireRate;  // Countdown Initialize
                }
            }

            fireCountDown -= Time.deltaTime;
        }

        private void Laser()
        {
            targetEnemy.TakeDamage(damage * Time.deltaTime);
            targetEnemy.Slow(slowAmount);

            if (!lineRenderer.enabled)
            {
                lineRenderer.enabled = true;
                impactEffect.Play();
                impactLight.enabled = true;
            }

            lineRenderer.SetPosition(0, firePoint.position);
            lineRenderer.SetPosition(1, target.position);

            laserImpactDir = firePoint.position - target.position;
            impactEffect.transform.position = target.position + laserImpactDir.normalized * 1f; // To do : change point
            impactEffect.transform.rotation = Quaternion.LookRotation(laserImpactDir);

        }

        private void Shoot()
        {
            bulletInitializer = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<Bullet>();

            if (bulletInitializer != null)
            {
                bulletInitializer.Seek(target, damage);
            }
        }

        private void RotateHead()
        {
            dir = target.position - transform.position;
            //rotation = Quaternion.LookRotation(dir).eulerAngles;
            targetQuaternion = Quaternion.LookRotation(dir);
            rotation = Quaternion.Lerp(rotatingPart.rotation, targetQuaternion, Time.deltaTime * turnSpeed).eulerAngles;
            rotatingPart.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        }


        private void UpdateTarget()
        {
            detectedColldiers = Physics.OverlapSphere(transform.position, range, enemyLayerMask);
            closestDistance = range;

            if (detectedColldiers.Length > 0)
            {
                closestDistIndex = -2;
                for (int i = 0; i < detectedColldiers.Length; i++)
                {
                    if (detectedColldiers[i] != null)
                    {
                        distanceToEnemy = Vector3.Distance(detectedColldiers[i].transform.position, transform.position); // To do : Change distance from turret to distance to goal
                        if (distanceToEnemy < closestDistance)
                        {
                            closestDistance = distanceToEnemy;
                            closestDistIndex = i;
                        }
                    }
                    else
                    {
                        Debug.Log("Error Check");    // To do : Check error when enemy passes goal
                    }
                }
                if (closestDistIndex > -1)
                {
                    target = detectedColldiers[closestDistIndex].transform;
                    targetEnemy = target.GetComponent<Enemy>();
                }
            }
            else
            {
                target = null;
            }
            return;
        }

        private IEnumerator DetectEnemyCoroutine()
        {
            while (true)
            {
                UpdateTarget();

                yield return detectionWaitForSec;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }

    }

}