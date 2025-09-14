using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDP
{

    public class WayPoints : MonoBehaviour
    {
        private static List<Transform[]> pointsList;  // Add multiple way 
        //private static Transform[][] pointsArray = new Transform[0][];

        public static void ClearWayPointsList()    // ## I don't know why pointsList not be cleared
        {
            pointsList = new List<Transform[]>();
        }

        public void Initialize()
        {
            Transform[] points = new Transform[transform.childCount];
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = transform.GetChild(i);
            }
            pointsList.Add(points);
        }

        public static Transform[] GetPoints(int wayPointsNumber)
        {
            if (pointsList.Count > wayPointsNumber)
                return pointsList[wayPointsNumber];
            else
                return pointsList[pointsList.Count - 1];
        }
    }
}
