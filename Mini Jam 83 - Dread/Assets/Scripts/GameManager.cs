using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    public static GameManager Instance { get; private set; }
    public int CurrentRoomIndex { get; set; }
    public Room CurrentRoom { get; set; }

    [SerializeField] AudioClip LevelMusic;
    [SerializeField] AudioClip LevelMusicOverlay1, LevelMusicOverlay2;

    LevelGenerator levelGenerator;
    CameraController cameraController;
    PersistentData persistentData;
    AudioStation audioStation;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        levelGenerator = LevelGenerator.Instance;
        cameraController = CameraController.Instance;
        persistentData = PersistentData.Instance;
        audioStation = AudioStation.Instance;

        StartGame();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            MonstersTakeTurn();
        }
    }

    void StartGame()
    {
        persistentData.CurrentLevel = 1;
        levelGenerator.Generate();

        Room entranceRoom = levelGenerator.Rooms[0];
        Vector3 entranceRoomPos = entranceRoom.transform.localPosition;
        Vector3 offset = new Vector3(-entranceRoom.ExitDoor.Dir.x * 6f, 1.5f,
                                     -entranceRoom.ExitDoor.Dir.z * 6f);
        Instantiate(playerPrefab, entranceRoomPos + offset, Quaternion.identity);

        UpdateCurrentRoom(CurrentRoomIndex);
        cameraController.RefreshToCurrentRoom();

        audioStation.StartNewMusicPlayer(LevelMusic);
    }

    [ContextMenu("Next Room")]
    public void NextRoom()
    {
        if (CurrentRoomIndex != levelGenerator.NumberOfRooms - 1)
        {
            UpdateCurrentRoom(CurrentRoomIndex++);
            cameraController.RefreshToCurrentRoom();
        }
        else
            Win();
    }

    public void MonstersMoved()
    {
        Debug.Log("Monsters ended turn");
    }

    public void MonstersTakeTurn()
    {
        foreach(Transform monster in CurrentRoom.transform.Find("Props").Find("Enemies"))
        {
            monster.GetComponent<MonsterAI>().TakeTurn();
            break;
        }
    }

    void UpdateCurrentRoom(int index)
    {
        CurrentRoom = levelGenerator.Rooms[index];
    }

    void Win()
    {
        print("level " + persistentData.CurrentLevel + " complete!");
        persistentData.CurrentLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}