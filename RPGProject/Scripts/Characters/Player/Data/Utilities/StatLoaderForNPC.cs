using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class StatTableForNPC : StatTableForEnemy
    {
        public bool HasDialogue;
        public int[] DialogueKeys;
        public int DialogueStartKey;
        public bool Tradable;
    }

    public class StatLoaderForNPC
    {
        public List<StatTableForNPC> TableList { get; private set; }
        public Dictionary<int, StatTableForNPC> TableDict { get; private set; }
        private StatTableForNPC nPCStatTable { get; set; }

        public StatLoaderForNPC(string path = "JSON/Statistics/StatData_NPC")
        {
            string loadedTableDataString;
            loadedTableDataString = Resources.Load<TextAsset>(path).text;
            TableList = JsonUtility.FromJson<Wrapper>(loadedTableDataString).Items;
            TableDict = new Dictionary<int, StatTableForNPC>();
            foreach (var item in TableList)
            {
                TableDict.Add(item.key, item);
            }
            nPCStatTable = TableDict[0];
        }

        [Serializable]
        private class Wrapper
        {
            public List<StatTableForNPC> Items;
        }

        public StatTableForNPC GetStat()
        {
            return nPCStatTable == null ? null : nPCStatTable;
        }

        public StatTableForNPC GetByKey(int key)
        {
            if (TableDict.ContainsKey(key))
            {
                return TableDict[key];
            }
            return null;
        }

        public StatTableForNPC GetByIndex(int index)
        {
            if (index >= 0 && index < TableList.Count)
            {
                return TableList[index];
            }
            return null;
        }

    }
}
