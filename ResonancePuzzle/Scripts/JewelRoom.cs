using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelRoom : MonoBehaviour
{
    public PolygonCollider2D coll;
    public Vector2Int cord { get; set; }
    public Vector2 cordForCheck;
    public Jewel jewel { get; set; }
    public SpriteRenderer spriteRenderer;

    public int jewelType = -1; // 0 ~ 6 : Jewel, 10 : Trap, 20: Item
    public int state = 0; // -1: ���� Ŭ������ Ȱ��ȭ ����  0: ��Ȱ��ȭ / 1: �̹� Ŭ������ Ȱ��ȭ / 2: Ȱ��ȭ �Ϸ�

    public int nextChainDir = 5; // 5: Ŭ�� �Ұ�, 1: ��, 2: ��. 3: ��, 4: ��, 6: ��, 7: ��, 8: ��, 9: ��


    public void Initialize(Jewel jewel, Vector2Int cord, JewelData jewelData)
    {
        if (coll == null)
            coll = GetComponent<PolygonCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        jewelType = Random.Range(0, 7);
        this.jewel = jewel;
        this.cord = cord;
        jewel.Initialize(cord, jewelData);
        JewelUpdate(false, jewelType);
    }

    public void JewelUpdate(bool isPop = true, int targetID = -1)
    {
        if (targetID == -1) // ���� ������ �������� ����
            jewelType = Random.Range(0, 7);
        else
        {
            jewelType = targetID;
        }

        jewel.ChangeJewelSprite(isPop, jewelType);

    }
}
