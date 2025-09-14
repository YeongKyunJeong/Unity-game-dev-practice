using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopScoreCalculator : MonoBehaviour
{
    private int[] popingJewelTypeCounts;

    public void Initialize(int puzzleSize)
    {
        popingJewelTypeCounts = new int[puzzleSize];
    }

    public void PopingToScore(int[] popingJewelTypeCounts)
    {

    }
}
