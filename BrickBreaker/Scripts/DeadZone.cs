using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private LayerMask ballLayer;
    private GameManager gameManager;
    [SerializeField] private bool isBelowWall = false;

    public void Initialize(LayerMask ballLayer)
    {
        gameManager = GameManager.Instance;
        this.ballLayer = ballLayer;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == ballLayer)
        {
            gameManager.DeadZoneOut(collision.gameObject);
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (isBelowWall)
    //        if (collision.gameObject.layer == ballLayer)
    //        {
    //            gameManager.DeadZoneOut();
    //        }
    //}
}
