using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDP
{
    public class RoundsSurvived : MonoBehaviour
    {
        public Text roundsText;
        public static WaitForSecondsRealtime roundTextChangingTime;
        private void OnEnable()
        {
            //roundsText.text = PlayerStats.Rounds.ToString();
            StartCoroutine(AnimateText());
        }
        private IEnumerator AnimateText()
        {
            roundsText.text = "0";
            int round = 0;

            roundTextChangingTime = new WaitForSecondsRealtime(0.05f);

            yield return new WaitForSecondsRealtime(0.7f);

            while (round < PlayerStats.Rounds)
            {
                roundsText.text = $"{++round}";
                yield return roundTextChangingTime;
            }
        }

    }

}
