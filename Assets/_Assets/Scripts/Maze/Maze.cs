using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// This is the main script that handle the generation of the maze from a default block
/// </summary>

//This is the struct that will hold the information about which tile to place when it is not a connecting path
[Serializable]
public struct WallTile
{
    public CardinalDirection Direction;
    public GameObject WallObject;
}

public class Maze : MonoBehaviour
{
    public static Maze Instance;
    
    [SerializeField] private GameObject ExitTile;
    [SerializeField] private GameObject FloorTile;
    [SerializeField] private GameObject DefaultTile;
    [SerializeField] private List<GameObject> Rooms = new List<GameObject>();
    [SerializeField] private WallTile[] WallTiles;

    //Rooms
    
    //Threats
    
    //Treasures
    
    private void Awake()
    {
        if (Instance != null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GenerateWithCurrentConfig();
        GenerateVisualWallsAndPaths();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

#region Generation

    private void GenerateWithCurrentConfig()
    {
        MazeConfiguration currentConfig = MazeConfigUtils.CurrentConfig;
        //TODO Get rid of this after testing
        if (currentConfig == null)
            currentConfig = new MazeConfiguration(10, 4, 6);

        int lastWalkDir = 4;
        Vector3 currentWalkerPos = Vector3.zero;
        for (int i = 0; i < currentConfig.Rooms;)
        {
            //Pick a random direction to walk in
            int randomDir = Random.Range(0, 4);
            var walkDir = GetDirectionOffset((CardinalDirection)randomDir);
            
            //Check if there is an exising room already
            var existingRoom = GetExistingRoom(currentWalkerPos);
            if (existingRoom)
            {
                existingRoom.TryGetComponent(out ConnectingPaths path);
                path.wallData[randomDir].isPath = true;
                currentWalkerPos += walkDir;
                lastWalkDir = randomDir;
                continue;
            }

            //Create new room tile
            GameObject newRoom = Instantiate(DefaultTile, currentWalkerPos, Quaternion.identity);
            Rooms.Add(newRoom);
            newRoom.TryGetComponent(out ConnectingPaths paths);
            
            //Update with the previous direction
            if (lastWalkDir != 4)
            {
                paths.wallData[GetOppositeCardinalDirection(lastWalkDir)].isPath = true;
            }

                //Jump out if it's the last room to generate
            if (i == currentConfig.Rooms - 1)
                break;
            
            //Move to new direction
            paths.wallData[randomDir].isPath = true;
            currentWalkerPos += walkDir;
            lastWalkDir = randomDir;
            i++;
        }

        //Pick a random eligible room to be the exit
        List<ConnectingPaths> potentialRooms = new List<ConnectingPaths>();
        foreach (var room in Rooms)
        {
            if (room.TryGetComponent(out ConnectingPaths path) && !path.wallData[0].isPath)
            {
                potentialRooms.Add(path);
            }
        }

        var randomIndex = Random.Range(0, potentialRooms.Count);
        potentialRooms[randomIndex].wallData[0].isExit = true;
    }

    private void GenerateVisualWallsAndPaths()
    {
        foreach (var room in Rooms)
        {
            room.TryGetComponent(out ConnectingPaths paths);

            for (int i = 0; i < 4; i++)
            {
                if (paths.wallData[i].isExit)
                {
                    var wallTile = Instantiate(ExitTile, room.transform);
                    wallTile.transform.localPosition = GetWallOffset(CardinalDirection.North);
                    continue;
                }
                if (!paths.wallData[i].isPath)
                {
                    var wallTile = Instantiate(WallTiles[i].WallObject, room.transform);
                    wallTile.transform.localPosition = GetWallOffset((CardinalDirection)i);
                    continue;
                }
                var floorTile = Instantiate(FloorTile, room.transform);
                floorTile.transform.localPosition = GetWallOffset((CardinalDirection)i);
            }
        }
    }

    private GameObject GetExistingRoom(Vector3 pos)
    {
        foreach (var room in Rooms)
        {
            if (room.transform.position == pos)
            {
                return room;
            }
        }

        return null;
    }

    private Vector3 GetWallOffset(CardinalDirection dir)
    {
        switch (dir)
        {
            case CardinalDirection.North:
                return new Vector3(0f, 2f, 0);
            case CardinalDirection.East:
                return new Vector3(2f, 0f, 0);
            case CardinalDirection.South:
                return new Vector3(0f, -2f, 0);
            case CardinalDirection.West:
                return new Vector3(-2f, 0f, 0);
            
        }

        return Vector3.zero;
    }

    private Vector3 GetDirectionOffset(CardinalDirection dir)
    {
        switch (dir)
        {
            case CardinalDirection.North:
                return new Vector3(0f, 5f, 0f);
            case CardinalDirection.East:
                return new Vector3(5f, 0f, 0f);
            case CardinalDirection.South:
                return new Vector3(0f, -5f, 0f);
            case CardinalDirection.West:
                return new Vector3(-5f, 0f, 0f);
            
        }

        return Vector3.zero;
    }

    private int GetOppositeCardinalDirection(int dir)
    {
        switch (dir)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 0;
            case 3:
                return 1;
        }

        return 4;
    }

#endregion
    
}
