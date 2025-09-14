using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class BaseStatTableForPlayer // To do : Use TableLoaderTarget class as parent to other target
    {
        public int key;
        public string Name;
        public int Level;
        public Faction Faction;
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

    public class BaseStatLoaderForPlayer
    {
        public List<BaseStatTableForPlayer> TableList { get; private set; }
        public Dictionary<int, BaseStatTableForPlayer> TableDict { get; private set; }
        private BaseStatTableForPlayer playerStatTable { get; set; }

        public BaseStatLoaderForPlayer(string path = "JSON/Statistics/BaseStatData_Player")
        {
            string loadedTableDataString;
            loadedTableDataString = Resources.Load<TextAsset>(path).text;
            TableList = JsonUtility.FromJson<Wrapper>(loadedTableDataString).Items;
            TableDict = new Dictionary<int, BaseStatTableForPlayer>();
            foreach (var item in TableList)
            {
                TableDict.Add(item.key, item);
            }
            playerStatTable = TableDict[1];
        }

        [System.Serializable]
        private class Wrapper
        {
            public List<BaseStatTableForPlayer> Items;
        }

        public BaseStatTableForPlayer GetStat()
        {
            return playerStatTable == null ? null : playerStatTable;
        }
    }

}
