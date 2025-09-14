using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class StatTableForEnemy
    {
        public int key;
        public string Name;
        public int Exp;
        public Faction Faction;
        public ChasingTargetType ChasingTargetType;
        public float SearchingDistance;
        public float MaxHP;
        public float HPRegen;
        public float MaxMP;
        public float MPRegen;
        public float MaxStamina;
        public float StaminaRegen;
        public float Attack;
        public float Defense;
        public float MovementSpeed;
        public float AttackSpeed;
    }

    public class StatLoaderForEnemy
    {

        public List<StatTableForEnemy> TableList { get; private set; }
        public Dictionary<int, StatTableForEnemy> TableDict { get; private set; }
        private StatTableForEnemy enemyStatTable { get; set; }

        public StatLoaderForEnemy(string path = "JSON/Statistics/StatData_Enemy")
        {
            string loadedTableDataString;
            loadedTableDataString = Resources.Load<TextAsset>(path).text;
            TableList = JsonUtility.FromJson<Wrapper>(loadedTableDataString).Items;
            TableDict = new Dictionary<int, StatTableForEnemy>();
            foreach (var item in TableList)
            {
                TableDict.Add(item.key, item);
            }
            enemyStatTable = TableDict[0];
        }

        [Serializable]
        private class Wrapper
        {
            public List<StatTableForEnemy> Items;
        }

        public StatTableForEnemy GetStat()
        {
            return enemyStatTable == null ? null : enemyStatTable;
        }

        public StatTableForEnemy GetByKey(int key)
        {
            if (TableDict.ContainsKey(key))
            {
                return TableDict[key];
            }
            return null;
        }
        public StatTableForEnemy GetByIndex(int index)
        {
            if (index >= 0 && index < TableList.Count)
            {
                return TableList[index];
            }
            return null;
        }

    }
}