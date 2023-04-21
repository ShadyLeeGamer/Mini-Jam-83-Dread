using UnityEngine;

public class Hallway : MonoBehaviour
{
    public PassType Type { get; set; }
    public Vector3Int Dir { get; set; }

    public void Initialise(Vector3Int dir, PassType type)
    {
        Dir = dir;

        // RESCALE BASED ON SIZE
        float hallwaySizeX = transform.localScale.x;
        float hallwaySizeZ = transform.localScale.z;
        float scaleMultiplierX = Dir.x == 0 ? 2 : 1;
        float scaleMultiplierZ = Dir.z == 0 ? 2 : 1;
        transform.GetChild(0).localScale = new Vector3(hallwaySizeX * scaleMultiplierX, 1f,
                                                       hallwaySizeZ * scaleMultiplierZ);

        ApplyType(type);
    }

    public void ApplyType(PassType type)
    {
        Type = type;
        gameObject.name = type + " Hallway";
    }
}

public enum PassType
{
    Entry,
    Exit
}