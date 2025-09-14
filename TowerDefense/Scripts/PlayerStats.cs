using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{
    public class PlayerStats : MonoBehaviour
    {
        public static int Money;
        [SerializeField] private int startMoney = 400;

        public static int Life;
        [SerializeField] private int startLives = 20;

        public static int Rounds;
        [SerializeField] private int startRounds = 0;

        public void Initialize()
        {
            Money = startMoney;
            Life = startLives;
            Rounds = startRounds;
        }
    }
}
