using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;
    public string playerName = "";
    public Race playerRace = Race.Human;
    public ClassType playerClass = ClassType.Fighter;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
