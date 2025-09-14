using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RSP2
{
    public class Enemy : CombatUnit
    {

        protected InGameManager gameManager;
        [field: SerializeField] public ForceReceiverForEnemy ForceReceiver { get; private set; }
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public CharacterController Controller { get; protected set; }
        [field: SerializeField] public MoverForEnemy Mover { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public StatHandlerForEnemy StatHandler { get; private set; }
        [field: SerializeField] public CombatSystemForEnemy CombatSystem { get; private set; }
        [field: SerializeField] public AttackHitBoxForEnemy AttackHitBox { get; private set; }

        public ActionStateMachineForEnemy ActionStateMachine { get; protected set; }

        public string Name;
        public int EnemyKey;

        [field: SerializeField] public AttackData[] AttackDataArray { get; protected set; }
        public RuntimeDataForEnemy RuntimeData { get; private set; }
        [field: SerializeField] protected float searchingDistance { get; set; }
        [field: SerializeField] public LayerMask SearchingLayerMask { get; protected set; }
        [field: SerializeField] public float FieldOfView { get; protected set; }


        [field: SerializeField][field: Range(0f, 25f)] public float RotationSpeedModifier { get; protected set; } = 10;



        protected virtual void Awake()
        {
            RuntimeData = new RuntimeDataForEnemy();
            ActionStateMachine = new ActionStateMachineForEnemy(this);


            if (ForceReceiver == null)
            {
                throw new NotImplementedException("Enemy ForceReceiver Not Assigned");
            }

            if (NavMeshAgent == null)
            {
                throw new NotImplementedException("Enemy Navimesh Agent Not Assigned");
            }

            if (Controller == null)
            {
                throw new NotImplementedException("Enemy Character Controller Not Assigned");
            }

            if (Mover == null)
            {
                throw new NotImplementedException("Enemy Mover Not Assigned");
            }
            Mover.Initialize(this);

            if (Animator == null)
            {
                throw new NotImplementedException("Enemy Animator Not Assigned");
            }

            if (StatHandler == null)
            {
                throw new NotImplementedException("Enemy StatisticsHandler Not Assigned");
            }

            if (CombatSystem == null)
            {
                throw new NotImplementedException("Player CombatSystem Not Assigned");
            }

            if (AttackHitBox == null)
            {
                throw new NotImplementedException("Player AttackHitBox Not Assigned");
            }
            AttackHitBox.Initialize(1 << LayerMask.NameToLayer("Combat Unit"));
            RuntimeData.AttackPositionModifier = new Vector3(0, AttackHitBox.HitBoxCollider.bounds.center.y, 0);
        }

        protected virtual void Start()
        {
            if (gameManager == null)
            {
                gameManager = InGameManager.Instance;
            }

            //StatisticsHandler.InitializeByDefault();
            StatHandler.Initialize(this, DataManager.Instance.TableDataLoader.StatLoaderForEnemy.GetByKey(EnemyKey));

            CombatSystem.DamageEvent += OnHit;
            CombatSystem.DieEvent += OnDie;

        }

        private void Update()
        {
            ActionStateMachine.CallUpdate();
            ForceReceiver.CallUpdate();
            Mover.CallUpdate();
        }

        protected void OnHit(float leftHP, float maxHP)
        {
            ActionStateMachine.OnHit();
        }

        protected override void OnDie()
        {
            EventBus.TriggerEnemyHunted(EnemyKey);

            //gameManager.EnemyDie(this);
            ActionStateMachine.OnDie();
            this.enabled = false;
            Controller.enabled = false;
            AttackHitBox.Deactivate();
        }
    }
}
