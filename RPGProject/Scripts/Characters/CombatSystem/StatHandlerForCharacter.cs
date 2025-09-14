using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public enum DamageType
    {
        Slashing,
        Blunging,
        Piercing,
        ByMainWeapon
    }

    public class StatHandlerForCharacter : MonoBehaviour
    {
        protected InGameManager inGameManager;
        protected DataManager dataManager;

        private StatForCharacter baseStat;
        public virtual StatForCharacter BaseStat
        {
            get
            {
                if (baseStat == null) InitializeByDefault();
                return baseStat;
            }
            protected set { baseStat = value; }
        }
        private StatForCharacter currentStatistics;
        public virtual StatForCharacter CurrentStatistics
        {
            get
            {
                if (currentStatistics == null) InitializeByDefault();
                return currentStatistics;
            }
            protected set { currentStatistics = value; }
        }
        protected CombatSystem combatSystem;

        public void InitializeByDefault()
        {
            BaseStat = new StatForCharacter();
            BaseStat.InitializeByDefault();
            CurrentStatistics = new StatForCharacter();
            CurrentStatistics.InitializeByDefault();

            if (combatSystem == null)
            {
                combatSystem = GetComponent<CombatSystem>();
            }

            CalculateFinalStat();
        }

        public void Initialize(StatForCharacter initialStatistics)
        {
            BaseStat = new StatForCharacter(initialStatistics);
            CurrentStatistics = new StatForCharacter(initialStatistics);
            combatSystem = GetComponent<CombatSystem>();

            CalculateFinalStat();
        }

        protected virtual void CalculateFinalStat()
        {
            //if (combatSystem != null)
            //{
            //    if (CurrentStatistics != null)
            //    {
            //        combatSystem.MyFaction = CurrentStatistics.Faction;

            //    }
            //}
            //    int health = baseStat.BaseHealth;
            //    float speed = baseStat.BaseSpeed;
            //    int attack = baseStat.BaseAttack;

            //    int flatHealth = 0;
            //    float flatSpeed = 0;
            //    int flatAttack = 0;

            //    float percentHealth = 1;
            //    float percentSpeed = 1;
            //    float percentAttack = 1;

            //    foreach (StatModifier modifier in modifiers)
            //    {
            //        switch (modifier.statType)
            //        {
            //            case StatType.Attack:
            //                if (modifier.modType == StatModType.Flat)
            //                    flatAttack += (int)modifier.value;
            //                else if (modifier.modType == StatModType.Percent)
            //                    percentAttack += modifier.value / 100f;
            //                break;
            //            case StatType.Speed:
            //                if (modifier.modType == StatModType.Flat)
            //                    flatSpeed += (int)modifier.value;
            //                else if (modifier.modType == StatModType.Percent)
            //                    percentSpeed += modifier.value / 100f;
            //                break;
            //            case StatType.Health:
            //                if (modifier.modType == StatModType.Flat)
            //                    flatHealth += (int)modifier.value;
            //                else if (modifier.modType == StatModType.Percent)
            //                    percentHealth += modifier.value / 100f;
            //                break;
            //        }
            //    }

            //    CurrentStat.Initialize(
            //        (int)((health + flatHealth) * percentHealth),
            //        (speed + flatSpeed) * percentSpeed,
            //        (int)((attack + flatAttack) * percentAttack)
            //    );
        }
    }
}
