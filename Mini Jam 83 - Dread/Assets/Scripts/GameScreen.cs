using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentLevel;
    [SerializeField] Button switchBTN;
    [SerializeField] TextMeshProUGUI switchTXT;

    public static GameScreen Instance;

    public bool PlayerIsOn { get; set; }

    void Awake()
    {
        Instance = this;
    }

    public void Switch()
    {
        PlayerIsOn = !PlayerIsOn;
        switchTXT.text = "Switch to " + (PlayerIsOn ? "Cam" : "Player");
    }
}