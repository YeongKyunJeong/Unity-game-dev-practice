using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mover : Fighter // abstract : ����ؼ� �� �ǵ� ���� ��ũ��Ʈ�� ������Ʈ�� �޾Ƽ� ���� ���� ��
{
    private Vector3 originalSize;

    protected BoxCollider2D boxCollider;
    protected Vector3 moveDelta;
    protected RaycastHit2D hit;
    public float ySpeed = 0.75f; //*** ��ǥ �������� �̵��� �� �������� �̵��� �� �����Ƿ� �����ؾ� ��
    public float xSpeed = 1.0f;


    protected virtual void Start()
    {
        originalSize = transform.localScale;
        boxCollider = GetComponent<BoxCollider2D>();
    }


    protected virtual void UpdateMotor(Vector3 input)
    {
        // Reset MoveDelta
        moveDelta = new Vector3 (input.x *xSpeed, input.y*ySpeed, 0 );

        // Swap sprite direction, wether you're going right or left
        if (moveDelta.x > 0)
        {
            transform.localScale = originalSize;
        }
        else if (moveDelta.x < 0)
        {
            transform.localScale = new Vector3(-originalSize.x, originalSize.y, originalSize.z);
        }

        // Add push vector, if any
        moveDelta += pushDirection;

        // Reduce push force every frame, based off recovery speed
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverSpeed); 
        //*** Vector3.Lerp�� ����ϸ� �����ӿ� ���� push force�� 0�� �Ǵ� �ӵ��� �޶����� �ʳ�?

        // Make sure we can move in this direction, by casting a box first. If the box returns null, we're free to move
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // Make this thing move;
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);

        }
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime)/*����?*/, LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {
            // Make this thing move;
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);

        }
    }

}
