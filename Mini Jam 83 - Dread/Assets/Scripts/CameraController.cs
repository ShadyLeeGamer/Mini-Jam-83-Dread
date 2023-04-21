using UnityEngine;

public class CameraController : MonoBehaviour
{
    Transform swivel, stick;

    [SerializeField] float moveSpeedMinZoom = 400f, moveSpeedMaxZoom = 100f;

    [SerializeField] float swivelMinZoom = 90f, swivelMaxZoom = 45f;
    [SerializeField] float stickMinZoom = -250f, stickMaxZoom = -45f;
    float zoom; // ZOOM PROGRESS (0 - MIN ZOOM, 1 - MAX ZOOM)

    [SerializeField] float rotationSpeed = 180f;

    LevelGenerator levelGenerator;
    GameManager gameManager;
    GameScreen gameScreen;

    public static CameraController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        levelGenerator = LevelGenerator.Instance;
        gameManager = GameManager.Instance;
        gameScreen = GameScreen.Instance;

        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    void Update()
    {
        if (!gameScreen.PlayerIsOn)
            HandleInput();
    }

    public void RefreshToCurrentRoom()
    {
        Vector3 currentRoomPos = gameManager.CurrentRoom.transform.localPosition;
        transform.localPosition = new Vector3(currentRoomPos.x, transform.localPosition.y,
                                              currentRoomPos.z);
        AdjustPosition(transform.localPosition);
        AdjustZoom(zoom);
    }

    void HandleInput()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"),
                                        Input.GetAxis("Vertical"));
        if (moveInput != Vector2.zero) // IF THERE IS MOVE INPUT
            AdjustPosition(moveInput); // ADJUST CAM TO NEW POS

        float zoomInput = Input.GetAxis("Mouse ScrollWheel");
        if (zoomInput != 0f) // IF THERE IS ZOOM INPUT
            AdjustZoom(zoomInput); // ADJUST CAM RIG TO NEW ZOOM

        float rotateInput = Input.GetAxis("Rotation");
        if (rotateInput != 0f) // IF THERE IS ROTATE INPUT
            AdjustRotation(rotateInput); // ADJUST CAM TO NEW ROTATION
    } 

    void AdjustPosition(Vector2 moveDelta)
    {
        Vector3 dir = transform.localRotation // MOVEMENT RELATIVE TO ROTATION
                    * new Vector3(moveDelta.x, 0f, moveDelta.y).normalized; // NORMALISE TO CIRCULAR DIR
        float moveSpeed = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom);
        // APPLY MOST EXTREME AXIS VALUE TO APPLY SMOOTH MOVEMENT AND GET RID OF START/STOP MOVEMENT DELAYS
        float damping = Mathf.Max(Mathf.Abs(moveDelta.x), Mathf.Abs(moveDelta.y));
        float dst = moveSpeed * damping * Time.deltaTime;

        Vector3 pos = transform.localPosition;
        pos += dir * dst;

        /*transform.localPosition = ClampPosition(pos);*/ // RESTRICT MOVEMENT TO MAP BOUNDS
        transform.localPosition = pos;
    }

    Vector3 ClampPosition (Vector3 pos)
    {
        Room currentRoom = levelGenerator.Rooms[gameManager.CurrentRoomIndex];
        float roomApothemX = currentRoom.GetSizeX() / 2f;
        float roomApothemZ = currentRoom.GetSizeZ() / 2f;
        Vector3 currentRoomPos = currentRoom.transform.localPosition;
        // CLAMP X | SUBTRACT HALF A CELL TO CENTRE TO THE RIGHTMOST CELL AS WELL
        float xMin = currentRoomPos.x - roomApothemX;
        float xMax = currentRoomPos.x + roomApothemX;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        // CLAMP Z | SUBTRACT A WHOLE CELL TO CENTRE TO THE TOPMOST CELL AS WELL
        float zMin = currentRoomPos.z - roomApothemZ;
        float zMax = currentRoomPos.z + roomApothemZ;
        pos.z = Mathf.Clamp(pos.z, zMin, zMax);

        return pos;
    }

    void AdjustZoom(float zoomDelta) // MATCH ZOOM LEVEL
    {
        zoom = Mathf.Clamp01(zoom + zoomDelta); // KEEP ZOOM WITHIN 0-1 RANGE

        // STICK
        float dst = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom); // USE ZOOM AS PROGRESS
        stick.localPosition = Vector3.forward * dst;

        // SWIVEL
        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    float rotationAngle;
    void AdjustRotation(float rotateDelta)
    {
        rotationAngle += rotateDelta * rotationSpeed * Time.deltaTime;
        // WRAP ANGLE
        if (rotationAngle < 0f)
            rotationAngle += 360f;
        else if (rotationAngle >= 360f)
            rotationAngle -= 360f;

        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
}