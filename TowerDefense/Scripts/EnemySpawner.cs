using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class EnemySpawner : MonoBehaviour
    {
        private GameManager gameManager;
        private StageManager stageManager;

        public static int EnemyAliveCount = 0;

        [SerializeField] private ObjectPool enemyPool;

        public Transform enemyPrefab;

        public Transform spawnPoint;

        public Enemy enemyInitializer;
        public Enemy enemyData;

        public float timeBetweenWaves = 2f;
        private float countDown = 2f;
        private bool countDownGoing = true;

        public Wave[] waves;
        private Wave thisRoundWave;
        private int waveIndex = 0;
        private int endLevel;

        private Coroutine coroutineField;
        private float spawnBySpawnTimeFloat = 1f;
        private WaitForSeconds spawnBySpawnTime;

        public void Initialize()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }

            if (stageManager == null)
            {
                stageManager = GameManager.StageManager;
            }

            if (enemyPool == null)
            {
                transform.GetComponent<ObjectPool>();
            }
            enemyPool.Initialize();

            waveIndex = 0;
            EnemyAliveCount = 0;
            countDownGoing = true;
            //endLevel = 2;
            endLevel = waves.Length;
        }


        private void Update()
        {
            if (countDownGoing)
            {
                if (waveIndex == endLevel)
                {
                    if (EnemyAliveCount <= 0)
                    {
                        //Debug.Log("You Won");
                        this.enabled = false;
                        stageManager.WinLevel();
                        return;
                    }
                }

                if (EnemyAliveCount > 0)
                {
                    return;
                }


                if (countDown <= 0f)
                {
                    SpawnWave();
                    countDown = timeBetweenWaves;
                }
                stageManager.ChangeValue(StageUITMPType.WaveCountDown, countDown);
                countDown -= Time.deltaTime;
            }


        }

        void SpawnWave()
        {
            coroutineField = StartCoroutine(SpawnWaveCoroutine());
        }

        IEnumerator SpawnWaveCoroutine()
        {

            countDownGoing = false;
            int thisWaveIndexMax = ++waveIndex; // To do : Change enemy per wave number variation logic 
            PlayerStats.Rounds++;
            thisRoundWave = waves[waveIndex - 1];
            spawnBySpawnTime = new WaitForSeconds(spawnBySpawnTimeFloat / thisRoundWave.rate);
            stageManager.ChangeValue(StageUITMPType.WaveIndex, waveIndex);

            for (int i = 0; i < thisRoundWave.count; i++)
            {
                SpawnEnemy(thisRoundWave.enemy);
                yield return spawnBySpawnTime;
            }

            countDownGoing = true;
            yield return null;
        }

        void SpawnEnemy(Enemy enemyToSpawn)
        {
            enemyInitializer = enemyPool.GetObject<Enemy>(PoolObjectType.Enemy);  // To do : make enemy avatar and use it
            //Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemyInitializer.transform.position = spawnPoint.position;
            enemyInitializer.transform.rotation = spawnPoint.rotation;
            enemyInitializer.SetEnemy(enemyToSpawn);
            EnemyAliveCount++;
        }
    }
}