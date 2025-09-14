using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class StatHandlerForEnemy : StatHandlerForCharacter
    {

        public StatForEnemy EnemyBaseStatistics;
        public StatForEnemy EnemyCurrentStatistics;

        private Enemy enemy;
        private RuntimeDataForEnemy runtimeData;
        private Coroutine attackCoolTimeCoroutune;

        public void Initialize(Enemy _enemy, StatTableForEnemy baseStatisticsTable)
        {
            enemy = _enemy;
            runtimeData = _enemy.RuntimeData;
            //StatTableForEnemy OriginalLoadedDataTable = baseStatisticsTable;
            EnemyBaseStatistics = new StatForEnemy(baseStatisticsTable);
            EnemyCurrentStatistics = new StatForEnemy(baseStatisticsTable);

            SetRuntimDataAndCombatSystem();
        }


        public void Initialize(NPC nPC, StatTableForNPC baseStatisticsTable)
        {
            enemy = nPC as Enemy;
            runtimeData = enemy.RuntimeData;
            //StatTableForNPC OriginalLoadedDataTable = baseStatisticsTable;
            EnemyBaseStatistics = new StatForNPC(baseStatisticsTable);
            EnemyCurrentStatistics = new StatForNPC(baseStatisticsTable);

            nPC.Name = EnemyBaseStatistics.Name;
            nPC.DialogueKey = baseStatisticsTable.DialogueStartKey;
            bool[] interactions = new bool[2] { false, false };

            if (baseStatisticsTable.HasDialogue)
            {
                nPC.HasDialogue = true;
                interactions[0] = true;

            }
            if (baseStatisticsTable.Tradable) interactions[1] = true;
            nPC.InteractionHitBox.Initialize(nPC, interactions);

            SetRuntimDataAndCombatSystem(true);
        }

        public void Initialize(Enemy _enemy, StatForEnemy initialStatistics)
        {
            enemy = _enemy;
            runtimeData = _enemy.RuntimeData;
            EnemyBaseStatistics = new StatForEnemy(initialStatistics);
            EnemyCurrentStatistics = new StatForEnemy(initialStatistics);

            SetRuntimDataAndCombatSystem();
        }

        private void SetRuntimDataAndCombatSystem(bool isNPC = false)
        {
            if (combatSystem == null)
            {
                combatSystem = GetComponent<CombatSystem>();
            }

            combatSystem.MyFaction = CurrentStatistics.Faction;

            runtimeData.ChasingTargetType = EnemyCurrentStatistics.ChasingTargetType;

            if (isNPC)
            {
                runtimeData.IsHostile = false;
            }
            else
            {
                runtimeData.IsHostile = true;
            }

            runtimeData.SearchingDistance = EnemyCurrentStatistics.SearchingDistance;
            runtimeData.SearchingDistanceSqr = EnemyCurrentStatistics.SearchingDistance * EnemyCurrentStatistics.SearchingDistance;

            //SetAttackRange(enemy.AttackHitBox.GetNowColliderAttackRange);
            SetAttackRange();

            CalculateFinalStat();
        }

        //public void SetAttackRange(float range)
        //{
        //    runtimeData.AttackRange = range;
        //    runtimeData.AttackRangeSqr = range * range;

        //    runtimeData.MinChasingDistance = runtimeData.AttackRange / 2;
        //    runtimeData.MinChasingDistanceSqr = runtimeData.AttackRangeSqr / 4;
        //}

        public void SetAttackRange()
        {
            float range = (enemy.AttackDataArray[0].ColliderSize.z + enemy.AttackDataArray[0].ColliderPosition.z);

            runtimeData.AttackRange = range;
            runtimeData.AttackRangeSqr = range * range;

            runtimeData.MinChasingDistanceSqr = runtimeData.AttackRangeSqr * 0.9f;
            runtimeData.MinChasingDistance = runtimeData.AttackRange * 0.81f;

            runtimeData.MaxAttackAngle = Mathf.Atan2(enemy.AttackDataArray[0].ColliderSize.x, range) * Mathf.Rad2Deg;
        }

        protected override void CalculateFinalStat()
        {
            BaseStat = EnemyBaseStatistics as StatForCharacter;
            CurrentStatistics = EnemyCurrentStatistics as StatForCharacter;
            base.CalculateFinalStat();
        }

        public void StartAttackCoroutine(float coolTime)
        {
            if (attackCoolTimeCoroutune != null)
            {
                StopCoroutine(attackCoolTimeCoroutune);
            }

            attackCoolTimeCoroutune = StartCoroutine(AttackCoolTimeStart(coolTime));
        }

        private IEnumerator AttackCoolTimeStart(float coolTime)
        {
            if (coolTime <= 0) yield return null;

            runtimeData.IsAttackReady = false;
            yield return new WaitForSeconds(coolTime);

            runtimeData.IsAttackReady = true;
            attackCoolTimeCoroutune = null;
            yield return null;
        }
    }
}
