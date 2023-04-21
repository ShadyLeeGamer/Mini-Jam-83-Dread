using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] Material unlockedMat, lockedMat;
    Material mat;
    public PassType Type { get; set; }

    public Vector3Int Dir { get; set; }

    public bool IsLocked;

    public void Initialise(Vector3Int dir, PassType type)
    {
        Type = type;
        Dir = dir;

        // CORRECT ROTATION
        int rotationMultiplier = 0;
        bool typeIsExit = type == PassType.Exit;
        if (Dir.x == -1) // LEFT
            rotationMultiplier = typeIsExit ? 1 : 3;
        if (Dir.x == 1) // RIGHT
            rotationMultiplier = typeIsExit ? 3 : 1;

        if (Dir.z == -1) // DOWN
            rotationMultiplier = typeIsExit ? 0 : 2;
        transform.eulerAngles = new Vector3(0f, 90f * rotationMultiplier, 0f);

        ApplyType(type);

        mat = transform.GetChild(0).GetComponent<Renderer>().material;

        SetLock(false);
    }

    public void ApplyType(PassType type)
    {
        Type = type;
        gameObject.name = type + " Door";
    }

    public void SetLock(bool isLocked)
    {
        IsLocked = isLocked;
        mat = isLocked ? lockedMat : unlockedMat;
    }
}