using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MazeConfiguration
{
    public int Rooms;
    public int Threats;
    public int Treasures;

    public MazeConfiguration(int rooms, int threats, int treasures)
    {
        Rooms = rooms;
        Threats = threats;
        Treasures = treasures;
    }

    public MazeConfiguration()
    {
    }
}
