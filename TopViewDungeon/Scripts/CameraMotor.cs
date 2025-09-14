using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    public float boundX = 0.3f;
    public float boundY = 0.15f;

    private float lerpSpeed = 0.02f;
    public bool isLerp = false;
    private Vector3 zCorrectedVector = Vector3.zero;

    private void Start()
    {
        lookAt = GameManager.instance.player.transform;
    }

    private void LateUpdate() // Follow after moving is done, unless there can be very small desync
    {
        Vector3 delta = Vector3.zero;
        isLerp = true;

        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX + 0.001f;
            }
            else
            {
                delta.x = deltaX + boundX - 0.001f;
            }
        }


        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY + 0.001f;
            }
            else
            {
                delta.y = deltaY + boundY - 0.001f;
            }
        }

        //if (isLerp)
        //{
        //    zCorrectedVector = Vector3.Lerp(transform.position, lookAt.position, /*Time.deltaTime**/lerpSpeed);
        //    zCorrectedVector.z = -10;
        //    transform.position = zCorrectedVector;
        //}
        //else
            transform.position += new Vector3(delta.x, delta.y, 0);

    }
}
