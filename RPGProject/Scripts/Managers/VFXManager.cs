using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class VFXManager : MonoSingleton<VFXManager>
    {
        private InGameManager gameManager;

        [field: SerializeField][Range(0f, 1f)] private float HitEffectPitchVariance;

        private ObjectPool objectPool;

        // TODO :: ADD tags string to call VFX by cases

        [SerializeField] private string DefaultHitVFXTag;
        [SerializeField] private string DefaultLevelUpVFXTag;

        [field: SerializeField] public VFXScriptableObject VFXSOData { get; private set; }

        //private void Awake()
        //{
        //    objectPool = GetComponent<ObjectPool>();
        //}

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;
            objectPool = GetComponent<ObjectPool>();
        }

        public static void PlayHitEffect(DamageType damageType, CombatUnit hitUnit, Vector3 sourcePosition, Vector3 sourceDir, float scaleMultiplier = 1.0f, float speedMultipliyer = 1.0f)
        {
            string PooledObjectTag;
            switch (damageType)
            {
                // TODO :: ADD tags and particleSystem objects by damage type & hit unit
                default:
                    {
                        PooledObjectTag = Instance.DefaultHitVFXTag;
                        break;
                    }
            }

            ParticleSystem vFXParticleSystem = PoolVFX(Instance.DefaultHitVFXTag, sourcePosition, sourceDir);
            var main = vFXParticleSystem.main;
            main.simulationSpeed = 1f + Random.Range(-Instance.HitEffectPitchVariance, Instance.HitEffectPitchVariance);
            main.simulationSpeed *= speedMultipliyer;
        }

        public static void PlayVFXEffect(string VFXName, Vector3 sourcePosition, Vector3 sourceDir, float scaleMultiplier = 1.0f, float speedMultiplier = 1.0f)
        {
            ParticleSystem vFXParticleSystem = PoolVFX(VFXName, sourcePosition, sourceDir);
            var main = vFXParticleSystem.main;
            main.simulationSpeed = 1f + Random.Range(-Instance.HitEffectPitchVariance, Instance.HitEffectPitchVariance);
            main.simulationSpeed *= speedMultiplier;
        }

        public static void PlayLevelUpEffect(Vector3 sourcePosition)
        {
            ParticleSystem vFXParticleSystem = PoolVFX(Instance.DefaultLevelUpVFXTag, sourcePosition, Vector3.up);
            var main = vFXParticleSystem.main;
            main.simulationSpeed = 1f;
        }

        private static ParticleSystem PoolVFX(string tag, Vector3 sourcePosition, Vector3 sourceDir)
        {
            GameObject go = Instance.objectPool.SpawnFromPool(tag);
            go.SetActive(true);
            go.transform.position = sourcePosition;
            go.transform.rotation = Quaternion.LookRotation(sourceDir);
            ParticleSystem particleSystem = go.GetComponent<ParticleSystem>();
            return particleSystem;
        }
    }
}
