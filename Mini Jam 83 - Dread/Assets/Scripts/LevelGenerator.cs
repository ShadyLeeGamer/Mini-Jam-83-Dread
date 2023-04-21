using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] Room[] roomPrefabs;
    [SerializeField] Hallway hallwayPrefab;
    [SerializeField] Door doorPrefab;

    [SerializeField] Transform BG;

    [SerializeField] int levelSizeX = 4, levelSizeZ = 4;

    RoomManager mapHolder;

    public List<Room> Rooms { get; set; }
    bool[,] map;
    public int NumberOfRooms => Rooms.Count;

    public List<KeySpot> ActiveKeySpots { get; set; }
    public List<KeySpot> KeySpots { get; set; }

    public Vector3Int[] directions =
    {
        new Vector3Int(0, 0, -1), // DOWN
        new Vector3Int(-1, 0, 0), // LEFT
        new Vector3Int(1, 0, 0) // RIGHT
    };

    public static LevelGenerator Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    float distanceX, distanceZ;
    float hallwayLength;
    Vector3 mapCentre;
    public void Generate()
    {
        #region PRE-GENERATION
        SetupMapHolder();

        hallwayLength = hallwayPrefab.transform.GetChild(0).localScale.x;
        distanceX = roomPrefabs[0].GetSizeX() + (hallwayLength * 2);
        distanceZ = roomPrefabs[0].GetSizeZ() + (hallwayLength * 2);

        mapCentre = new Vector3(distanceX * (levelSizeX - 1) / 2f, 0f,
                              -(distanceZ * (levelSizeZ - 1) / 2f));
        SetupBGTransform();
        #endregion

        Rooms = new List<Room>();
        map = new bool[levelSizeX, levelSizeZ];

        // Create entrance from top
        int roomPosX = Random.Range(0, levelSizeX);
        int roomPosZ = 0;
        CreateRoom(roomPosX * distanceX, roomPosZ, RoomType.Entrance, roomPosX);

        // Create rooms middle to exit
        while (roomPosX >= 0 && roomPosX <= levelSizeX - 1 &&
               roomPosZ <= 0 && roomPosZ >= -levelSizeZ + 1)
        {
            // Left, right or down (more downs to encorage more of a linear shape)
            int randomDirIndex = Random.Range(0, directions.Length);
            Vector3Int randomDir = directions[randomDirIndex];

            int newPosX = roomPosX + randomDir.x;
            int newPosZ = roomPosZ + randomDir.z;

            // Check if we're hitting the edge of the map
            if (newPosX < 0 || newPosX > levelSizeX - 1 ||
                newPosZ > 0 || newPosZ < -levelSizeZ + 1)
            {
                // If we're not hitting bottom edge, then move down and create a new middle room
                if (roomPosZ > -levelSizeZ + 1)
                {
                    randomDir = directions[0];
                    roomPosX += randomDir.x;
                    roomPosZ += randomDir.z;

                    CreateMiddleRoom(roomPosX, roomPosZ, randomDir);
                    continue;
                }
                // If we are on bottom edge, then mark new room as exit and end generation
                else if (roomPosZ == -levelSizeZ + 1)
                {
                    Room exitRoom = Rooms[Rooms.Count - 1];
                    exitRoom.ApplyType(RoomType.Exit);

                    CreateHallway(exitRoom, directions[0], PassType.Exit);
                    CreateDoor(exitRoom, directions[0], PassType.Exit);
                    break;
                }
            }

            // If chosen direction is blocked by another room, then pick another
            if (map[newPosX, -newPosZ] == true)
                continue; 

            // If we are just within the map, then create a middle room
            roomPosX = newPosX;
            roomPosZ = newPosZ;

            CreateMiddleRoom(roomPosX, roomPosZ, randomDir);
        }

        // Create path nodes for enemy AI
        mapHolder.CreateNodes();
    }

    void SetupMapHolder()
    {
        if (mapHolder)
            DestroyImmediate(mapHolder.gameObject);

        mapHolder = new GameObject("Room Manager").AddComponent<RoomManager>();
        mapHolder.transform.SetParent(transform);
    }

    void SetupBGTransform()
    {
        BG.transform.position = new Vector3(mapCentre.x, -1f,
                                            mapCentre.z);
        BG.transform.localScale = new Vector3(hallwayLength + (distanceX * levelSizeX), 1f,
                                              hallwayLength + (distanceZ * levelSizeZ));
    }

    void CreateMiddleRoom(int roomPosX, int roomPosZ, Vector3Int dir)
    {
        Room prevRoom = Rooms[Rooms.Count - 1];
        CreateConnectionBetween(prevRoom,
                             CreateRoom(roomPosX * distanceX, roomPosZ * distanceZ,
                             RoomType.Middle, roomPosX, roomPosZ), dir);
    }

    Room CreateRoom(float posX, float posZ,
                    RoomType type, int coordX = 0, int coordZ = 0)
    {
        Room randomRoomPrefab = roomPrefabs[Random.Range(0, roomPrefabs.Length)];
        Room newRoom = Instantiate(randomRoomPrefab, new Vector3(posX, 0f, posZ), Quaternion.identity);
        newRoom.Initialise(coordX, coordZ, type);
        newRoom.transform.SetParent(mapHolder.transform);
        Rooms.Add(newRoom);
        map[coordX, -coordZ] = true;
        return newRoom;
    }

    void CreateConnectionBetween(Room room1, Room room2, Vector3Int dir)
    {
        room1.NextNeighbour = room2;
        room2.PrevNeighbour = room1;

        CreateHallway(room1, dir, PassType.Exit);
        CreateHallway(room2, dir, PassType.Entry);

        CreateDoor(room1, dir, PassType.Exit);
        CreateDoor(room2, dir, PassType.Entry);
    }

    void CreateHallway(Room room, Vector3Int dir, PassType type)
    {
        // POSITION
        float distanceX = (this.distanceX / 2f) - hallwayLength / 2f;
        float distanceZ = (this.distanceZ / 2f) - hallwayLength / 2f;
        int posOffset = type == PassType.Exit ? 1 : -1;
        float hallwayPosX = room.transform.localPosition.x + distanceX * dir.x * posOffset;
        float hallwayPosZ = room.transform.localPosition.z + distanceZ * dir.z * posOffset;

        // SPAWN WITH POSITION
        Hallway newHallway = Instantiate(hallwayPrefab, new Vector3(hallwayPosX, 0f,
                                                                    hallwayPosZ), Quaternion.identity);
        newHallway.transform.SetParent(room.transform);
        newHallway.Initialise(dir, type);

        // ADD TO ROOM
        room.SetHallway(newHallway);
    }

    void CreateDoor(Room room, Vector3Int dir, PassType type)
    {
        // POSITION
        int posOffset = type == PassType.Exit ? 1 : -1;
        float doorPosX = room.transform.localPosition.x + (room.GetSizeX() / 2f) * dir.x * posOffset;
        float doorPosZ = room.transform.localPosition.z + (room.GetSizeX() / 2f) * dir.z * posOffset;

        // SPAWN WITH POSITION
        Door newDoor = Instantiate(doorPrefab, new Vector3(doorPosX, 2.35f,
                                                           doorPosZ), Quaternion.identity);
        newDoor.transform.SetParent(room.transform);
        newDoor.Initialise(dir, type);

        // ADD TO ROOM
        room.SetDoor(newDoor);
    }

    void SortKeySpots()
    {
        // SET KEY SPOTS ACTIVE
        for (int i = 0; i < Rooms.Count; i++)
        {
            Rooms[i].SortKeySpots();
        }

        // ACTIVATE SOME KEY SPOTS IF NON ARE ACTIVE
        if (ActiveKeySpots.Count == 0)
        {
            int numOfSpotsToActivate = (levelSizeX + levelSizeZ) / 2;
            for (int i = 0; i < numOfSpotsToActivate; i++)
                Rooms[Random.Range(0, Rooms.Count)].SetKeySpotActive(null, true);
        }

        // HIDE KEY IN RANDOM ACTIVE KEY SPOT
        ActiveKeySpots[Random.Range(0, ActiveKeySpots.Count)].HideKey();
    }
}