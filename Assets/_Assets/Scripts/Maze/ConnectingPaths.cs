using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardinalDirection
{
    North,
    East,
    South,
    West,
    None
}

[Serializable]
public struct WallData
{
    public bool isPath;
    public bool isExit;
}

public class ConnectingPaths : MonoBehaviour
{
    //Tile display for connecting paths
    public WallData[] wallData = new WallData[4];
}
