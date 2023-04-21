using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public int currentLevel = 1;
    public int CurrentLevel { get { return currentLevel; }
                                set { currentLevel = value; } }
    public int bestLevel = int.MaxValue;
    public int BestLevel { get { return bestLevel; }
                             set { bestLevel = value; } }
    public bool bestLevelIsLocked = true;
    public bool BestLevelIsLocked { get { return bestLevelIsLocked; }
                                      set { bestLevelIsLocked = value; } }

    public static PersistentData Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void RefreshBestAttempt()
    {
        if (!BestLevelIsLocked)
            if (currentLevel > bestLevel)
                SaveBestAttempt(bestLevel = currentLevel);
    }

    public int LoadBestAttempt()
    {
        return PlayerPrefs.GetInt("BestLevel", int.MaxValue);
    }

    public void SaveBestAttempt(int value)
    {
        PlayerPrefs.SetInt("BestLevel", value);
    }

    public int LoadBestAttemptIsLocked()
    {
        return PlayerPrefs.GetInt("BestLevelIsLocked", 1);
    }

    public void SaveBestAttemptIsLocked(int value)
    {
        PlayerPrefs.SetInt("BestLevelIsLocked", value);
    }
}