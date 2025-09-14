using UnityEngine;

namespace RSP2
{
    public class MoverForPlayer : MonoBehaviour
    {
        private Player player;
        private PlayerScriptableObject sOData;
        private MovementStateDataForPlayer movementStateData;
        //private AttackDataLibrary attackData;
        private RuntimeDataForPlayer runtimeData;
        //private PlayerInputReader inputReader;
        private CharacterController controller;

        private Vector2 movementInputVector;
        private Vector3 nextVerticalVelocityVector;
        private Vector3 nextHorizontalMovementVector;
        private Vector3 nextForceVector;
        private Vector3 nextRotationVector;
        private Transform mainCameraTransform;
        private bool keepRotation;

        private bool needToSetHeight;
        private Vector3 targetHeight;

        public void Initialize(Player _player)
        {
            player = _player;
            sOData = _player.SOData;
            movementStateData = sOData.MovementStateData;
            //attackData = sOData.AttackDataLibrary;
            runtimeData = _player.RuntimeData;
            //inputReader = GetComponent<PlayerInputReader>();
            controller = _player.Controller;
            mainCameraTransform = Camera.main.transform;

            nextVerticalVelocityVector = 5 * Time.deltaTime * Physics.gravity;
            nextForceVector = Vector3.zero;
            nextRotationVector = transform.forward;
            keepRotation = false;
            needToSetHeight = false;
            //fixedDeltaTime = Time.fixedDeltaTime;
        }

        public void CallPhysicsUpdate()
        {
            return;
        }

        public void CallUpdate()
        {
            ApplyUpdatedMovement();
            return;
        }

        private void ApplyUpdatedMovement()
        {

            controller.Move((nextVerticalVelocityVector + nextHorizontalMovementVector + nextForceVector) * Time.deltaTime);

            // keepRotation : Not moving, just rotation (nextHorizontalMovementVector == Vector3.Zero)
            if (keepRotation || (nextHorizontalMovementVector != Vector3.zero))
            {
                Rotate(nextRotationVector);
            }


            nextVerticalVelocityVector = 5 * Time.deltaTime * Physics.gravity;
            nextForceVector = Vector3.zero;

        }

        public void UpdateNextVerticalVelocityVector(Vector3 velocityVector)
        {
            nextVerticalVelocityVector = velocityVector;
        }

        public void UpdateNextHorizontalMovementVector(Vector3 movementVector)
        {
            nextHorizontalMovementVector = movementVector;
            if (movementVector != Vector3.zero)
            {
                nextRotationVector = movementVector;
                nextRotationVector.y = 0;
            }
        }

        public void UpdateNextForceVector(Vector3 forceVector)
        {
            nextForceVector = forceVector;
        }

        private void Rotate(Vector3 targetDir)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir),
                Time.deltaTime * movementStateData.RotationSpeedModifier);
        }

        public void SetHeightManually(Vector3 targetHeightVector)
        {
            needToSetHeight = true;
            targetHeight = targetHeightVector;
        }

        public void SetKeepRotate(bool isKeep)
        {
            keepRotation = isKeep;
        }
    }
}
