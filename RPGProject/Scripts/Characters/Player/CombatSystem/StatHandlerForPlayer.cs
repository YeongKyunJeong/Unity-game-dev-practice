using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public enum StatsToDisplay
    {
        //Level,

        //NextExp,
        CurrentExp,

        //MaxHP,
        //MaxMP,
        //MaxStamina,

        CurrentHP,
        CurrentMP,
        CurrentStamina,

        //BaseAttack,
        //WeaponAttack,
        //ArmorDefense,
        //BaseDefense,
        //AttackSpeed,
        //MovementSpeed
    }

    public class StatHandlerForPlayer : StatHandlerForCharacter
    {
        public CombatSystemForPlayer combatSystemForPlayer;
        public StatForPlayer PlayerBaseStatistics { get; private set; }
        public StatForPlayer PlayerCurrentStatistics { get; private set; }

        public ExpDataTable NowLevelExpData { get; private set; }

        public event Action<StatsToDisplay, float> BaseStatChangeEvent;

        public event Action<int, int, int, StatForPlayer, CombatSystem, bool> LevelChangeEvent;


        public int CurrentLevel { get; private set; }
        public int CurrentExp { get; private set; }

        private int maxLevel;

        public void Initialize(BaseStatTableForPlayer baseStatisticsTable)
        {
            PlayerBaseStatistics = new StatForPlayer(baseStatisticsTable);
            PlayerCurrentStatistics = new StatForPlayer(baseStatisticsTable);

            SetStatAndCombatSystem();
        }

        public void Initialize(StatForPlayer initialStatistics)
        {
            PlayerBaseStatistics = new StatForPlayer(initialStatistics);
            PlayerCurrentStatistics = new StatForPlayer(initialStatistics);

            SetStatAndCombatSystem();
        }

        private void SetStatAndCombatSystem()
        {
            combatSystemForPlayer = GetComponent<CombatSystemForPlayer>();
            inGameManager = InGameManager.Instance;
            dataManager = DataManager.Instance;

            CurrentLevel = PlayerBaseStatistics.Level;

            LevelStatTable levelStatTable = dataManager.TableDataLoader.LevelStatLoaderForPlayer.GetStatByKey(CurrentLevel);
            PlayerBaseStatistics.SetStatByLevelTable(levelStatTable);
            PlayerCurrentStatistics.SetStatByLevelTable(levelStatTable);

            maxLevel = dataManager.TableDataLoader.LevelStatLoaderForPlayer.MaxLevel;

            NowLevelExpData = dataManager.TableDataLoader.ExpDataLoader.GetExpByKey(PlayerBaseStatistics.Level);
            CurrentExp = NowLevelExpData.TotalExp - NowLevelExpData.RequiredExp;

            if (combatSystem == null)
            {
                combatSystem = GetComponent<CombatSystem>();
            }

            combatSystem.MyFaction = CurrentStatistics.Faction;

            combatSystem.DamageEvent += OnCurrentHPChange;
            combatSystem.HealEvent += OnCurrentHPChange;

            combatSystem.MPSpendEvent += OnCurrentMPChange;
            combatSystem.MPRecoveryEvent += OnCurrentMPChange;

            combatSystem.StaminaSpendEvent += OnCurrentStaminaChange;
            combatSystem.StaminaRecoveryEvent += OnCurrentStaminaChange;

            //gameManager.EnemyDieEvent += OnEnemyDie;
            EventBus.OnEnemyHunted += OnEnemyDie;

            CalculateFinalStat();
        }

        public void OnEnemyDie(Enemy enemy)
        {
            GainExperience(enemy.CombatSystem.GainExp);
        }

        public void OnEnemyDie(int iD)
        {
            if (iD >= NPC.NPC_KEY_CONST)
            {
                GainExperience(DataManager.Instance.TableDataLoader.StatLoaderForNPC.GetByKey(iD - NPC.NPC_KEY_CONST).Exp);
                return;
            }

            GainExperience(DataManager.Instance.TableDataLoader.StatLoaderForEnemy.GetByKey(iD).Exp);
        }

        public bool GainExperience(int amount, bool levelUpVFX = true)
        {
            CurrentExp += amount;
            BaseStatChangeEvent(StatsToDisplay.CurrentExp, CurrentExp);
            bool isLevelUp = false;

            int limit = 0;

            while (CurrentExp >= NowLevelExpData.TotalExp)
            {
                LevelUp(levelUpVFX);
                isLevelUp = true;
                limit++;
                if (limit >= 100) break;
                if (CurrentExp < NowLevelExpData.TotalExp) break;
            }

            if (isLevelUp)
            {
                return true;
            }

            Debug.Log($"Player got {amount} Exp");
            return false;
        }

        protected override void CalculateFinalStat()
        {
            BaseStat = PlayerBaseStatistics as StatForCharacter;
            CurrentStatistics = PlayerCurrentStatistics as StatForCharacter;
            base.CalculateFinalStat();

            ExpDataTable nextLevelExpData = dataManager.TableDataLoader.ExpDataLoader.GetExpByKey(CurrentLevel);
            if (nextLevelExpData != null)
            {
                LevelChangeEvent?.Invoke(CurrentLevel, nextLevelExpData.TotalExp, CurrentExp,
                    PlayerCurrentStatistics, combatSystem, true);
            }
        }

        private void LevelUp(bool levelUpVFX = true)
        {
            if (CurrentLevel >= maxLevel)
            {
                Debug.Log("Max level reached. No more leveling up.");
                return;
            }

            CurrentLevel++;
            Debug.Log($"Player became Level {CurrentLevel}");

            ExpDataTable nextLevelExpData = dataManager.TableDataLoader.
                                        ExpDataLoader.GetExpByKey(CurrentLevel);
            if (nextLevelExpData != null)
            {
                NowLevelExpData = nextLevelExpData;
                LevelStatTable levelStatTable = dataManager.TableDataLoader.
                                        LevelStatLoaderForPlayer.GetStatByKey(CurrentLevel);
                combatSystemForPlayer.ChangeStatByLevelUp(levelStatTable);
                PlayerBaseStatistics.SetStatByLevelTable(levelStatTable);
                PlayerCurrentStatistics.SetStatByLevelTable(levelStatTable);

                if (levelUpVFX) VFXManager.PlayLevelUpEffect(combatSystem.MyUnit.transform.position);

                LevelChangeEvent?.Invoke(CurrentLevel, nextLevelExpData.TotalExp, CurrentExp,
                    PlayerCurrentStatistics, combatSystem, false);
            }
        }

        private void OnCurrentHPChange(float leftHP, float maxHP)
        {
            BaseStatChangeEvent?.Invoke(StatsToDisplay.CurrentHP, leftHP);
        }

        private void OnCurrentMPChange()
        {
            BaseStatChangeEvent?.Invoke(StatsToDisplay.CurrentHP, combatSystem.CurrentMP);
        }

        private void OnCurrentStaminaChange()
        {
            BaseStatChangeEvent?.Invoke(StatsToDisplay.CurrentHP, combatSystem.CurrentStamina);
        }

        public void SetDataFromSave(PlayerSaveData saveData)
        {
            GainExperience(saveData.Exp, false);
            combatSystemForPlayer.SetDataFromSave(saveData);
        }
    }
}
