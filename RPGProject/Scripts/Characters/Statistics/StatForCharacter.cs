using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSP2
{
    public class StatForCharacter : IStatForCharacter
    {
        public int key { get; set; }
        public string Name { get; set; }
        public Faction Faction { get; set; }
        public float MaxHP { get; set; }
        public float HPRegen { get; set; }
        public float MaxMP { get; set; }
        public float MPRegen { get; set; }
        public float MaxStamina { get; set; }
        public float StaminaRegen { get; set; }
        public float Attack { get; set; }
        public float Defense { get; set; }
        public float MovementSpeed { get; set; }
        public float AttackSpeed { get; set; }

        public StatForCharacter()
        {
            //Debug.Log("Initialized Without Data");
        }

        public virtual void InitializeByDefault()
        {
            key = -1;

            Name = "Default";

            Faction = Faction.Enemy;

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

        public StatForCharacter(StatForCharacter baseStatistics)
        {
            SetStatistics(baseStatistics);
        }

        private void SetStatistics(StatForCharacter newData)
        {
            key = newData.key;

            Name = newData.Name;

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
    }
}
