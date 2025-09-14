using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace RSP2
{
    public class Player : CombatUnit
    {
        private InGameManager gameManager;
        public string Name { get; set; }

        [field: SerializeField] public PlayerInputReader InputReader { get; private set; }
        [field: SerializeField] public MoverForPlayer Mover { get; private set; }
        [field: SerializeField] public ForceReceiverForPlayer ForceReceiver { get; private set; }

        [field: SerializeField] public CharacterController Controller { get; private set; }
        [field: SerializeField] public PlayerScriptableObject SOData { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public StatHandlerForPlayer StatHandler { get; private set; }
        [field: SerializeField] public CombatSystemForPlayer CombatSystem { get; private set; }
        [field: SerializeField] public AttackHitBox AttackHitBox { get; private set; }
        [field: SerializeField] public InteractionHitBoxForPlayer InteractionHitBox { get; private set; }
        [field: SerializeField] public Inventory Inventory { get; private set; }


        [field: SerializeField] public Transform MainCameraTransform { get; private set; }

        [field: SerializeField] public Transform WeaponHolder { get; private set; }// TODO:: Make WeaponHolder class and use it to show weapon
        [field: SerializeField] public Weapon CurrentWeapon { get; private set; }

        public Collider AttackHitBoxCollider { get; private set; }
        public ActionStateMachineForPlayer ActionStateMachine { get; private set; }

        public RuntimeDataForPlayer RuntimeData { get; private set; }


        public event Action<ItemInstance, StatForPlayer> EquipmentChangeEvent;
        public event Action<EquipmentType, StatForPlayer> UnequipmentEvent;

        private void Awake()
        {
            RuntimeData = new RuntimeDataForPlayer();
            ActionStateMachine = new ActionStateMachineForPlayer(this);

            if (SOData == null)
            {
                throw new NotImplementedException("Player Scriptable Object Not Assigned");
            }

            if (InputReader == null)
            {
                throw new NotImplementedException("Player Input Reader Not Assigned");
            }
            InputReader.Initialize(this);

            if (Mover == null)
            {
                throw new NotImplementedException("Player Mover Not Assigned");
            }
            Mover.Initialize(this);

            if (ForceReceiver == null)
            {
                throw new NotImplementedException("Player Force Receiver Not Assigned");
            }

            if (Controller == null)
            {
                throw new NotImplementedException("Player Character Controller Not Assigned");
            }

            if (Animator == null)
            {
                throw new NotImplementedException("Player Animator Not Assigned");
            }

            if (StatHandler == null)
            {
                throw new NotImplementedException("Player StatisticsHandler Not Assigned");
            }

            if (CombatSystem == null)
            {
                throw new NotImplementedException("Player CombatSystem Not Assigned");
            }

            if (AttackHitBox == null)
            {
                throw new NotImplementedException("Player AttackHitBox Not Assigned");
            }
            AttackHitBox.Initialize(SOData.AttackDataLibrary.BaseAttackData.TargetLayerMask);
            RuntimeData.AttackPositionModifier = new Vector3(0, AttackHitBox.HitBoxCollider.bounds.center.y, 0);

            if (InteractionHitBox == null)
            {
                throw new NotImplementedException("Player InteractionHitBox Not Assigned");
            }
            InteractionHitBox.Initialize(this);


            if (MainCameraTransform == null)
            {
                MainCameraTransform = Camera.main.transform;
            }

            Name = "You";
        }

        private void Start()
        {
            if (gameManager == null)
            {
                gameManager = InGameManager.Instance;
            }


            if (Inventory == null)
            {
                Inventory = GetComponent<Inventory>();
                if (Inventory == null)
                {
                    throw new NotImplementedException("Player Inventory Not Assigned");
                }
            }
            Inventory.Initialize();


            StatHandler.Initialize(DataManager.Instance.TableDataLoader.BaseStatLoaderForPlayer.GetStat());

            CombatSystem.DamageEvent += OnHit;
            CombatSystem.DieEvent += OnDie;

            gameManager.GetDataFromSave();
        }

        private void Update()
        {
            ActionStateMachine.CallUpdate();
            ForceReceiver.CallUpdate();
            Mover.CallUpdate();
        }

        private void FixedUpdate()
        {
            ActionStateMachine.CallPhysicsUpdate();
            ForceReceiver.CallPhysicsUpdate();
            Mover.CallPhysicsUpdate();
        }

        public void EquipItem(ItemInstance item, EquipmentData equipmentData)
        {
            if (equipmentData == null)
            {
                equipmentData = item.ItemData as EquipmentData;
            }

            switch (equipmentData.EquipmentType)
            {
                case EquipmentType.Weapon:
                    {
                        WeaponData weaponData = item.ItemData as WeaponData;
                        if (weaponData == null) return;

                        if (weaponData.EquipPrefab == null) return;

                        if (CurrentWeapon)
                        {
                            CurrentWeapon.ItemInstance.isEquipped = false;
                            Destroy(CurrentWeapon.gameObject);
                        }

                        GameObject nextWeaponGO = Instantiate(weaponData.EquipPrefab, WeaponHolder);
                        CurrentWeapon = nextWeaponGO.GetComponent<Weapon>();
                        CurrentWeapon?.Initialize(item);
                        item.isEquipped = true;
                        ActionStateMachine.OnEquipWeapon(true);

                        break;
                    }
                    // TO DO :: Add other equipment logic
            }
            EquipmentChangeEvent?.Invoke(item, StatHandler.PlayerCurrentStatistics);
        }

        public void UnEquipItem(EquipmentType equipmentType)
        {
            switch (equipmentType)
            {
                case EquipmentType.Weapon:
                    {
                        if (CurrentWeapon)
                        {
                            CurrentWeapon.ItemInstance.isEquipped = false;
                            Destroy(CurrentWeapon.gameObject);
                            ActionStateMachine.OnEquipWeapon(false);
                        }
                        break;
                    }
                    // TO DO :: Add other equipment logic
            }
            UnequipmentEvent?.Invoke(equipmentType, StatHandler.PlayerCurrentStatistics);
        }

        public bool AddItem(ItemData item, int amount = 1, int upgrade = 0)
        {
            return Inventory.AddItem(item, amount, upgrade);
        }

        private void OnHit(float leftHP, float MaxHP)
        {
            ActionStateMachine.OnHit();
            float cameraShakeIntensity = MaxHP > 0 ? 1 - (leftHP) / MaxHP : 0;
            if (cameraShakeIntensity >= 0.5)
            {
                CameraManager.Instance.CallCameraShakeByHit(cameraShakeIntensity);
            }
        }

        protected override void OnDie()
        {
            ActionStateMachine.OnDie();
            InputReader.enabled = false;
            // TODO :: Add something to do On Dying;
        }

        public void GetPlayerDataForSave(PlayerSaveData saveData)
        {
            saveData.Exp = StatHandler.CurrentExp;
            //saveData.Gold = Inventory. // TO DO:: Add logic after adding gold system
            saveData.HP = StatHandler.combatSystemForPlayer.CurrentHP;
            saveData.MP = StatHandler.combatSystemForPlayer.CurrentMP;
            saveData.Stamina = StatHandler.combatSystemForPlayer.CurrentStamina;

            saveData.Position = transform.position;
            
            //saveData.GameProgress = // TO DO:: Add logic after adding game progress system

            Inventory.GetInventoryDataForSave(saveData);
        }

        public void SetPlayerDataFromSave(PlayerSaveData saveData)
        {
            StatHandler.SetDataFromSave(saveData);

            transform.position = saveData.Position;

            foreach (ItemSaveData itemSaveData in saveData.InventoryData)
            {
                if (itemSaveData.SlotPosition < 0)
                {
                    SetEquipmentFromSave(itemSaveData);
                }
                else
                {
                    SetItemFromSave(itemSaveData);
                }
            }
        }


        private void SetEquipmentFromSave(ItemSaveData itemSaveData)
        {
            switch (itemSaveData.EquipmentType)
            {
                case EquipmentType.Weapon:
                    {
                        //WeaponData weaponData = SOData.EquipmentDataLibrary.GetWeaponDataCopy(itemSaveData.ItemKey);
                        //weaponData.Upgrade = itemSaveData.ItemUpgrade;

                        ItemInstance newEquipment = new ItemInstance(SOData.EquipmentDataLibrary.WeaponData[itemSaveData.ItemKey],
                                                            itemSaveData.Amount, itemSaveData.ItemUpgrade);

                        Inventory.EquipBySaveData(newEquipment);
                        break;
                    }
                case EquipmentType.Armor:
                    {
                        //Inventory.EquipBySaveData(SOData.EquipmentDataLibrary.WeaponData[itemSaveData.ItemKey]);
                        break;
                    }
                case EquipmentType.Accessory:
                    {
                        //Inventory.EquipBySaveData(SOData.EquipmentDataLibrary.WeaponData[itemSaveData.ItemKey]);
                        break;
                    }
                default:
                    break;
            }
        }

        private void SetItemFromSave(ItemSaveData itemSaveData)
        {
            switch (itemSaveData.ItemType)
            {
                case ItemType.Equipable:
                    {
                        switch (itemSaveData.EquipmentType)
                        {
                            case EquipmentType.Weapon:
                                {
                                    //WeaponData weaponData = SOData.EquipmentDataLibrary.WeaponData[itemSaveData.ItemKey];

                                    //WeaponData weaponData = SOData.EquipmentDataLibrary.GetWeaponDataCopy(itemSaveData.ItemKey);
                                    //weaponData.Upgrade = itemSaveData.ItemUpgrade;

                                    ItemInstance newEquipment = new ItemInstance(SOData.EquipmentDataLibrary.WeaponData[itemSaveData.ItemKey],
                                                                           itemSaveData.Amount, itemSaveData.ItemUpgrade);

                                    Inventory.AddItemToSpecificSlot(newEquipment, itemSaveData.SlotPosition);
                                    break;
                                }
                            case EquipmentType.Armor:
                                {
                                    //Inventory.AddItemToSpecificSlot(itemSaveData.SlotPosition,
                                    //    SOData.EquipmentDataLibrary.ArmorData[itemSaveData.ItemKey], 1);
                                    break;
                                }
                            case EquipmentType.Accessory:
                                {
                                    //Inventory.AddItemToSpecificSlot(itemSaveData.SlotPosition,
                                    //    SOData.EquipmentDataLibrary.AccessoryData[itemSaveData.ItemKey], 1);
                                    break;
                                }
                        }
                        break;
                    }
                case ItemType.Consumable:
                    {
                        //ConsumableData consumableData = SOData.ConsumableDataLibrary.GetConsumableDataCopy(itemSaveData.ItemKey);

                        ItemInstance newConsumable = new ItemInstance(SOData.ConsumableDataLibrary.ConsumableData[itemSaveData.ItemKey],
                                                          itemSaveData.Amount, itemSaveData.ItemUpgrade);

                        Inventory.AddItemToSpecificSlot(newConsumable, itemSaveData.SlotPosition);
                        break;
                    }
            }
        }
    }

}