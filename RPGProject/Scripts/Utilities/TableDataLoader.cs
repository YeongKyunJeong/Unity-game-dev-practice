using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class TableDataLoader
    {
        public BaseStatLoaderForPlayer BaseStatLoaderForPlayer { get; private set; }
        public LevelStatLoaderForPlayer LevelStatLoaderForPlayer { get; private set; }
        public ExpDataLoader ExpDataLoader { get; private set; }

        public StatLoaderForEnemy StatLoaderForEnemy { get; private set; }
        public StatLoaderForNPC StatLoaderForNPC { get; private set; }

        public DialogueDataLoader DialogueDataLoader { get; private set; }
        public DialogueScriptsLoader DialogueScriptsLoader { get; private set; }


        public void Initialize()
        {
            BaseStatLoaderForPlayer = new BaseStatLoaderForPlayer();
            LevelStatLoaderForPlayer = new LevelStatLoaderForPlayer();
            ExpDataLoader = new ExpDataLoader();

            StatLoaderForEnemy = new StatLoaderForEnemy();
            StatLoaderForNPC = new StatLoaderForNPC();

            DialogueDataLoader = new DialogueDataLoader();
            DialogueScriptsLoader = new DialogueScriptsLoader();
        }
    }


}
