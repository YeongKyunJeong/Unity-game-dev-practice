using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace RSP2
{
    public class DialogueDisplay : MonoBehaviour
    {
        [field: SerializeField] private TextMeshProUGUI talkerNameTMP { get; set; }
        [field: SerializeField] private TextMeshProUGUI speechContentTMP { get; set; }
        //[field: SerializeField] private TMP_Text speechContentTMP { get; set; }
        private string targetScript { get; set; }
        private Coroutine speechCoroutine { get; set; }
        private WaitForSeconds waitingSecond;
        private StringBuilder stringBuilder;

        public bool isPlaying { get; set; }


        public void Initialize()
        {
            stringBuilder = new StringBuilder();
            Deactivate();
            isPlaying = false;
        }

        public void SetName(string name)
        {
            talkerNameTMP.text = name;
        }

        public void SetScript(DialogueScript script, float letterPerSec = 30)
        {
            speechContentTMP.text = string.Empty;
            targetScript = script.ScriptContent;

            speechCoroutine = StartCoroutine(TalkTyping(letterPerSec));
        }

        public void EndScript()
        {
            StopCoroutine(speechCoroutine);
            speechContentTMP.text = targetScript;
            isPlaying = false;
        }

        IEnumerator TalkTyping(float letterPerSec)
        {
            isPlaying = true;
            stringBuilder.Clear();
            waitingSecond = new WaitForSeconds(1 / letterPerSec);
            for (int i = 0; i < targetScript.Length; i++)
            {
                stringBuilder.Append(targetScript[i]);
                speechContentTMP.text = stringBuilder.ToString();
                if (targetScript[i] == ' ')
                {
                    yield return null;
                }
                yield return waitingSecond;
            }

            isPlaying = false;
            yield return null;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }


    }
}
