using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class NPC : Enemy
    {
        public const int NPC_KEY_CONST = 10000;
        
        public int NPCKey;

        [field: SerializeField] public NPCInteraction Interactions { get; private set; }
        [field: SerializeField] public InteractionHitBoxForNPC InteractionHitBox { get; private set; }
        [field: SerializeField] public NPCCamera NPCCamera { get; private set; }

        [field: SerializeField] public bool HasDialogue { get; set; }
        [field: SerializeField] public int DialogueKey { get; set; }

        protected override void Start()
        {
            if (gameManager == null)
            {
                gameManager = InGameManager.Instance;
            }

            if (InteractionHitBox == null)
            {
                Debug.LogError("Interaction Hit Box Not Assigned");
                //throw new NotImplementedException("Interaction Hit Box Not Assigned");
            }

            if (NPCCamera != null)
            {
                CameraManager.Instance.AddCamera(NPCCamera.VirtualCamera);
            }


            StatHandler.Initialize(this, DataManager.Instance.TableDataLoader.StatLoaderForNPC.GetByKey(NPCKey));


            CombatSystem.DamageEvent += OnHit;
            CombatSystem.DieEvent += OnDie;
        }

        protected override void OnDie()
        {
            EventBus.TriggerEnemyHunted(NPCKey + NPC.NPC_KEY_CONST);

            //gameManager.EnemyDie(this);
            ActionStateMachine.OnDie();
            this.enabled = false;
            Controller.enabled = false;
            AttackHitBox.Deactivate();
        }

    }
}

