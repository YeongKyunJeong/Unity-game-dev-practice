using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class CameraController : MonoBehaviour
    {
        [SerializeField] private bool doMoveCamera = true;

        [SerializeField] private float panSpeed = 30f;
        //[SerializeField] private float panBorderTickness = 40f;

        [SerializeField] private float scrollSpeed = 5f;

        private const float MIN_CAMERA_Y = 20f;
        private const float MAX_CAMERA_Y = 100f;

        private float scrollInput;
        private Vector3 pos;


        private void Update()
        {
            if (GameManager.isGameOver)
            {
                this.enabled = false;
                return;
            }

            if (Input.GetKeyDown("m"))
            {
                doMoveCamera = !doMoveCamera;
            }

            if (!doMoveCamera)
                return;

            if (Input.GetKey("w"))
            {
                transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("s"))
            {
                transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("d"))
            {
                transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            }
            if (Input.GetKey("a"))
            {
                transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            }

            //if (Input.GetKey("w") ||
            //    ((Input.mousePosition.y <= Screen.height) && (Input.mousePosition.y >= Screen.height - panBorderTickness)))
            //{
            //    transform.Translate(Vector3.forward * panSpeed * Time.deltaTime, Space.World);
            //}

            //if (Input.GetKey("s") ||
            //   ((Input.mousePosition.y <= panBorderTickness) && (Input.mousePosition.y >= 0)))
            //{
            //    transform.Translate(Vector3.back * panSpeed * Time.deltaTime, Space.World);
            //}
            //if (Input.GetKey("d") ||
            //  ((Input.mousePosition.x <= Screen.width) && (Input.mousePosition.x >= Screen.width - panBorderTickness)))
            //{
            //    transform.Translate(Vector3.right * panSpeed * Time.deltaTime, Space.World);
            //}
            //if (Input.GetKey("a") ||
            //  ((Input.mousePosition.x <= panBorderTickness) && (Input.mousePosition.x >= 0)))
            //{
            //    transform.Translate(Vector3.left * panSpeed * Time.deltaTime, Space.World);
            //}

            scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput > 0.05f || scrollInput < -0.05f)
            {
                pos = transform.position;
                pos.y -= scrollInput * 1000 * scrollSpeed * Time.deltaTime;
                pos.y = Mathf.Clamp(pos.y, MIN_CAMERA_Y, MAX_CAMERA_Y);

                transform.position = pos;
            }


        }

    }
}
