using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSP2
{
    public class SoundSource : MonoBehaviour
    {
        private AudioSource audioSource;

        public void Play(AudioClip clip, float sFXVolume, float sFXPitch, float sFXPichVariance)
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

            CancelInvoke();

            audioSource.clip = clip;
            audioSource.volume = sFXVolume;

            audioSource.Play();
            audioSource.pitch = 1f + Random.Range(-sFXPichVariance, sFXPichVariance);
            audioSource.pitch *= sFXPitch;

            Invoke("Disable", clip.length + 2);
        }

        public void Disable()
        {
            audioSource?.Stop();
            gameObject?.SetActive(false);
        }
    }
}
