using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace RSP2
{
    [System.Serializable]
    public class UserData
    {
        public const int SAVE_DATA_LIMIT = 8;

        public int UserID;
        public string UserName;
        public List<int> SaveNumberingList = new List<int>();

        public static UserData InitialUserData(int userID = 0, string userName = "You")
        {
            UserData userData = new UserData();

            userData.UserID = userID;
            userData.UserName = userName;
            userData.SaveNumberingList.Add(0);

            return userData;
        }
    }

    public class UserDataWriter
    {
        private SaveDataWriter saveDataWriter;
        private UserDataLoader userDataLoader;

        public void Initialize(UserDataLoader _userDataLoader, SaveDataWriter _saveDataWriter)
        {
            userDataLoader = _userDataLoader;
            saveDataWriter = _saveDataWriter;
        }

        public void SaveCurrentData(bool restrictedSaveNumber = true, string userDataPath = "/Json/UserData", string saveDataPath = "/Json/SaveData")
        {
            if (userDataLoader.CurrentUserData == null) return;

            PlayerSaveData newSaveData = InGameManager.Instance.GetCurrentDataToSave();
            UserData userData = userDataLoader.CurrentUserData;
            // New Save Key = Last Save Key +1
            int newSaveKey = userData.SaveNumberingList[userData.SaveNumberingList.Count - 1] + 1;
            // Restrict Save Numbering from 0 to 999
            newSaveKey = newSaveKey > PlayerSaveData.SAVE_DATA_NUMBERING_LIMIT ? 0 : newSaveKey;

            newSaveData.SaveKey = newSaveKey;

            userDataLoader.UpdateCurrentUserData(newSaveData);

            if (SaveUserDataToJson(userData, userDataPath))
            {
                // Restrict Save Number in 8
                if (restrictedSaveNumber)
                {
                    saveDataWriter.RemoveOtherSaveData(userData.SaveNumberingList, userData.UserName, saveDataPath);
                }

                saveDataWriter.SavePlayerDataToJson(newSaveData, userData.UserName, saveDataPath);

                return;
            }
        }



        //public void SaveCurrentUserData(string path = "/Json/UserData")
        //{
        //    if (userDataLoader.CurrentUserData == null) return;

        //    SaveUserDataToJson(userDataLoader.CurrentUserData);
        //}

        public bool SaveUserDataToJson(UserData userData, string path = "/Json/UserData")
        {
            path = string.Concat(Application.persistentDataPath, path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = string.Concat(path, "/", userData.UserID.ToString());

            string jsonData = JsonUtility.ToJson(userData, true);
            try
            {
                File.WriteAllText(path, jsonData);
                Debug.Log("File Was Saved At" + path);
            }
            catch (Exception e)
            {
                Debug.Log("Save Failed : " + e.Message);
                return false;
            }


            return true;
        }
    }

    public class UserDataLoader
    {
        private UserDataWriter userDataWriter;

        private SaveDataLoader saveDataLoader;
        private SaveDataWriter saveDataWriter;

        private UserData userData;
        public UserData CurrentUserData { get => userData; }

        private List<PlayerSaveData> saveDataList = new List<PlayerSaveData>();
        public List<PlayerSaveData> CurrentSaveDataList { get => saveDataList; }

        public void Initialize(UserDataWriter _userDataWriter, SaveDataWriter _saveDataWriter, SaveDataLoader _saveDataLoader, int userID = 0)
        {
            userDataWriter = _userDataWriter;
            saveDataWriter = _saveDataWriter;
            saveDataLoader = _saveDataLoader;
            LoadUserData(userID);
            ///////////////////////////////////////////////////////////////////////
        }


        public UserData LoadUserData(int userID, bool rememberUserData = true, string path = "/Json/UserData")
        {
            if ((userData != null) && (userData.UserID == userID)) return userData;

            path = string.Concat(Application.persistentDataPath, path, "/", userID.ToString());

            if (!File.Exists(path))
            {
                Debug.Log("No User Data Exists");
                userData = UserData.InitialUserData();

                PlayerSaveData initialSaveData = PlayerSaveData.InitialSaveData();
                initialSaveData.UserID = userID;
                saveDataList.Add(initialSaveData);

                if (userDataWriter.SaveUserDataToJson(userData))
                {
                    saveDataWriter.SavePlayerDataToJson(initialSaveData, userData.UserName);
                }

                return userData;
            }

            string loadedUserDataString;
            loadedUserDataString = File.ReadAllText(path);

            if (rememberUserData)
            {
                userData = JsonUtility.FromJson<UserData>(loadedUserDataString);
                foreach (int numbering in userData.SaveNumberingList)
                {
                    PlayerSaveData saveData = saveDataLoader.LoadSaveData(userData.UserName, numbering);
                    if (saveData != null)
                        saveDataList.Add(saveData);
                }

                if (saveDataList.Count == 0)
                {
                    userData.SaveNumberingList.Clear();
                    userData.SaveNumberingList.Add(0);
                    saveDataList.Add(PlayerSaveData.InitialSaveData(userData.UserID));

                    saveDataWriter.SavePlayerDataToJson(saveDataList[0], userData.UserName);
                }

                return userData;
            }

            return JsonUtility.FromJson<UserData>(loadedUserDataString);
        }

        public void UpdateCurrentUserData(PlayerSaveData newSaveData)
        {
            if (saveDataList.Contains(newSaveData)) return;

            if (saveDataList.Count >= UserData.SAVE_DATA_LIMIT)
            {
                saveDataList = saveDataList.GetRange(1, UserData.SAVE_DATA_LIMIT - 1);
                userData.SaveNumberingList = userData.SaveNumberingList.GetRange(1, UserData.SAVE_DATA_LIMIT - 1);
            }

            saveDataList.Add(newSaveData);
            userData.SaveNumberingList.Add(newSaveData.SaveKey);
        }

        public PlayerSaveData LoadLastSaveData(int userID, bool rememberUserData = true, string path = "/Json/UserData")
        {
            if ((userData != null) && (userData.UserID == userID))
            {
                if (saveDataList.Count > 0) return saveDataList[saveDataList.Count - 1];

                //return saveDataLoader.LoadSaveData(userData.UserName, userData.SaveNumberList[userData.SaveNumberList.Count - 1]);
            }

            UserData _userData = LoadUserData(userID, rememberUserData);

            return saveDataLoader.LoadSaveData(_userData.UserName, _userData.SaveNumberingList[_userData.SaveNumberingList.Count - 1]);
        }

        public PlayerSaveData LoadUserAndSaveData(int userID, int saveNumber, bool rememberUserData = true, string path = "/Json/UserData")
        {
            if (userID != userData.UserID)
            {
                LoadUserData(userID, rememberUserData);
            }

            if (userData.SaveNumberingList.Contains(saveNumber))
            {
                return saveDataList.Where(x => x.SaveKey == saveNumber).First();
            }
            else
            {
                Debug.LogWarning("Save Data Not Exists");
                return null;
            }
        }
    }
}