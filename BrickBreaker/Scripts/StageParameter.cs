using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageParameter
{
    public int level;
    public int brickRowCount;
    public int brickPerRow;
    public float brickSpaceX;
    public float brickSpaceY;
    public string[] rowBrickHealths;
}
