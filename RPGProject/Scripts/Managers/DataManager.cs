using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RSP2
{
    public class DataManager : MonoSingleton<DataManager>
    {
        public TableDataLoader TableDataLoader { get; private set; }

        public UserDataWriter UserDataWriter { get; private set; }
        public UserDataLoader UserDataLoader { get; private set; }

        public SaveDataWriter SaveDataWriter { get; private set; }
        public SaveDataLoader SaveDataLoader { get; private set; }

        public void Initialize()
        {
            TableDataLoader = new TableDataLoader();
            TableDataLoader.Initialize();

            SaveDataWriter = new SaveDataWriter();
            SaveDataLoader = new SaveDataLoader();

            UserDataWriter = new UserDataWriter();
            UserDataLoader = new UserDataLoader();
            UserDataWriter.Initialize(UserDataLoader, SaveDataWriter);
            UserDataLoader.Initialize(UserDataWriter, SaveDataWriter, SaveDataLoader);
        }

        public void CallSave()
        {
            //SaveDataWriter.SavePlayerDataToJson();
        }

        public PlayerSaveData LoadSaveData(int useID, int saveNumber = -1)
        {
            if (saveNumber == -1)
            {
                return UserDataLoader.LoadLastSaveData(useID);
            }

            return UserDataLoader.LoadUserAndSaveData(useID, saveNumber);

        }
    }


}
