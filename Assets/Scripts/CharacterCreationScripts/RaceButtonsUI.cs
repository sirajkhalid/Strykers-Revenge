using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceButtonsUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject racePanel;
    [SerializeField] GameObject classPanel;

    [Header("UI")]
    [SerializeField] Button confirmButton;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = new Color(0.90f, 0.85f, 0.70f, 1f);

    [Header("Description Boxes (one per race)")]
    [SerializeField] TMP_Text humanDescription;
    [SerializeField] TMP_Text elfDescription;
    [SerializeField] TMP_Text dwarfDescription;
    [SerializeField] TMP_Text hellspawnDescription;
    [SerializeField] TMP_Text dragonHybridDescription;

    public Race SelectedRace { get; private set; }

    RaceButtonTag[] tags;
    Dictionary<Race, TMP_Text> raceDescMap;

    void Awake()
    {
        if (!racePanel) racePanel = gameObject;
        if (classPanel) classPanel.SetActive(false);

        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }

        // Map races to their description boxes
        raceDescMap = new Dictionary<Race, TMP_Text>
        {
            { Race.Human, humanDescription },
            { Race.Elf, elfDescription },
            { Race.Dwarf, dwarfDescription },
            { Race.HellSpawn, hellspawnDescription },
            { Race.DragonHybrid, dragonHybridDescription }
        };

        tags = GetComponentsInChildren<RaceButtonTag>(true);
        foreach (var t in tags)
        {
            var local = t;
            if (local.button) local.button.onClick.AddListener(() => OnPick(local));
            var img = local.GetComponent<Image>();
            if (img) img.color = normalColor;
        }

        // hide all description boxes at start
        foreach (var kvp in raceDescMap)
            if (kvp.Value) kvp.Value.gameObject.SetActive(false);
    }

    void OnPick(RaceButtonTag picked)
    {
        SelectedRace = picked.race;

        // highlight
        foreach (var t in tags)
        {
            var img = t.GetComponent<Image>();
            if (img) img.color = (t == picked) ? selectedColor : normalColor;
        }

        // hide all boxes first
        foreach (var kvp in raceDescMap)
            if (kvp.Value) kvp.Value.gameObject.SetActive(false);

        // apply & show selected race bonuses
        ApplyRaceBonuses(SelectedRace);
        ShowBonusDescription(SelectedRace);

        if (confirmButton) confirmButton.gameObject.SetActive(true);
    }

    void ApplyRaceBonuses(Race race)
    {
        var cd = GameState.Instance.current;
        var a = cd.abilities;

        switch (race)
        {
            case Race.Human:
                a.strength++; a.dexterity++; a.constitution++;
                a.intelligence++; a.wisdom++; a.charisma++;
                break;
            case Race.Elf:
                a.dexterity += 2; a.intelligence++;
                break;
            case Race.Dwarf:
                a.constitution += 2; a.strength++;
                break;
            case Race.HellSpawn:
                a.charisma += 2; a.wisdom -= 1;
                break;
            case Race.DragonHybrid:
                a.strength += 2; a.constitution++;
                break;
        }
        cd.abilities = a;

        foreach (var s in GetSkillProficiencies(race))
            if (!cd.selectedSkills.Contains(s)) cd.selectedSkills.Add(s);
    }

    void ShowBonusDescription(Race race)
    {
        if (!raceDescMap.TryGetValue(race, out TMP_Text desc) || !desc) return;

        desc.gameObject.SetActive(true);
        var lines = new List<string> { $"<b>{race}</b> bonuses:" };

        switch (race)
        {
            case Race.Human:
                lines.Add("+1 to all Abilities");
                break;
            case Race.Elf:
                lines.Add("+2 Dexterity, +1 Intelligence");
                lines.Add("+1 Perception (skill)");
                break;
            case Race.Dwarf:
                lines.Add("+2 Constitution, +1 Strength");
                lines.Add("+1 History (skill)");
                break;
            case Race.HellSpawn:
                lines.Add("+2 Charisma, –1 Wisdom");
                lines.Add("+1 Intimidation (skill)");
                break;
            case Race.DragonHybrid:
                lines.Add("+2 Strength, +1 Constitution");
                lines.Add("+1 Athletics (skill)");
                break;
        }

        desc.text = string.Join("\n", lines);
    }

    static List<Skill> GetSkillProficiencies(Race race)
    {
        return race switch
        {
            Race.Elf => new List<Skill> { Skill.Perception },
            Race.Dwarf => new List<Skill> { Skill.History },
            Race.HellSpawn => new List<Skill> { Skill.Intimidation },
            Race.DragonHybrid => new List<Skill> { Skill.Athletics },
            _ => new List<Skill>()
        };
    }

    void Confirm()
    {
        if (classPanel) classPanel.SetActive(true);
        if (racePanel) racePanel.SetActive(false);
    }
}
