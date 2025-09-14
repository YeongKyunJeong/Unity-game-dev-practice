using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace RSP2
{
    public class CombatSystem : MonoBehaviour
    {
        public CombatUnit MyUnit { get; private set; }
        private ForceReceiver forceReceiver { get; set; }

        [SerializeField] private float healthChangeDelay = .5f;

        private StatHandlerForCharacter statHandler;

        [SerializeField] protected Faction myFaction;
        public Faction MyFaction { get => myFaction; set { myFaction = value; } }

        [SerializeField] protected bool isDead;

        public bool IsDead { get => isDead; set { isDead = value; } }

        private float timeSinceLastChange = float.MaxValue;


        protected AttackHitBox attackHitBox;
        public event Action<float, float> DamageEvent;
        public event Action<float, float> HealEvent;
        public event Action DieEvent;
        public event Action InvincibilityEndEvent;
        private Coroutine hPRegenCoroutine;
        private Coroutine hPRegenDelayCoroutine;
        private float hpRegenDelayTime = 5f;

        public event Action MPSpendEvent;
        public event Action MPRecoveryEvent;
        private Coroutine mPRegenCoroutine;
        private Coroutine mPRegenDelayCoroutine;
        private float mPRegenDelayTime = 5f;

        public event Action StaminaSpendEvent;
        public event Action StaminaRecoveryEvent;
        private Coroutine staminaRegenCoroutine;
        private Coroutine staminaRegenDelayCoroutine;
        private float staminaRegenDelayTime = 2f;
        private float exhaustionDelayTime = 5f;

        private bool isInitialized;

        private float currentHP;
        public float CurrentHP
        {
            get
            {
                if (!isInitialized) InitStatistics();
                return currentHP;
            }
            protected set
            {
                currentHP = value;
            }
        }
        public float MaxHP => statHandler.CurrentStatistics.MaxHP;

        private float currentMP;
        public float CurrentMP
        {
            get
            {
                if (!isInitialized) InitStatistics();
                return currentMP;
            }
            protected set
            {
                currentMP = value;
            }
        }
        public float MaxMP => statHandler.CurrentStatistics.MaxMP;

        private float currentStamina;
        public float CurrentStamina
        {
            get
            {
                if (!isInitialized) InitStatistics();
                return currentStamina;
            }
            protected set
            {
                currentStamina = value;
            }
        }
        public float MaxStamina => statHandler.CurrentStatistics.MaxStamina;

        protected virtual void Awake()
        {
            MyUnit = GetComponent<CombatUnit>();
            forceReceiver = GetComponent<ForceReceiver>();
            if(forceReceiver == null)
            {
                forceReceiver = GetComponent<ForceReceiver>();
            }
            statHandler = GetComponent<StatHandlerForCharacter>();
            isInitialized = false;
            isDead = false;
        }

        public virtual void InitStatistics()
        {
            isInitialized = true;
            CurrentHP = MaxHP;
            CurrentMP = MaxMP;
            CurrentStamina = MaxStamina;
            isDead = false;
        }

        private void Update()
        {
            if (timeSinceLastChange < healthChangeDelay)
            {
                timeSinceLastChange += Time.deltaTime;
                if (timeSinceLastChange >= healthChangeDelay)
                {
                    InvincibilityEndEvent?.Invoke();
                }
            }
        }

        public bool TakeDamage(float value, DamageType damageType, bool applyDef = true)
        {
            if (value == 0 || timeSinceLastChange < healthChangeDelay)
            {
                return false;
            }

            timeSinceLastChange = 0;

            float reducedDamage;

            if (applyDef)
            {
                // HP & Def = 10 => 2HP & Def = 0 
                reducedDamage = (10 / (10 + statHandler.CurrentStatistics.Defense)) * value;
                reducedDamage = Mathf.Round(reducedDamage * 10f) / 10f;
            }
            else
            {
                reducedDamage = value;
            }

            ChangeHealth(-reducedDamage);

            SFXManager.PlayDamageSoundClip(damageType, transform.position);


            return true;
        }

        public virtual void TakeForce(Vector3 force)
        {
            forceReceiver?.AddForce(force);
        }

        public bool ChangeHealth(float value)
        {
            if (!isInitialized) InitStatistics();

            CurrentHP += value;
            CurrentHP = Mathf.Round(CurrentHP * 100) / 100;
            CurrentHP = CurrentHP > MaxHP ? MaxHP : CurrentHP;
            CurrentHP = CurrentHP < 0 ? 0 : CurrentHP;

            if (value > 0)
            {
                HealEvent?.Invoke(currentHP, MaxHP);
            }
            else
            {
                DamageEvent?.Invoke(currentHP, MaxHP);

                if (hPRegenCoroutine != null)
                {
                    StopCoroutine(hPRegenCoroutine);
                    hPRegenCoroutine = null;
                }

                if (hPRegenDelayCoroutine != null)
                {
                    StopCoroutine(hPRegenDelayCoroutine);
                    hPRegenDelayCoroutine = null;
                }

                hPRegenDelayCoroutine = StartCoroutine(StartHPRegenAfterDelay());
            }

            if (CurrentHP <= 0f)
            {
                Die();
            }

            return true;
        }

        private void Die()
        {
            isDead = true;
            DieEvent?.Invoke();
            StopAllRegenCoroutine();
            this.enabled = false;
        }

        public void ChangeMana(float value)
        {
            if (!isInitialized) InitStatistics();

            CurrentMP += value;
            CurrentMP = Mathf.Round(CurrentMP * 100) / 100;
            CurrentMP = CurrentMP > MaxMP ? MaxMP : CurrentMP;
            CurrentMP = CurrentMP < 0 ? 0 : CurrentMP;
            //Debug.Log(CurrentMP);

            if (value > 0)
            {
                MPRecoveryEvent?.Invoke();
            }
            else
            {
                MPSpendEvent?.Invoke();

                if (mPRegenCoroutine != null)
                {
                    StopCoroutine(mPRegenCoroutine);
                    mPRegenCoroutine = null;
                }

                if (mPRegenDelayCoroutine != null)
                {
                    StopCoroutine(mPRegenDelayCoroutine);
                    mPRegenDelayCoroutine = null;
                }

                mPRegenDelayCoroutine = StartCoroutine(StartMPRegenAfterDelay());
            }
        }

        public void ChangeStamina(float value)
        {
            if (!isInitialized) InitStatistics();

            CurrentStamina += value;
            CurrentStamina = Mathf.Round(CurrentStamina * 100) / 100;
            CurrentStamina = CurrentStamina > MaxStamina ? MaxStamina : CurrentStamina;
            CurrentStamina = CurrentStamina < 0 ? 0 : CurrentStamina;
            //Debug.Log(CurrentStamina);

            if (value > 0)
            {
                StaminaRecoveryEvent?.Invoke();
            }
            else
            {
                StaminaSpendEvent?.Invoke();

                if (staminaRegenCoroutine != null)
                {
                    StopCoroutine(staminaRegenCoroutine);
                    staminaRegenCoroutine = null;
                }

                if (staminaRegenDelayCoroutine != null)
                {
                    StopCoroutine(staminaRegenDelayCoroutine);
                    staminaRegenDelayCoroutine = null;
                }

                staminaRegenDelayCoroutine = StartCoroutine(StartStaminaRegenAfterDelay());
            }
        }

        private void StopAllRegenCoroutine()
        {
            if (hPRegenCoroutine != null)
            {
                StopCoroutine(hPRegenCoroutine);
                hPRegenCoroutine = null;
            }
            if (hPRegenDelayCoroutine != null)
            {
                StopCoroutine(hPRegenDelayCoroutine);
                hPRegenDelayCoroutine = null;
            }
            if (mPRegenCoroutine != null)
            {
                StopCoroutine(mPRegenCoroutine);
                mPRegenCoroutine = null;
            }
            if (mPRegenDelayCoroutine != null)
            {
                StopCoroutine(mPRegenDelayCoroutine);
                mPRegenDelayCoroutine = null;
            }
            if (staminaRegenCoroutine != null)
            {
                StopCoroutine(staminaRegenCoroutine);
                staminaRegenCoroutine = null;
            }

            if (staminaRegenDelayCoroutine != null)
            {
                StopCoroutine(staminaRegenDelayCoroutine);
                staminaRegenDelayCoroutine = null;
            }
        }

        #region HP Regen Coroutines
        private IEnumerator StartHPRegenAfterDelay()
        {
            yield return new WaitForSeconds(hpRegenDelayTime);

            if (CurrentHP < MaxHP)
            {
                hPRegenCoroutine = StartCoroutine(HPRegen());
            }

            hPRegenDelayCoroutine = null;

            yield return null;
        }

        private IEnumerator HPRegen()
        {
            while (CurrentHP < MaxHP)
            {
                CurrentHP += statHandler.CurrentStatistics.HPRegen / 4;
                CurrentHP = CurrentHP > MaxHP ? MaxHP : CurrentHP;
                HealEvent?.Invoke(currentHP, MaxHP);

                yield return new WaitForSeconds(1 / 4f);
            }

            hPRegenCoroutine = null;

            yield return null;
        }
        #endregion

        #region Mp Regen Coroutines
        private IEnumerator StartMPRegenAfterDelay()
        {
            yield return new WaitForSeconds(mPRegenDelayTime);

            if (CurrentMP < MaxMP)
            {
                mPRegenCoroutine = StartCoroutine(MPRegen());
            }

            mPRegenDelayCoroutine = null;

            yield return null;
        }

        private IEnumerator MPRegen()
        {
            while (CurrentMP < MaxMP)
            {
                CurrentMP += statHandler.CurrentStatistics.MPRegen / 4;
                CurrentMP = CurrentMP > MaxMP ? MaxMP : CurrentMP;
                MPRecoveryEvent?.Invoke();

                //Debug.Log($"{name} MP 회복 중 : {CurrentMP}/{MaxMP}");

                yield return new WaitForSeconds(1 / 4f);
            }

            mPRegenCoroutine = null;

            yield return null;
        }
        #endregion


        #region Stamina Regen Coroutines
        private IEnumerator StartStaminaRegenAfterDelay()
        {
            yield return new WaitForSeconds(staminaRegenDelayTime);

            if (CurrentStamina < MaxStamina)
            {
                staminaRegenCoroutine = StartCoroutine(StaminaRegen());
            }

            staminaRegenDelayCoroutine = null;

            yield return null;
        }

        private IEnumerator StaminaRegen()
        {
            while (CurrentStamina < MaxStamina)
            {
                CurrentStamina += statHandler.CurrentStatistics.StaminaRegen / 8;
                CurrentStamina = CurrentStamina > MaxStamina ? MaxStamina : CurrentStamina;
                StaminaRecoveryEvent?.Invoke();

                //Debug.Log($"{name} 스태미나 회복 중 : {CurrentStamina}/{MaxStamina}");

                yield return new WaitForSeconds(1 / 8f);
            }

            staminaRegenCoroutine = null;

            yield return null;
        }
        #endregion
    }
}
