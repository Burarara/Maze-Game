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
    
    [Header("Rooms")]
    [SerializeField] private GameObject ExitTile;
    [SerializeField] private GameObject FloorTile;
    [SerializeField] private GameObject DefaultTile;
    [SerializeField] private List<GameObject> Rooms = new List<GameObject>();
    [SerializeField] private WallTile[] WallTiles;

    //Player
    [SerializeField] private GameObject PlayerPrefab;
    
    //Threats
    [SerializeField] private List<GameObject> ThreatPrefabs;

    //Treasures
    [SerializeField] private GameObject TreasurePrefab;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GenerateWithCurrentConfig();
        GenerateVisualWallsAndPaths();
        GeneratePlaceables();
    }

    private void OnDestroy()
    {
        Instance = null;
    }

#region Maze Generation

    private void GenerateWithCurrentConfig()
    {
        MazeConfiguration currentConfig = MazeConfigUtils.CurrentConfig;
        //TODO Get rid of this after testing
        if (currentConfig == null)
        {
            currentConfig = new MazeConfiguration(10, 4, 6);
            MazeConfigUtils.CurrentConfig = currentConfig;
        }
        
        //Adding 1 to account for the empty room that the player spawns in.
        int placeableTotal = currentConfig.Threats + currentConfig.Treasures + 1;
        int roomAmount = placeableTotal > currentConfig.Rooms ? placeableTotal : currentConfig.Rooms;

        int lastWalkDir = 4;
        Vector3 currentWalkerPos = Vector3.zero;
        for (int i = 0; i < roomAmount;)
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
            if (i == roomAmount - 1)
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

#region Placeable Generation

    private void GeneratePlaceables()
    {
        MazeConfiguration currentConfig = MazeConfigUtils.CurrentConfig;
        Vector3 offset = new Vector3(0f, 0f, -0.1f);
        //Shuffle the room list before placing the assets
        List<GameObject> shuffledRooms = Shuffle(Rooms);

        //Place player in index 0
        var newPlayer = Instantiate(PlayerPrefab, shuffledRooms[0].transform.position, Quaternion.identity);
        newPlayer.transform.position += offset;
        Camera.main.transform.parent = newPlayer.transform;
        Camera.main.transform.localPosition = new Vector3(0, 0, -10);

        int roomIndex = 1;
        //All threats
        for (int i = 0; i < currentConfig.Threats; i++)
        {
            int randomThreat = Random.Range(0, ThreatPrefabs.Count);
            var newThreat = Instantiate(ThreatPrefabs[randomThreat], shuffledRooms[roomIndex].transform);
            newThreat.transform.localPosition += offset;
            roomIndex++;
        }
        //All treasures
        for (int i = 0; i < currentConfig.Treasures; i++)
        {
            var newTreasure = Instantiate(TreasurePrefab, shuffledRooms[roomIndex].transform);
            newTreasure.transform.localPosition += offset;
            roomIndex++;
        }
    }

    private List<T> Shuffle<T>(List<T> listToShuffle)
    {
        List<T> listToReturn = listToShuffle;
        
        for (int i = 0; i < listToReturn.Count; i++)
        {
            T temp = listToReturn[i];
            int randomIndex = Random.Range(i, listToReturn.Count);
            listToReturn[i] = listToReturn[randomIndex];
            listToReturn[randomIndex] = temp;
        }

        return listToReturn;
    }

#endregion

}
