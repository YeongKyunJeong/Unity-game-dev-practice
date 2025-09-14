using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class SFXManager : MonoSingleton<SFXManager>
    {
        private InGameManager gameManager;

        [field: SerializeField][Range(0f, 1f)] private float soundEffectVolume;
        [field: SerializeField][Range(0f, 1f)] private float soundEffectPitchVariance;
        [field: SerializeField][Range(0f, 1f)] private float musicVolume;

        private ObjectPool objectPool;

        private AudioSource musicAudioSource;
        public AudioClip musicClip;


        [field: SerializeField] public SFXScriptableObject SFXSOData { get; private set; }

        //private void Awake()
        //{
        //    musicAudioSource = Camera.main.GetComponent<AudioSource>();
        //    musicAudioSource.volume = musicVolume;
        //    musicAudioSource.loop = true;

        //    objectPool = GetComponent<ObjectPool>();
        //}

        public void Initialize(InGameManager _gameManager)
        {
            gameManager = _gameManager;
            musicAudioSource = Camera.main.GetComponent<AudioSource>();
            musicAudioSource.volume = musicVolume;
            musicAudioSource.loop = true;

            objectPool = GetComponent<ObjectPool>();
        }

        private void Start()
        {
            if (gameManager == null) return;

            //if (!gameManager.IsInitialized) return;

            ChangeBackGroundMusic(musicClip);
        }

        public static void ChangeBackGroundMusic(AudioClip musicClip)
        {
            Instance.musicAudioSource.Stop();
            Instance.musicAudioSource.clip = musicClip;
            Instance.musicAudioSource.Play();
        }

        public static void PlayClip(AudioClip clip, Vector3 sourcePosition, float volumeMultiplier = 1.0f, float speedMultipliyer = 1.0f)
        {
            if (clip == null) return;
            SoundSource soundSource = GetAndSetSoundSource(sourcePosition);
            soundSource.Play(clip, Instance.soundEffectVolume * volumeMultiplier, speedMultipliyer, Instance.soundEffectPitchVariance);
        }

        public static void PlayDamageSoundClip(DamageType damageType, Vector3 sourcePosition, float volumeMultiplier = 1.0f, float speedMultipliyer = 1.0f)
        {
            SoundSource soundSource = GetAndSetSoundSource(sourcePosition);

            switch (damageType)
            {
                case DamageType.ByMainWeapon:
                    break;
                case DamageType.Slashing:
                    {
                        soundSource.Play(Instance.SFXSOData.SFXDataLibrary.SlashingHitSounds[0], Instance.soundEffectVolume * volumeMultiplier, speedMultipliyer, Instance.soundEffectPitchVariance);

                        break;
                    }
                case DamageType.Blunging:
                    {
                        soundSource.Play(Instance.SFXSOData.SFXDataLibrary.BlungingHitSounds[0], Instance.soundEffectVolume * volumeMultiplier, speedMultipliyer, Instance.soundEffectPitchVariance);
                        break;
                    }
            }
        }

        private static SoundSource GetAndSetSoundSource(Vector3 sourcePosition)
        {
            GameObject go = Instance.objectPool.SpawnFromPool("SoundSource", false);
            go.SetActive(true);
            go.transform.position = sourcePosition;
            SoundSource soundSource = go.GetComponent<SoundSource>();
            return soundSource;
        }

    }
}
