using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    // Active character being built/played
    public CharacterData current = new CharacterData();

    // Where we save (one slot for now; expand later if you want multiple characters)
    public const string SaveFileName = "character.json";

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Try auto-load (optional)
        var loaded = SaveSystem.Load(SaveFileName);
        if (loaded != null) current = loaded;
    }

    // Convenience setters (use these from your UI)
    public void SetName(string name) { current.characterName = name; current.Touch(); }
    public void SetRace(Race race) { current.race = race; current.Touch(); }
    public void SetClass(ClassType c) { current.classType = c; current.Touch(); }
    public void SetBackground(BackgroundType b) { current.background = b; current.Touch(); }
    public void SetAbilities(Abilities a) { current.abilities = a; current.Touch(); }
    public void SetSkills(System.Collections.Generic.List<Skill> s) { current.selectedSkills = s; current.Touch(); }

    public void Save() => SaveSystem.Save(SaveFileName, current);
    public void ResetCharacter() => current = new CharacterData();
}
