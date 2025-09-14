using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RSP2
{
    public enum ChasingTargetType
    {
        PlayerOnly,
        EnemyOnly,
        AllFaction,
        NotMyFaction
    }

    //public enum EnemyBasicAttack
    //{
    //    Melee,
    //    Ranged
    //}

    public class StatForEnemy : StatForCharacter
    {
        public int Exp;

        public ChasingTargetType ChasingTargetType { get; set; }
        public float SearchingDistance { get; set; }

        public StatForEnemy()
        {
            key = -1;

            Name = "Default Enemy";

            Exp = 300;

            Faction = Faction.Enemy;

            ChasingTargetType = ChasingTargetType.PlayerOnly;

            SearchingDistance = 4;

            MaxHP = 50;

            HPRegen = 1;

            MaxMP = 20;

            MPRegen = 1;

            MaxStamina = 20;

            StaminaRegen = 5;

            Attack = 3;

            Defense = 3;

            MovementSpeed = 4;

            AttackSpeed = 5;

            return;
        }

        public StatForEnemy(StatTableForEnemy baseStatisticsTable)
        {
            //Debug.Log("Loaded by JSON : Enemy");
            SetStatisticsByTable(baseStatisticsTable);
        }

        public StatForEnemy(StatForEnemy baseStatistics)
        {
            SetStatistics(baseStatistics);
        }

        public void SetStatisticsByTable(StatTableForEnemy newDataTable)
        {
            key = newDataTable.key;

            Name = newDataTable.Name;

            Exp = newDataTable.Exp;

            Faction = newDataTable.Faction;

            ChasingTargetType = newDataTable.ChasingTargetType;

            SearchingDistance = newDataTable.SearchingDistance;

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

        private void SetStatistics(StatForEnemy newData)
        {
            key = newData.key;

            Name = newData.Name;

            Exp = newData.Exp;

            Faction = newData.Faction;

            ChasingTargetType = newData.ChasingTargetType;

            SearchingDistance = newData.SearchingDistance;

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
    }
}
