using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class Room : MonoBehaviour
{
    [SerializeField] Transform floor;
    [SerializeField] TextMeshPro coordsTXT;
    [SerializeField] Transform props;

    [SerializeField] KeySpot[] keySpots = new KeySpot[2];

    public Room PrevNeighbour { get; set; }
    public Room NextNeighbour { get; set; }

    public Hallway EntryHallway { get; set; }
    public Hallway ExitHallway { get; set; }
    public List<Hallway> hallways = new List<Hallway>(4);

    public Door EntryDoor { get; set; }
    public Door ExitDoor { get; set; }
    public List<Door> doors = new List<Door>(4);

    [SerializeField] GameObject[] hideWalls = new GameObject[4];

    public RoomType Type { get; set; }

    LevelGenerator levelGenerator;

    void Awake()
    {
        //levelGenerator = LevelGenerator.Instance;
    }

    public void Initialise(int posX, int posZ, RoomType type)
    {
        coordsTXT.text = new Vector2Int(posX, posZ).ToString();
        coordsTXT.gameObject.SetActive(false);

        // RANDOM ROTATION
        int randomRotationMultiplier = Random.Range(0, 4);
        props.localEulerAngles = new Vector3(0, 90 * randomRotationMultiplier, 0);

        ApplyType(type);
    }

    public void ApplyType(RoomType type)
    {
        Type = type;
        gameObject.name = type + " Room";

        SetMaterialColour(floor.gameObject);
    }

    void SetMaterialColour(GameObject rendererObject)
    {
        if (!rendererObject)
            return;

        Color colour = default;
        switch (Type)
        {
            case RoomType.Entrance:
                colour = Color.green;
                break;
            case RoomType.Middle:
                colour = Color.white;
                break;
            case RoomType.Exit:
                colour = Color.red;
                break;
        }
        var tempMat = new Material(rendererObject.GetComponent<Renderer>().sharedMaterial);
        tempMat.color = colour;
        rendererObject.GetComponent<Renderer>().sharedMaterial = tempMat;
    }

    public float GetSizeX()
    {
        return floor.transform.localScale.x;
    }

    public float GetSizeZ()
    {
        return floor.transform.localScale.z;
    }

    public void SetHallway(Hallway hallway)
    {
        int roomHallwayIndex = 0;
        bool typeIsExit = hallway.Type == PassType.Exit;
        if (hallway.Dir.x == -1) // LEFT
            roomHallwayIndex = typeIsExit ? 0 : 2;
        if (hallway.Dir.x == 1) // RIGHT
            roomHallwayIndex = typeIsExit ? 2 : 0;

        if (hallway.Dir.z == -1) // DOWN
            roomHallwayIndex = typeIsExit ? 1 : 3;
        hallways[roomHallwayIndex] = hallway;

        if (typeIsExit)
            ExitHallway = hallway;
        else
            EntryHallway = hallway;
    }

    public void SetDoor(Door door)
    {
        int roomDoorIndex = 0;
        bool typeIsExit = door.Type == PassType.Exit;
        if (door.Dir.x == -1) // LEFT
            roomDoorIndex = typeIsExit ? 0 : 2;
        if (door.Dir.x == 1) // RIGHT
            roomDoorIndex = typeIsExit ? 2 : 0;

        if (door.Dir.z == -1) // DOWN
            roomDoorIndex = typeIsExit ? 1 : 3;
        doors[roomDoorIndex] = door;

        if (typeIsExit)
        {
            ExitDoor = door;

            if (Type == RoomType.Exit)
                ExitDoor.SetLock(true);
        }
        else
            EntryDoor = door;
    }

    [ContextMenu("Win")]
    public void Win()
    {
        TransitionHideWall();
    }

    public void TransitionHideWall()
    {
        //StartCoroutine(FadeOut(type == HallwayType.Exit ? ExitHallway : EntryHallway));
        //NextNeighbour.TransitionHideWall(HallwayType.Entry);
    }

    IEnumerator FadeOut(Hallway hallway)
    {
        int exitHideWallIndex = hallways.IndexOf(hallway);
        print(exitHideWallIndex);
        var tempMat = new Material(hideWalls[exitHideWallIndex].GetComponent<Renderer>().sharedMaterial);
        for (float f = 1f; f >= -.05f; f -= .05f)
        {
            Color tempMatColour = tempMat.color;
            tempMatColour.a = f;
            tempMat.color = tempMatColour;
            hideWalls[exitHideWallIndex].GetComponent<Renderer>().sharedMaterial = tempMat;
            yield return new WaitForSeconds(.05f);
        }
    }

    public void SortKeySpots()
    {
        //levelGenerator = LevelGenerator.Instance;

        for (int i = 0; i < keySpots.Length; i++)
        {
            bool isActive = Random.value >= .5f;

            SetKeySpotActive(keySpots[i], isActive);
        }
    }

    public void SetKeySpotActive(KeySpot keySpot, bool value)
    {
        if (!keySpot)
            keySpot = keySpots[Random.Range(0, keySpots.Length)];

        keySpot.gameObject.SetActive(value);

        //LevelGenerator.Instance.KeySpots.Add(keySpot);
        if (value)
            LevelGenerator.Instance.ActiveKeySpots.Add(keySpot);
    }
}

public enum RoomType
{
    Entrance,
    Middle,
    Exit
}