using UnityEngine;

public class KeySpot : MonoBehaviour
{
    public bool StoringKey { get; set; }

    public void HideKey()
    {
        StoringKey = true;

        name = "True KeySpot";
    }

    public void Inspect()
    {
        if (StoringKey)
        {
            // PLAYER HAS KEY = TRUE
        }
    }
}