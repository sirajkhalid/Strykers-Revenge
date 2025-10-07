using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class CharacterData
{
    // Basic identity
    public string characterId;          // GUID for uniqueness
    public string characterName;

    // Core choices
    public Race race;
    public ClassType classType;
    public BackgroundType background;

    // Build
    public Abilities abilities;               // STR/DEX/CON/INT/WIS/CHA
    public List<Skill> selectedSkills = new(); // chosen proficiencies

    // Meta
    public string createdAtUtc;
    public string lastUpdatedUtc;
    public int saveVersion = 1;

    public CharacterData()
    {
        characterId = Guid.NewGuid().ToString();
        createdAtUtc = DateTime.UtcNow.ToString("o");
        lastUpdatedUtc = createdAtUtc;
        // defaults
        characterName = "Unnamed";
        race = Race.Human;
        classType = ClassType.Fighter;
        background = BackgroundType.Outlander;
        abilities = new Abilities { strength = 10, dexterity = 10, constitution = 10, intelligence = 10, wisdom = 10, charisma = 10 };
    }

    public void Touch() => lastUpdatedUtc = DateTime.UtcNow.ToString("o");
}
