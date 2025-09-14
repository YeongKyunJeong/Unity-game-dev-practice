using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class ProjectileManager : MonoSingleton<ProjectileManager>
    {
        private InGameManager gameManager;
        private ObjectPool objectPool;
        //public List<Projectile> projectileList;
        private static HashSet<Projectile> projectiles;

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;
            objectPool = GetComponent<ObjectPool>();
            projectiles = new HashSet<Projectile>();
        }
        
        //private void Awake()
        //{
        //    objectPool = GetComponent<ObjectPool>();
        //    projectiles = new HashSet<Projectile>();
        //}

        // Update is called once per frame
        public void CallUpdate()
        {
            //for (int i = 0; i < projectileList.Count; i++)
            //{
            //    projectileList[i].CallUpdate();
            //}

            if (projectiles.Count == 0)
            {
                return;
            }
            foreach (Projectile projectile in projectiles)
            {
                    projectile.CallUpdate();
            }

        }

        public static Projectile ShootProjectile(AttackData attackData, float attackStat, float weaponDamage, CombatSystem combatSystem, ProjectileData projectileData, Vector3 shooterPosition, Vector3 shooterForward, float speedModifier = 1)
        {
            Projectile projectile = PoolProjectile(projectileData.poolTag, shooterPosition, shooterForward);

            projectile.SetData(attackData, attackStat, weaponDamage, combatSystem, projectileData, speedModifier);
            projectiles.Add(projectile);

            return projectile;
        }

        private static Projectile PoolProjectile(string tag, Vector3 shootPosition, Vector3 shootDir)
        {
            GameObject go = Instance.objectPool.SpawnFromPool(tag);
            go.SetActive(true);
            go.transform.position = shootPosition;
            go.transform.rotation = Quaternion.LookRotation(shootDir);
            Projectile projectile = go.GetComponent<Projectile>();
            return projectile;
        }

        //public static void DeHash(Projectile projectile)
        //{
        //    if (projectiles.Contains(projectile))
        //        projectiles.Remove(projectile   );
        //}
    }
}
