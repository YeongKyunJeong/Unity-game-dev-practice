using CsvHelper;
using CsvHelper.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.Timeline.TimelinePlaybackControls;

namespace RSP2
{
    public class DialogueScriptSet
    {
        public DialogueScript_Legacy[] DialogueScripts;
        public int Length;
    }

    public struct DialogueScript_Legacy
    {
        //1.State(int) : 출력 상황
        public int State;
        //2.Random(int) : 랜덤 출력 여부(0: 순차 출력, 1: 랜덤 출력)
        public bool IsRandom;
        //3.Player(int) : Player의 대사 여부(0: NPC 대사, 1: Player 대사)
        public bool IsPlayerScript;
        //4.Redirection(int) : 해당 대화 종료 시 해당 state로 변경
        public int Redirection;
        //5.Name(string) : 출력 시 이름
        public string Name;
        //6.Content(string) : 출력 대사 내용
        public string Content;

        //7.Button action 1, 2, 3(int) : 버튼 기능(0: redirection, 1: 거래)
        public int ButtonAction1;
        //8.Button content1 ,2 ,3(string) : 버튼 위 출력 내용
        public string ButtonContent1;
        //9.Button redirection 1, 2, 3(int) : 해당 버튼 클릭 시 해당 state로 변경
        public int ButtonRedirection1;

        public int ButtonAction2;
        public string ButtonContent2;
        public int ButtonRedirection2;

        public int ButtonAction3;
        public string ButtonContent3;
        public int ButtonRedirection3;



    }

    public class DialogueDataLoader_Legacy
    {
        private Dictionary<int, DialogueScriptSet> nPCDialogueDictionary;

        public DialogueDataLoader_Legacy()
        {
            nPCDialogueDictionary = new Dictionary<int, DialogueScriptSet>();

        }

        public void CallDialogueDataLoading(DialogueType dialogueType, int key, string name, string path = "CSV/Dialogue")
        {
            string loadedCSVDataString = string.Empty;

            switch (dialogueType)
            {
                case DialogueType.NPC:
                    {
                        loadedCSVDataString = Resources.Load<TextAsset>(string.Concat(path, "/NPC/", name)).text;
                        break;
                    }
            }

            using (var reader = new StringReader(loadedCSVDataString))
            {
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    IgnoreBlankLines = true,
                    TrimOptions = TrimOptions.Trim,
                    BadDataFound = null
                }))
                {
                    var rawCSVText = csv.GetRecords<Dictionary<string, string>>();
                    DialogueScriptSet result = new DialogueScriptSet();
                    result.DialogueScripts = new DialogueScript_Legacy[rawCSVText.ToList().Count];
                    int ind = 0;

                    foreach (var row in rawCSVText)
                    {
                        DialogueScript_Legacy oneScript = new DialogueScript_Legacy();

                        #region Base Dialogue
                        oneScript.State = int.Parse(row["State"]);
                        oneScript.IsRandom = int.Parse(row["Random"]) == 0 ? false : true;
                        oneScript.IsPlayerScript = int.Parse(row["Player"]) == 0 ? false : true;
                        oneScript.Redirection = string.IsNullOrWhiteSpace(row["Redirection"])
                            ? -1
                            : int.TryParse(row["Redirection"], out int redirection)
                                ? redirection : -1;
                        oneScript.Name = string.IsNullOrWhiteSpace(row["Name"])
                            ? "?"
                            : row["Name"];
                        oneScript.Content = row["Content"];
                        #endregion

                        #region Button 1
                        oneScript.ButtonAction1 = string.IsNullOrWhiteSpace(row["Button action 1"])
                            ? -1
                            : int.TryParse(row["Button action 1"], out int action1)
                                ? action1 : -1;
                        oneScript.ButtonContent1 = row["Button content 1"];
                        oneScript.ButtonRedirection1 = string.IsNullOrWhiteSpace(row["Button redirection 1"])
                            ? -1
                            : int.TryParse(row["Button redirection 1"], out int buttonRedirection1)
                                ? buttonRedirection1 : -1;
                        #endregion

                        #region Button 2
                        oneScript.ButtonAction2 = string.IsNullOrWhiteSpace(row["Button action 2"])
                            ? -1
                            : int.TryParse(row["Button action 2"], out int action2)
                                ? action2 : -1;
                        oneScript.ButtonContent2 = row["Button content 2"];
                        oneScript.ButtonRedirection2 = string.IsNullOrWhiteSpace(row["Button redirection 2"])
                            ? -1
                            : int.TryParse(row["Button redirection 2"], out int buttonRedirection2)
                                ? buttonRedirection2 : -1;
                        #endregion

                        #region Button 3
                        oneScript.ButtonAction3 = string.IsNullOrWhiteSpace(row["Button action 3"])
                            ? -1
                            : int.TryParse(row["Button action 3"], out int action3)
                                ? action3 : -1;
                        oneScript.ButtonContent3 = row["Button content 3"];
                        oneScript.ButtonRedirection3 = string.IsNullOrWhiteSpace(row["Button redirection 3"])
                            ? -1
                            : int.TryParse(row["Button redirection 3"], out int buttonRedirection3)
                                ? buttonRedirection3 : -1;
                        #endregion

                        result.DialogueScripts[ind] = oneScript;
                        ind++;
                    }

                    switch (dialogueType)
                    {
                        case DialogueType.NPC:
                            {
                                nPCDialogueDictionary.Add(key, result);

                                break;
                            }
                        case DialogueType.Narrator:
                            {
                                break;
                            }
                        default: break;
                    }

                }
            }

        }

        public DialogueScriptSet GetDialogueData(DialogueType dialogueType, int key)
        {
            switch (dialogueType) 
            {
                case DialogueType.NPC: 
                    {
                        return nPCDialogueDictionary[key];
                    }
            default : return null;
            }

            return null;
        }


    }
}
