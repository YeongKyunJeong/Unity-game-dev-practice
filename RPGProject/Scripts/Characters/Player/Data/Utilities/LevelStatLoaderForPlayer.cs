using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class LevelStatTable // To do : Use TableLoaderTarget class as parent to other target
    {
        public int key;
        public int Level;
        public float MaxHP;
        public float HPRegen;
        public float MaxMP;
        public float MPRegen;
        public float MaxStamina;
        public float StaminaRegen;
        public float Attack;
        public float Defense;
    }


    public class LevelStatLoaderForPlayer
    {
        public List<LevelStatTable> TableList { get; private set; }
        public Dictionary<int, LevelStatTable> TableDict { get; private set; }

        private LevelStatTable levelStatTable { get; set; }

        public int MaxLevel
        {
            get
            {
                return GetMaxLevel();
            }
        }

        public LevelStatLoaderForPlayer(string path = "JSON/Statistics/LevelStatData_Player")
        {
            string loadedTableDataString;
            loadedTableDataString = Resources.Load<TextAsset>(path).text;
            TableList = JsonUtility.FromJson<Wrapper>(loadedTableDataString).Items;
            TableDict = new Dictionary<int, LevelStatTable>();
            foreach (var item in TableList)
            {
                TableDict.Add(item.key, item);
            }
            levelStatTable = TableDict[1];
        }

        [Serializable]
        private class Wrapper
        {
            public List<LevelStatTable> Items;
        }

        public List<LevelStatTable> GetWholeTableList()
        {
            return TableList == null ? null : TableList;
        }

        public Dictionary<int, LevelStatTable> GetWholeTableDictonary()
        {
            return TableDict == null ? null : TableDict;
        }


        public LevelStatTable GetStatByKey(int key)
        {
            return TableDict.ContainsKey(key) ? TableDict[key] : null;
        }

        public LevelStatTable GetStatByIndex(int index)
        {
            return (index >= 0 && index < TableList.Count) ? TableList[index] : null;
        }

        private int GetMaxLevel()
        {
            return TableList[TableList.Count - 1].Level;
        }
    }
}
