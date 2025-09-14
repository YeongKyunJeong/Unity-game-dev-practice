using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace TDP
{

    public class StageMaker : MonoBehaviour
    {
        [SerializeField] Transform nodeParent;
        [SerializeField] GameObject nodePrefab;

        [SerializeField] private bool temporaryNodeGenerater;
        //private void OnValidate()
        //{
        //    if (temporaryNodeGenerater)
        //    {
        //        temporaryNodeGenerater = false;
        //        if (!Application.isPlaying)
        //        {
        //            Node[] nodes = nodeParent.GetComponentsInChildren<Node>();

        //            for (int i = 0; i < 16; i++)
        //            {
        //                for (int j = 0; j < 16; j++)
        //                {
        //                    nodes[i + 16 * j].gameObject.name = $"Node ({i + 16 * j})";
        //                    nodes[i + 16 * j].transform.position = new Vector3(i, 0, j) * 5f;
        //                }
        //            }
        //        }
        //    }
        //}

    }

}