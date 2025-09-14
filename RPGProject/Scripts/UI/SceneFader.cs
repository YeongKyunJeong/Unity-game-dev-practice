using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RSP2
{
    public class SceneFader : MonoBehaviour
    {
        [field: SerializeField] private CanvasGroup canvasGroup;
        [field: SerializeField] private Image screenImg;
        [field: SerializeField] private AnimationCurve animationCurve;

        [Header("Slow")]
        [field: SerializeField] private float slowFadingStartTime/* = 0.5f*/;
        [field: SerializeField] private float slowFadingTime/* = 1.5f*/;

        [Header("Fast")]
        [field: SerializeField] private float fastFadingStartTime/* = 0.1f*/;
        [field: SerializeField] private float fastFadingTime/* = 0.5f*/;

        private bool isScreenPersisted;
        private Coroutine fadingCoroutine;
        private float t;
        private float a;
        private GameManager gameManager;

        public void Initialize(GameManager _gameManager)
        {
            this.gameManager = _gameManager;
            isScreenPersisted = false;
            EnableInputBlock(false);
            SetScreen();
        }

        public void EnableInputBlock(bool isEnable = true)
        {
            if (canvasGroup == null) return;

            canvasGroup.blocksRaycasts = isEnable;
        }

        public void SetScreen(bool isScreen = false, bool stopFading = false)
        {
            if (stopFading && fadingCoroutine != null) StopCoroutine(fadingCoroutine);

            if (isScreen)
            {
                isScreenPersisted = true;
                screenImg.color = new Color(0f, 0f, 0f, 1);
                return;
            }

            isScreenPersisted = false;
            screenImg.color = new Color(0f, 0f, 0f, 0f);
        }

        public void CallFade(FadingType fadingType, bool inputBlock, Action OnComplete)
        {
            EnableInputBlock(inputBlock);

            if (isScreenPersisted) return;

            if (fadingCoroutine != null)
            {
                StopCoroutine(fadingCoroutine);
                fadingCoroutine = null;
            }

            switch (fadingType)
            {
                case FadingType.SlowFadeIn:
                    {
                        fadingCoroutine = StartCoroutine(FadeIn(false, OnComplete));
                        break;
                    }
                case FadingType.FastFadeIn:
                    {
                        fadingCoroutine = StartCoroutine(FadeIn(true, OnComplete));
                        break;
                    }
                case FadingType.SlowFadeOut:
                    {
                        fadingCoroutine = StartCoroutine(FadeOut(false, OnComplete));
                        break;
                    }
                case FadingType.FastFadeOut:
                    {
                        fadingCoroutine = StartCoroutine(FadeIn(true, OnComplete));
                        break;
                    }
                default:
                    {
                        SetScreen();
                        break;
                    }
            }
        }

        private IEnumerator FadeIn(bool isFast = false, Action onComplete = null)
        {
            SetScreen(true);
            t = 1;

            if (isFast)
            {
                yield return new WaitForSeconds(fastFadingStartTime);

                while (t > 0f)
                {
                    t -= Time.deltaTime / fastFadingTime;
                    a = animationCurve.Evaluate(t);
                    screenImg.color = new Color(0f, 0f, 0f, t);

                    yield return 0;
                }
            }
            else
            {
                yield return new WaitForSeconds(slowFadingStartTime);

                while (t > 0f)
                {
                    t -= Time.deltaTime / slowFadingTime;
                    a = animationCurve.Evaluate(t);
                    screenImg.color = new Color(0f, 0f, 0f, t);

                    yield return 0;
                }
            }

            onComplete?.Invoke();
        }

        private IEnumerator FadeOut(bool isFast = false, Action onComplete = null)
        {
            SetScreen(false);
            t = 0;

            if (isFast)
            {
                yield return new WaitForSecondsRealtime(fastFadingStartTime);

                while (t < 1f)
                {
                    t += Time.deltaTime / fastFadingTime;
                    a = animationCurve.Evaluate(t);
                    screenImg.color = new Color(0f, 0f, 0f, t);

                    yield return 0;
                }
            }
            else
            {
                yield return new WaitForSecondsRealtime(slowFadingStartTime);

                while (t < 1f)
                {
                    t += Time.deltaTime / slowFadingTime;
                    a = animationCurve.Evaluate(t);
                    screenImg.color = new Color(0f, 0f, 0f, t);

                    yield return 0;
                }
            }

            onComplete?.Invoke();
        }
    }
}
