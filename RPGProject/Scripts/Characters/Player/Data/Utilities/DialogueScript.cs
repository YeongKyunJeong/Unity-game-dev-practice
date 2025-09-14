using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RSP2
{
    [Serializable]
    public class DialogueScript
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Key;

        /// <summary>
        /// Script index
        /// </summary>
        public int Index;

        /// <summary>
        /// 0: Narrator, 1: Player, 2: NPC
        /// </summary>
        public int Talker;

        /// <summary>
        /// Script to be displayed
        /// </summary>
        public string ScriptContent;
    }


    public class DialogueScriptsLoader
    {
        public List<DialogueScript> ItemsList { get; private set; }
        public Dictionary<int, DialogueScript> ItemsDictByKey { get; private set; }

        public DialogueScriptsLoader(string path = "JSON/Dialogue/DialogueScripts")
        {
            string jsonData;
            jsonData = Resources.Load<TextAsset>(path).text;
            ItemsList = JsonUtility.FromJson<Wrapper>(jsonData).Items;
            ItemsDictByKey = new Dictionary<int, DialogueScript>();
            foreach (var item in ItemsList)
            {
                ItemsDictByKey.Add(item.Key, item);
            }
        }

        [Serializable]
        private class Wrapper
        {
            public List<DialogueScript> Items;
        }

        public DialogueScript GetByKey(int key)
        {
            if (ItemsDictByKey.ContainsKey(key))
            {
                return ItemsDictByKey[key];
            }
            return null;
        }

        public DialogueScript GetByIndex(int index)
        {
            if (index >= 0 && index < ItemsList.Count)
            {
                return ItemsList[index];
            }
            return null;
        }

        public DialogueScript[] GetByMultipleKeys(List<int> keys)
        {
            return ItemsList.Where(x => keys.Contains(x.Key)).ToArray();
        }
    }
}
