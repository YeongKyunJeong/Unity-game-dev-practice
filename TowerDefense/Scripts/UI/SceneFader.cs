using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TDP
{
    public enum SceneType
    {
        MainMenu,
        Stage,
        LevelSelecter,
        Retry
    }

    public class SceneFader : MonoBehaviour
    {
        [SerializeField] private Image screenImg;
        [SerializeField] private float fadingStartTime = 0.5f;
        [SerializeField] private float fadingTime = 1f;
        private float t;
        private float a;
        public AnimationCurve animationCurve;
        private GameManager gameManager;

        private string targetSceneName;

        private void Start()
        {
            if (gameManager == null)
            {
                gameManager = GameManager.Instance;
            }
            StartCoroutine(FadeIn());
        }

        public void FadeTo(SceneType targetSceneType, int targetLevel = 0)
        {
            StartCoroutine(FadeOut(targetSceneType, targetLevel));
        }

        IEnumerator FadeIn()
        {
            screenImg.color = new Color(0f, 0f, 0f, 1);
            yield return new WaitForSeconds(fadingStartTime);
            t = 1;
            while (t > 0f)
            {
                t -= Time.deltaTime / fadingTime;
                a = animationCurve.Evaluate(t);
                screenImg.color = new Color(0f, 0f, 0f, t);

                yield return 0;
            }
        }

        IEnumerator FadeOut(SceneType targetSceneType, int targetLevel)
        {
            yield return new WaitForSecondsRealtime(fadingStartTime);
            t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime / fadingTime;
                a = animationCurve.Evaluate(t);
                screenImg.color = new Color(0f, 0f, 0f, t);

                yield return 0;
            }

            switch (targetSceneType)
            {
                case SceneType.MainMenu:
                    {
                        targetSceneName = GameManager.MAIN_MENU_SCENE_NAME_STR;
                        break;
                    }
                case SceneType.Stage:
                    {
                        targetSceneName = $"{GameManager.STAGE_SCENE_NAME_STR}_Level{targetLevel:00}";
                        break;
                    }
                case SceneType.LevelSelecter:
                    {
                        targetSceneName = GameManager.LEVEL_SELECTER_SCENE_STR;
                        break;
                    }
                case SceneType.Retry:
                    {
                        targetSceneName = SceneManager.GetActiveScene().name;
                        break;

                    }
                default:
                    {

                        throw new System.Exception("Loaded Not Existent Scene");

                    }
            }
            SceneManager.LoadScene(targetSceneName);
        }

    }
}
