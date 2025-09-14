using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

namespace RSP2
{
    public class ActionStateForPlayer : IState
    {
        protected Player player;

        protected RuntimeDataForPlayer runtimeData;
        protected MovementStateDataForPlayer movementStateData;
        protected AttackDataLibrary attackDataLibrary;

        protected Transform mainCameraTransform;

        protected ActionStateMachineForPlayer stateMachine;
        protected StatHandlerForPlayer statHandler;
        protected PlayerInputReader inputReader;
        protected MoverForPlayer mover;
        protected ForceReceiver forceReceiver;
        protected CharacterController controller;
        protected Animator animator;

        protected AnimatorStateInfo animationStateInfo;


        private RaycastHit hit;
        private Vector3 slopeDetectingRayVector;
        private float slopeDetectingRayMaxDistance;
        private Vector3 slopeDetectingRayStartHeightVector;
        private Vector3 floatingHeightVector;
        private LayerMask groundLayer;
        private Transform playerTransform;

        protected Vector2 moveInput;

        public ActionStateForPlayer(Player _player, ActionStateMachineForPlayer _stateMachine)
        {
            player = _player;
            stateMachine = _stateMachine;

            runtimeData = player.RuntimeData;
            movementStateData = player.SOData.MovementStateData;
            attackDataLibrary = player.SOData.AttackDataLibrary;

            statHandler = _player.StatHandler;
            inputReader = player.InputReader;
            mover = player.Mover;
            forceReceiver = player.ForceReceiver;
            controller = player.Controller;
            animator = player.Animator;


            playerTransform = player.transform;

            slopeDetectingRayStartHeightVector = Vector3.up * movementStateData.SlopeDetectingRayStartHeight;
            slopeDetectingRayVector = Vector3.down * (movementStateData.RaycastDistance + movementStateData.SlopeDetectingRayStartHeight);
            slopeDetectingRayMaxDistance = movementStateData.RaycastDistance + movementStateData.SlopeDetectingRayStartHeight;
            floatingHeightVector = Vector3.up * (movementStateData.FloatingHeight);

            groundLayer = movementStateData.GroundLayer;

        }

        #region IState Methods

        public virtual void Enter()
        {
            inputReader.MoveEvent += OnMoveInput;
            inputReader.JumpEvent += OnJumpInput;
            inputReader.WalkToggleEvent += OnWalkToggleInput;
            inputReader.DashEvent += OnDashInput;
            inputReader.AttackEvent += OnAttackInput;
            inputReader.AimEvent += OnAimInput;
            inputReader.QSkillEvent += OnQSkillInput;
            inputReader.ESkillEvent += OnESkillInput;


            moveInput = runtimeData.MoveInput;
        }

        public virtual void Enter(int dataKey)
        {
            Enter();
        }

        public virtual void Exit()
        {
            inputReader.MoveEvent -= OnMoveInput;
            inputReader.JumpEvent -= OnJumpInput;
            inputReader.WalkToggleEvent -= OnWalkToggleInput;
            inputReader.DashEvent -= OnDashInput;
            inputReader.AttackEvent -= OnAttackInput;
            inputReader.AimEvent -= OnAimInput;
            inputReader.QSkillEvent -= OnQSkillInput;
            inputReader.ESkillEvent -= OnESkillInput;
        }

        public virtual void CallUpdate()
        {
        }


        public virtual void CallPhysicsUpdate()
        {
        }

        public virtual void OnAnimationEnterEvent()
        {
        }

        public virtual void OnAnimationExitEvent()
        {
        }

        public virtual void OnAnimationTransitEvent()
        {
        }

        #endregion


        #region Movement Input Method

        protected virtual void OnMoveInput(Vector2 _moveInput)
        {
            moveInput = _moveInput;
            runtimeData.MoveInput = moveInput;
        }

        protected virtual void OnJumpInput() { }

        protected virtual void OnWalkToggleInput()
        {
            runtimeData.IsWalking = !runtimeData.IsWalking;
        }

        protected virtual void OnDashInput() { }

        protected virtual void OnAttackInput() { }

        protected virtual void OnQSkillInput() { }

        protected virtual void OnESkillInput() { }

        protected virtual void OnAimInput() { }

        #endregion


        protected virtual void SetAnimatorSelfStateParameter(bool isOn) { }



        protected float GetNormalizedTime(Animator animator, string tag)
        {
            if (animator.IsInTransition(0))
            {
                animationStateInfo = animator.GetNextAnimatorStateInfo(0);
                return animationStateInfo.IsTag(tag) ? animationStateInfo.normalizedTime : -1f;
            }
            else
            {
                animationStateInfo = animator.GetCurrentAnimatorStateInfo(0);
                return animationStateInfo.IsTag(tag) ? animationStateInfo.normalizedTime : -1f;
            }
        }
    }

    public static class FallingCalculator
    {
        private static Vector3 gravity = Physics2D.gravity;
        private static float fallingThreshold;

        private static RaycastHit hit;
        private static Vector3 slopeDetectingRayStart = new Vector3(0, 0.25f, 0);
        private static float slopeDetectionRayDistance = 0.5f;
        private static LayerMask groundLayer = 1 << LayerMask.NameToLayer("Environment");

        public static void ApplyFallingToVector(ref Vector3 velocityVector, float timeDelta)
        {
            velocityVector += timeDelta * gravity;
            return;
        }

        public static bool CheckFalling(Vector3 fallingVelocityVector, Vector3 slopeNormalVector, CharacterController controller)
        {
            if (slopeNormalVector.y < -0.98f)
            {
                fallingThreshold = 3 * Physics.gravity.y * Time.deltaTime;
                if (!controller.isGrounded && (fallingVelocityVector.y < fallingThreshold))
                {
                    return true;
                }
            }
            return false;
        }

        public static Vector3 CheckIsSlope(Transform transform, bool stickFloor = true)
        {
            if (Physics.Raycast(transform.position + slopeDetectingRayStart, Vector3.down, out hit, slopeDetectionRayDistance,
                groundLayer))
            {
                return hit.normal;
            }

            return Vector3.down;

        }
    }



    public static class InputToDirectionVectorConverter
    {
        static Transform mainCameraTransform = Camera.main.transform;
        //static Vector3 horizontalMovementVector;
        private static Vector3 forward;
        private static Vector3 right;

        private static Vector3 vectorOnXZ;
        private static Vector3 vectorOnSlope;

        public static Vector3 ConvertInputToMovementDirectionVector(Vector3 input)
        {
            forward = mainCameraTransform.forward;
            right = mainCameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            return (forward * input.y + right * input.x);
        }

        public static Vector3 ConvertInputToMovementDirectionVectorOnSlope(Vector3 input, Vector3 normal)
        {
            vectorOnXZ = ConvertInputToMovementDirectionVector(input);

            vectorOnSlope = (vectorOnXZ - Vector3.Dot(vectorOnXZ, normal) * normal).normalized;

            return vectorOnSlope;
        }

    }



}
