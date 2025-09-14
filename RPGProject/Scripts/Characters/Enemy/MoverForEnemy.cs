using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RSP2
{
    public class MoverForEnemy : MonoBehaviour
    {
        private Enemy enemy;
        private RuntimeDataForEnemy runtimeData;
        private CharacterController controller;
        private NavMeshAgent navimeshAgent;

        private Transform targetTransform;
        private bool doMoving;
        private float speed;
        private bool onlyRotateThisFrame;
        private bool isGroundedBeforeFrame;

        private Vector3 nextVerticalVelocityVector;
        private Vector3 nextHorizontalMovementVector;

        // No falling state for enemy 
        private Vector3 fallingCalculatedVelocity;

        private Vector3 nextForceVector;
        //private Vector3 nextRotationVector;


        // Start is called before the first frame update
        public void Initialize(Enemy _enemy)
        {
            enemy = _enemy;
            runtimeData = _enemy.RuntimeData;
            controller = _enemy.Controller;
            navimeshAgent = _enemy.NavMeshAgent;

            navimeshAgent.isStopped = true;
            navimeshAgent.updatePosition = false;
            navimeshAgent.speed = 10;
            targetTransform = null;
            doMoving = false;
            speed = 0;
            onlyRotateThisFrame = false;
            isGroundedBeforeFrame = false;

            nextHorizontalMovementVector = Vector3.zero;
            nextVerticalVelocityVector = 5 * Time.deltaTime * Physics.gravity;
            nextForceVector = Vector3.zero;
            //nextRotationVector = transform.forward;

        }

        public void CallFixedUpdate()
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
            if (doMoving)
            {
                navimeshAgent.destination = targetTransform.position;

                nextHorizontalMovementVector = navimeshAgent.desiredVelocity;

                //if (isGroundedBeforeFrame)
                //{
                //    // TO DO:: More Accurate Vertical Movement
                //}

            }
            else if (onlyRotateThisFrame)
            {
                onlyRotateThisFrame = false;

                nextHorizontalMovementVector = targetTransform.position - transform.position;
                nextHorizontalMovementVector.y = 0;

                if (nextHorizontalMovementVector.sqrMagnitude < 0.001)
                {
                    nextHorizontalMovementVector = transform.forward;
                }

                Rotate(nextHorizontalMovementVector);

                nextHorizontalMovementVector = Vector3.zero;
            }
            else
            {
                nextHorizontalMovementVector = Vector3.zero;
            }



            controller.Move((nextVerticalVelocityVector + nextHorizontalMovementVector + nextForceVector) * Time.deltaTime);

            ApplyGravity();

            nextForceVector = Vector3.zero;
            ArrangeNavimeshPosition();
        }

        //private void ApplyUpdatedMovement()
        //{

        //    if (nextHorizontalMovementVector != Vector3.zero)
        //    {
        //        Rotate(nextHorizontalMovementVector);
        //    }

        //    if (onlyRotateThisFrame)
        //    {
        //        onlyRotateThisFrame = false;
        //        nextHorizontalMovementVector = Vector3.zero;
        //    }

        //    controller.Move((nextVerticalVelocityVector + nextHorizontalMovementVector + nextForceVector) * Time.deltaTime);

        //    ApplyGravity();

        //    nextForceVector = Vector3.zero;
        //}

        private void ApplyGravity()
        {
            if (controller.isGrounded) // Reset falling velocity
            {
                isGroundedBeforeFrame = true;
                nextVerticalVelocityVector = 5 * Time.deltaTime * Physics.gravity;
                fallingCalculatedVelocity = Vector3.zero;
            }
            else
            {
                isGroundedBeforeFrame = false;
                fallingCalculatedVelocity += Time.deltaTime * Physics.gravity;
                nextVerticalVelocityVector = fallingCalculatedVelocity;
            }
        }

        public void UpdateNextVerticalVelocityVector(Vector3 velocityVector)
        {
            nextVerticalVelocityVector = velocityVector;
        }

        public void SetTarget(Transform newTargetTransform)
        {
            targetTransform = newTargetTransform;
        }

        public void StartChasing(float speed)
        {
            doMoving = true;
            navimeshAgent.isStopped = false;
            navimeshAgent.speed = speed > 0.5f ? speed : 0.5f;
        }

        public void StopChasing()
        {
            doMoving = false;
            navimeshAgent.isStopped = true;
            onlyRotateThisFrame = false;
        }

        public void ArrangeNavimeshPosition()
        {
            navimeshAgent.nextPosition = transform.position;
        }

        //public void UpdateNextHorizontalMovementVector(Vector3 movementVector)
        //{
        //    nextHorizontalMovementVector = movementVector;
        //    if (movementVector != Vector3.zero)
        //    {
        //        nextHorizontalMovementVector.y = 0; // To safety
        //    }
        //}

        public void UpdateNextForceVector(Vector3 forceVector)
        {
            nextForceVector = forceVector;
        }

        private void Rotate(Vector3 targetDir)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir),
                Time.deltaTime * enemy.RotationSpeedModifier);
        }

        public void SetOnlyRotateThisFrame()
        {
            onlyRotateThisFrame = true;
        }
    }
}
