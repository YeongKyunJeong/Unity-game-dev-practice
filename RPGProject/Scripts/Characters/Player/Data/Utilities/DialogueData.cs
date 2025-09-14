using RSP2;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RSP2
{

    [Serializable]
    public class DialogueData
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Key;

        /// <summary>
        /// Name
        /// </summary>
        public string Name;

        /// <summary>
        /// Script Group
        /// </summary>
        public int Group;

        /// <summary>
        /// Random among scripts
        /// </summary>
        public bool Random;

        /// <summary>
        /// Scripts to read
        /// </summary>
        public List<int> ScriptKeys;

        /// <summary>
        /// Next Key
        /// </summary>
        public int Redirection;
    }



    public enum DialogueType
    {
        None,
        NPC,
        Narrator
    }



    public class DialogueDataLoader
    {
        public List<DialogueData> ItemsList { get; private set; }
        public Dictionary<int, DialogueData> ItemsDict { get; private set; }

        public DialogueDataLoader(string path = "JSON/Dialogue/DialogueData")
        {
            string jsonData;
            jsonData = Resources.Load<TextAsset>(path).text;
            ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
            ItemsDict = new Dictionary<int, DialogueData>();
            foreach (var item in ItemsList)
            {
                ItemsDict.Add(item.Key, item);
            }
        }

        [Serializable]
        private class Wrapper
        {
            public List<DialogueData> Items;
        }

        public DialogueData GetByKey(int key)
        {
            if (ItemsDict.ContainsKey(key))
            {
                return ItemsDict[key];
            }
            return null;
        }

        public DialogueData GetByIndex(int index)
        {
            if (index >= 0 && index < ItemsList.Count)
            {
                return ItemsList[index];
            }
            return null;
        }
    }


}