using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class CombatSystemForEnemy : CombatSystem
    {
        public StatHandlerForEnemy StatHandlerForEnemy;

        public int GainExp;

        protected override void Awake()
        {
            base.Awake();

            StatHandlerForEnemy = GetComponent<StatHandlerForEnemy>();
        }

        public override void InitStatistics()
        {
            base.InitStatistics();

            GainExp = StatHandlerForEnemy.EnemyCurrentStatistics.Exp;
        }

    }
}
