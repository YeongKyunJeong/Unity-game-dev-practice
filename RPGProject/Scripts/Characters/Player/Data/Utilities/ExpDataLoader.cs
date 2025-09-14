using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    [System.Serializable]
    public class ExpDataTable // To do : Use TableLoaderTarget class as parent to other target
    {
        public int key;
        public int RequiredExp;
        public int TotalExp;
    }


    public class ExpDataLoader
    {
        public List<ExpDataTable> TableList { get; private set; }
        public Dictionary<int, ExpDataTable> TableDict { get; private set; }
        private ExpDataTable ExpDataTable { get; set; }

        public ExpDataLoader(string path = "JSON/Statistics/ExpData")
        {
            string loadedTableDataString;
            loadedTableDataString = Resources.Load<TextAsset>(path).text;
            TableList = JsonUtility.FromJson<Wrapper>(loadedTableDataString).Items;
            TableDict = new Dictionary<int, ExpDataTable>();
            foreach (var item in TableList)
            {
                TableDict.Add(item.key, item);
            }
            ExpDataTable = TableDict[1];
        }

        [Serializable]
        private class Wrapper
        {
            public List<ExpDataTable> Items;
        }

        public List<ExpDataTable> GetWholeExpTableList()
        {
            return TableList == null ? null : TableList;
        }

        public Dictionary<int, ExpDataTable> GetWholeExpTableDictionary()
        {
            return TableDict == null ? null : TableDict;
        }

        public ExpDataTable GetExpByKey(int key)
        {
            return TableDict.ContainsKey(key) ? TableDict[key] : null;
        }

        public ExpDataTable GetExpByIndex(int index)
        {
            return (index >= 0 && index < TableList.Count) ? TableList[index] : null;
        }
    }
}
