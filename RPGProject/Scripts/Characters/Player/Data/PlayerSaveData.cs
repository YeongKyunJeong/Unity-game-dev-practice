using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace RSP2
{
    [System.Serializable]
    public class PlayerSaveData
    {
        public const int SAVE_DATA_NUMBERING_LIMIT = 999;

        [Header("Meta Data")]
        public int UserID;
        public int SaveKey;
        public string SaveTime;

        [Header("Statistics")]
        public int Exp;
        public int Gold; // TO DO::
        public float HP;
        public float MP;
        public float Stamina;

        [Space]
        [Header("Progress Data")]
        public Vector3 Position;
        public int SceneNumber;
        public List<int> GameProgress;

        [Space]
        [Header("Inventory Data")]
        public List<ItemSaveData> InventoryData = new List<ItemSaveData>();

        [Space]
        [Header("Object Data")]
        public List<QuestSaveData> Quests = new List<QuestSaveData>();
        public List<NPCSaveData> NPCs = new List<NPCSaveData>();

        public static PlayerSaveData InitialSaveData(int initialUserID = 0)
        {
            PlayerSaveData initialSaveData = new PlayerSaveData();

            initialSaveData.UserID = initialUserID;
            initialSaveData.SaveKey = 0;
            //initialSaveData.SaveTime = DateTime.Now.ToString();

            initialSaveData.Exp = 500;
            initialSaveData.Gold = 0;
            initialSaveData.HP = 140;
            initialSaveData.MP = 70;
            initialSaveData.Stamina = 36;

            initialSaveData.Position = new Vector3(0, 0, 0);
            initialSaveData.SceneNumber = 1;
            initialSaveData.GameProgress = new List<int> { 0 };

            initialSaveData.InventoryData = new List<ItemSaveData> {
                                                    new ItemSaveData(
                                                        ItemType.Equipable,
                                                        EquipmentType.Weapon,
                                                        1, 0,false, 1, 1
                                                        ),
                                                    new ItemSaveData(
                                                        ItemType.Consumable,
                                                        EquipmentType.Weapon,
                                                        0, 0,false, 2, 4
                                                        ),
                                                    new ItemSaveData(
                                                        ItemType.Equipable,
                                                        EquipmentType.Weapon,
                                                        0, 1,true, -1, 1
                                                        ),
                                                };

            return initialSaveData;
        }
    }

    [System.Serializable]
    public class ItemSaveData
    {
        public ItemType ItemType;
        public EquipmentType EquipmentType;
        public int ItemKey;
        public int ItemUpgrade;
        public bool IsEquipped;
        public int SlotPosition;
        public int Amount;

        public ItemSaveData(
            ItemType itemType = ItemType.Equipable,
            EquipmentType equipmentType = EquipmentType.Weapon,
            int itemKey = 0,
            int itemUpgrade = 0,
            bool isEquipped = false,
            int slotPosition = 0,
            int amount = 1)
        {
            ItemType = itemType;
            EquipmentType = equipmentType;
            ItemKey = itemKey;
            ItemUpgrade = itemUpgrade;
            IsEquipped = isEquipped;
            SlotPosition = slotPosition;
            Amount = amount;
        }
    }

    [System.Serializable]
    public class QuestSaveData
    {
        public int QuestKey;
        public QuestStatus QuestStatus;
        public List<int> CurrentCounts;
    }

    [System.Serializable]
    public class NPCSaveData
    {
        public int NPCKey;
        public bool NPCStatus;
        public int NPCDialogueKey;
    }

    public class SaveDataWriter
    {
        //public bool SavePlayerDataToJson(int userID, string userName, int saveNumber, string path = "/Json/SaveData")
        //{
        //    PlayerSaveData saveData = InGameManager.Instance.GetCurrentDataToSave();

        //    path = string.Concat(Application.persistentDataPath, path, "/", userName);

        //    if (!Directory.Exists(path))
        //        Directory.CreateDirectory(path);

        //    string jsonData = JsonUtility.ToJson(saveData, true);

        //    path = string.Concat(path, "/Save_", saveNumber.ToString("D3"));
        //    try
        //    {
        //        File.WriteAllText(path, jsonData);
        //        Debug.Log("File Was Saved At" + path);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log("Save Failed : " + e.Message);
        //    }

        //    return true;
        //}

        public bool SavePlayerDataToJson(PlayerSaveData saveData, string userName, string path = "/Json/SaveData")
        {
            path = string.Concat(Application.persistentDataPath, path, "/", userName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string jsonData = JsonUtility.ToJson(saveData, true);

            path = string.Concat(path, "/Save_", saveData.SaveKey.ToString("D3"));
            try
            {
                File.WriteAllText(path, jsonData);
                Debug.Log("File Was Saved At" + path);
            }
            catch (Exception e)
            {
                Debug.Log("Save Failed : " + e.Message);
            }

            return true;
        }

        public void RemoveOtherSaveData(List<int> saveNumberings, string userName, string path = "/Json/SaveData")
        {
            // TO DO :: Remove Other Save
            return;
        }
    }

    public class SaveDataLoader
    {
        private PlayerSaveData playerSaveData { get; set; }

        public PlayerSaveData LoadSaveData(string userName, int saveNumber, string path = "/Json/SaveData")
        {
            if (saveNumber == -1)
            {
                path = string.Concat(Application.persistentDataPath, path, "/", userName, "/AutoSave");
            }
            else
            {
                path = string.Concat(Application.persistentDataPath, path, "/", userName, "/Save_", saveNumber.ToString("D3"));
            }

            if (!File.Exists(path))
            {
                Debug.Log("No Save Exists");
                //playerSaveData = PlayerSaveData.InitialSaveData();
                //return playerSaveData;
                return null;
            }

            string loadedSaveDataString;
            loadedSaveDataString = File.ReadAllText(path);
            playerSaveData = JsonUtility.FromJson<PlayerSaveData>(loadedSaveDataString);

            return playerSaveData;
        }

        public PlayerSaveData GetSaveData()
        {
            return playerSaveData == null ? null : playerSaveData;
        }
    }

}
