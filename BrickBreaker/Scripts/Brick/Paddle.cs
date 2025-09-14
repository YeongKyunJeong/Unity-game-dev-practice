using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    private PaddleHandler paddleHandler;

    public float speed = 100f;
    private bool doDamping = false;
    private const float MINIMUM_SPEED = 1 / 1024f;
    private static float paddleSizeHalf = 2.5f;
    private static float screenEdgeX = 18f;
    private static float stopXPos;
    private Vector3 leftStopPosVec;
    private Vector3 rightStopPosVec;
    private bool isFirstTimeUsage = true;
    // public Rigidbody2D rigidBody { get; private set; }

    public Vector2 firstPosition { get; private set; }

    public Vector3 velocity;

    public float dampingTime = 0.1f;

    public float stackTime = 0f;

    //public void GlobalInitializer()
    //{
    //}

    public void Initialize()
    {
        // if (rigidBody == null)
        // {
        //     rigidBody = GetComponent<Rigidbody2D>();
        //
        // }
        firstPosition = transform.position;
        if (paddleHandler == null)
        {
            paddleHandler = GetComponent<PaddleHandler>();
        }
        paddleHandler.Initialize();

        if (isFirstTimeUsage)
        {
            paddleHandler.OnMovementInput += MovePaddle;
        }
        isFirstTimeUsage = false;

        ResetPaddle();

        stopXPos = screenEdgeX - paddleSizeHalf;
        leftStopPosVec = new Vector3(-stopXPos, firstPosition.y, 0);
        rightStopPosVec = new Vector3(stopXPos, firstPosition.y, 0);
    }

    public void MovePaddle(Vector2 moveDirection)
    {
        if (moveDirection == Vector2.zero)
        {
            //rigidBody.velocity = Vector2.zero;
            // stackTime = 0f;
            doDamping = true;
            return;
        }
        else
        {
            doDamping = false;
            velocity = moveDirection;
        }


    }

    public void ChangePaddleStopX(float changedPaddleWidthHalf)
    {
        stopXPos = screenEdgeX - changedPaddleWidthHalf;
        leftStopPosVec = new Vector3(-stopXPos, firstPosition.y, 0);
        rightStopPosVec = new Vector3(stopXPos, firstPosition.y, 0);
    }

    private void Update()
    {
        if (doDamping)
        {
            if (velocity.x != 0)
            {
                if (Mathf.Abs(velocity.x) < MINIMUM_SPEED)
                {
                    velocity = Vector3.zero;
                }
                else
                    velocity = Vector3.SmoothDamp(velocity, Vector2.zero, ref velocity, dampingTime);
            }
        }


        transform.position += velocity * (speed * Time.deltaTime);

        //Debug.Log(velocity.x * (speed * Time.deltaTime));
        if (transform.position.x <= -stopXPos)
        {
            transform.position = leftStopPosVec;
        }

        if (transform.position.x >= stopXPos)
        {
            transform.position = rightStopPosVec;
        }

        //if (doDamping)
        //{
        //}

        // if(stackTime < dampingTime)
        // {
        //     stackTime += Time.deltaTime;
        //     velocity = Vector3.Lerp(velocity, Vector2.zero, stackTime / dampingTime);
        // }

    }

    public void ResetPaddle()
    {
        transform.position = firstPosition;
        //rigidBody.velocity = Vector2.zero;
    }


}
