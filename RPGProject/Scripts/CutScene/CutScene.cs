using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RSP2
{
    public enum CutsceneAnimation
    {
        Idling,
        Walking,
        StandingUp,
        Laying
    }

    public enum CutsceneActor
    {
        None,
        Player,
        Enemy,
        NPC
    }

    public enum FadingType
    {
        None,
        SlowFadeIn,
        FastFadeIn,
        SlowFadeOut,
        FastFadeOut,
    }

    public class Cutscene : MonoBehaviour
    {
        private readonly int instantIdlingUpHash = Animator.StringToHash("Cutscene.Idling");
        private readonly int instantWalkingUpHash = Animator.StringToHash("Cutscene.Walking");
        private readonly int instantLayingHash = Animator.StringToHash("Cutscene.Laying");
        private readonly int instantStandingUpHash = Animator.StringToHash("Cutscene.StandingUp");

        private readonly int cutsceneEndHash = Animator.StringToHash("CutsceneEnd");

        //[field: SerializeField] private int cutSceneID;
        [field: SerializeField] private CinemachineVirtualCamera cutsceneCamera;
        [field: SerializeField] private List<OneCut> cuts;

        private Coroutine cutsceneCoroutine;
        private Coroutine timerCoroutine;
        private bool dialogueEnd = false;
        private bool cutSkipSignal = false;
        private float elapsed = 0f;
        private float cameraT;
        private float actorT;
        private Quaternion cameraDirFrom;
        private Quaternion cameraDirTo;
        private Quaternion actorDirFrom;
        private Quaternion actorDirTo;
        private Transform actorTransform;
        private CutsceneActor lastActor;
        private int animationHash;

        public void Play()
        {
            if (cuts == null || cuts.Count == 0) return;

            cutsceneCoroutine = StartCoroutine(PlayCutscene());
        }

        private IEnumerator PlayCutscene(float waitTime = 2)
        {
            CameraManager.Instance.AddCamera(cutsceneCamera, true);

            foreach (OneCut cut in cuts)
            {
                if (cut.startWithScreen)
                {
                    GameManager.Instance.SceneFader.SetScreen(true, true);
                }
                else
                {
                    GameManager.Instance.SceneFader.SetScreen(false, true);
                }

                if (cut.fadeInOrOut != FadingType.None)
                {
                    GameManager.Instance.SceneFader.CallFade(cut.fadeInOrOut, false, null);
                }

                ReadyDialogue(cut);
                ReadyCamera(cut);
                ReadyActor(cut);

                elapsed = 0;
                // While Not Camera Moving Ends, Not Actor Moving Ends, Not Dialogue Ends
                while (elapsed < cut.cameraMoveTime || elapsed < cut.actorMoveTime || !dialogueEnd)
                {
                    elapsed += Time.deltaTime;

                    if (cut.moveCamera && elapsed < cut.cameraMoveTime)
                    {
                        cameraT = Mathf.Clamp01(elapsed / cut.cameraMoveTime);
                        cutsceneCamera.transform.rotation = Quaternion.Slerp(cameraDirFrom, cameraDirTo, cameraT);
                        cutsceneCamera.transform.position = Vector3.Slerp(cut.cameraStartPos, cut.cameraEndPos, cameraT);
                    }

                    if (elapsed < cut.actorMoveTime)
                    {
                        actorT = Mathf.Clamp01(elapsed / cut.actorMoveTime);
                        actorTransform.rotation = Quaternion.Slerp(actorDirFrom, actorDirTo, actorT);
                        actorTransform.position = Vector3.Slerp(cut.actorStartPos, cut.actorEndPos, actorT);
                    }

                    elapsed += Time.deltaTime;
                    yield return null;
                }


                cutSkipSignal = false;
                if (cut.needClickToEnd)
                {
                    while (!cutSkipSignal)
                    {
                        yield return null;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(3); // Fixed Waiting Time
                }

                //yield return null;
            }

            EndActorAnimation(lastActor);

            CameraManager.Instance.RemoveCamera(cutsceneCamera);
            CanvasUIManager.Instance.SetPanelUIActive(PanelUIType.Dialogue, false);
            InGameManager.Instance.OnInteractionUIOpen(false);
            CanvasUIManager.Instance.cutsceneDialogueEndEvent -= OnDialogueEnd;
            Destroy(gameObject);
        }

        private void ReadyDialogue(OneCut cut)
        {
            if (!cut.needDialogue) { dialogueEnd = true; return; }

            dialogueEnd = false;
            CanvasUIManager.Instance.cutsceneDialogueEndEvent += OnDialogueEnd;
            CanvasUIManager.Instance.SetPanelUIActive(PanelUIType.Dialogue, true);
            InGameManager.Instance.OnInteractionUIOpen(true);
            CanvasUIManager.Instance.SendDialogueStartCall(cut.dialogueDataKey, true);

        }

        private void ReadyActor(OneCut cut)
        {
            if (cut.actor != CutsceneActor.None)
            {
                actorDirFrom = Quaternion.Euler(cut.actorStartDir);
                actorDirTo = Quaternion.Euler(cut.actorEndDir);

                switch (cut.actorAnimation)
                {
                    case CutsceneAnimation.Idling:
                        {
                            animationHash = instantIdlingUpHash;
                            break;
                        }
                    case CutsceneAnimation.Walking:
                        {
                            animationHash = instantWalkingUpHash;
                            break;
                        }
                    case CutsceneAnimation.Laying:
                        {
                            animationHash = instantLayingHash;
                            break;
                        }
                    case CutsceneAnimation.StandingUp:
                        {
                            animationHash = instantStandingUpHash;
                            break;
                        }
                }
            }

            switch (cut.actor)
            {
                case CutsceneActor.None: { lastActor = CutsceneActor.None; break; }
                case CutsceneActor.Player:
                    {
                        lastActor = CutsceneActor.Player;
                        actorTransform = InGameManager.Instance.Player.transform;
                        actorT = 0;
                        InGameManager.Instance.Player.Animator.Play(animationHash, 0);
                        break;
                    }
                    // TO DO :: Add Logic To Find NPC or Enemy by Spawner
            }
        }

        private void ReadyCamera(OneCut cut)
        {
            cutsceneCamera.transform.position = cut.cameraStartPos;
            cutsceneCamera.transform.eulerAngles = cut.cameraStartDir;

            if (cut.moveCamera)
            {
                cameraDirFrom = Quaternion.Euler(cut.cameraStartDir);
                cameraDirTo = Quaternion.Euler(cut.cameraEndDir);
            }
        }

        private void EndActorAnimation(CutsceneActor actor)
        {
            switch (actor)
            {
                case CutsceneActor.None: break;
                case CutsceneActor.Player:
                    {
                        InGameManager.Instance.Player.Animator.SetTrigger(cutsceneEndHash);
                        break;
                    }
                    // TO DO :: Add Logic To Find NPC or Enemy
            }
        }

        private void OnDialogueEnd()
        {
            if (dialogueEnd) { cutSkipSignal = true; return; }

            dialogueEnd = true;
        }
    }

    [System.Serializable]
    public class OneCut
    {
        [field: SerializeField] public bool needClickToEnd;
        [field: SerializeField] public bool startWithScreen;
        [field: SerializeField] public FadingType fadeInOrOut;

        [Header("Camera")]
        [field: SerializeField] public Vector3 cameraStartPos;
        [field: SerializeField] public Vector3 cameraStartDir;

        [Header("Actor")]
        [field: SerializeField] public CutsceneActor actor;
        [field: SerializeField] public CutsceneAnimation actorAnimation;

        [field: SerializeField] public bool moveActor;
        [field: SerializeField] public float actorMoveTime;

        [field: SerializeField] public Vector3 actorStartPos;
        [field: SerializeField] public Vector3 actorEndPos;
        [field: SerializeField] public Vector3 actorStartDir;
        [field: SerializeField] public Vector3 actorEndDir;

        [Header("Dialogue")]
        [field: SerializeField] public bool needDialogue;
        [field: SerializeField] public int dialogueDataKey;

        [Header("Option")]
        [field: SerializeField] public bool moveCamera;

        [field: SerializeField] public float cameraMoveTime;
        [field: SerializeField] public Vector3 cameraEndPos;
        [field: SerializeField] public Vector3 cameraEndDir;
    }
}
