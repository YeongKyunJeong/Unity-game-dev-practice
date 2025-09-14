using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class StatForPlayer : StatForCharacter
    {
        public int Level;

        public StatForPlayer()
        {
            key = -1;

            Name = "Player";

            Level = 1;

            Faction = Faction.Player;

            MaxHP = 10;

            HPRegen = 10;

            MaxMP = 10;

            MPRegen = 10;

            MaxStamina = 10;

            StaminaRegen = 10;

            Attack = 10;

            Defense = 10;

            MovementSpeed = 10;

            AttackSpeed = 10;
        }

        public StatForPlayer(BaseStatTableForPlayer baseStatTable)
        {
            //Debug.Log("Loaded by JSON : Player");
            SetBaseStatByTable(baseStatTable);
        }

        public StatForPlayer(StatForPlayer baseStatistics)
        {
            SetBaseStat(baseStatistics);
        }

        public void SetBaseStatByTable(BaseStatTableForPlayer newDataTable)
        {
            key = newDataTable.key;

            Name = newDataTable.Name;

            Level = newDataTable.Level;

            Faction = newDataTable.Faction;

            MaxHP = newDataTable.MaxHP;

            HPRegen = newDataTable.HPRegen;

            MaxMP = newDataTable.MaxMP;

            MPRegen = newDataTable.MPRegen;

            MaxStamina = newDataTable.MaxStamina;

            StaminaRegen = newDataTable.StaminaRegen;

            Attack = newDataTable.Attack;

            Defense = newDataTable.Defense;

            MovementSpeed = newDataTable.MovementSpeed;

            AttackSpeed = newDataTable.AttackSpeed;
        }

        private void SetBaseStat(StatForPlayer newData)
        {
            key = newData.key;

            Name = newData.Name;

            Level = newData.Level;

            Faction = newData.Faction;

            MaxHP = newData.MaxHP;

            HPRegen = newData.HPRegen;

            MaxMP = newData.MaxMP;

            MPRegen = newData.MPRegen;

            MaxStamina = newData.MaxStamina;

            StaminaRegen = newData.StaminaRegen;

            Attack = newData.Attack;

            Defense = newData.Defense;

            MovementSpeed = newData.MovementSpeed;

            AttackSpeed = newData.AttackSpeed;
        }

        public void SetStatByLevelTable(LevelStatTable newStat)
        {
            Level = newStat.Level;

            MaxHP = newStat.MaxHP;

            HPRegen = newStat.HPRegen;

            MaxMP = newStat.MaxMP;

            MPRegen = newStat.MPRegen;

            MaxStamina = newStat.MaxStamina;

            StaminaRegen = newStat.StaminaRegen;

            Attack = newStat.Attack;

            Defense = newStat.Defense;

        }

    }
}
