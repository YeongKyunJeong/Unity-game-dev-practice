using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private GameManager gameManager;
    public Rigidbody2D paddleRigidBody { get; private set; }
    public Transform paddleTransform { get; private set; }
    public Vector2 direction { get; private set; }

    private string horizontalBtn = "Horizontal";
    private string escButton = "Cancel";
    private float horizontalInput = 0;
    private float inputMin = 0.1f;
    private bool isMoving = false;

    public static event Action<float> OnMovementInput;
    public static event Action OnESCInput;

    public void Initialize()
    {
        if (gameManager == null)
            gameManager = GameManager.Instance;
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw(horizontalBtn);

        if (horizontalInput == 0 && isMoving)
        {
            isMoving = false;
            OnMovementInput?.Invoke(0);
        }
        else
        {
            isMoving = true;
            OnMovementInput?.Invoke(horizontalInput);
        }

        if (Input.GetButtonDown(escButton))
        {
            OnESCInput?.Invoke();
        }

    }



}
