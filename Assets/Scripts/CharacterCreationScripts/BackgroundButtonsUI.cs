using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BackgroundButtonsUI : MonoBehaviour
{
    [Header("panels")]
    [SerializeField] GameObject backgroundPanel;     // this panel
    [SerializeField] GameObject abilitiesPanel;      // next panel (starts inactive)

    [Header("ui")]
    [SerializeField] Button confirmButton;           // starts inactive
    [SerializeField] TMP_Text descriptionText;       // text on the right
    [SerializeField] GameObject descriptionBox;      // parent of the text on the right (CanvasGroup faded)
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = new Color(0.90f, 0.85f, 0.70f, 1f);

    [Header("fade")]
    [SerializeField, Min(0f)] float fadeDuration = 0.25f;

    public BackgroundType SelectedBackground { get; private set; }

    BackgroundButtonTag[] tags;
    CanvasGroup descCg;

    void Awake()
    {
        if (!backgroundPanel) backgroundPanel = gameObject;
        if (abilitiesPanel) abilitiesPanel.SetActive(false);

        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }

        if (descriptionBox)
        {
            descCg = descriptionBox.GetComponent<CanvasGroup>();
            if (!descCg) descCg = descriptionBox.AddComponent<CanvasGroup>();
            descriptionBox.SetActive(false);
            descCg.alpha = 0f;
            descCg.interactable = false;
            descCg.blocksRaycasts = false;
        }

        tags = GetComponentsInChildren<BackgroundButtonTag>(true);
        foreach (var t in tags)
        {
            var local = t; // capture
            if (local.button) local.button.onClick.AddListener(() => OnPick(local));
            if (local.img) local.img.color = normalColor;
        }
    }

    void OnPick(BackgroundButtonTag picked)
    {
        SelectedBackground = picked.background;

        // highlight
        foreach (var t in tags)
            if (t.img) t.img.color = (t == picked) ? selectedColor : normalColor;

        // build description text (simple header + proficiencies)
        var profs = GetProficiencies(SelectedBackground);
        if (descriptionText)
        {
            descriptionText.text =
                $"<size=130%><b>{SelectedBackground}</b></size>\n\n" +
                $"{GetShortFlavour(SelectedBackground)}\n\n" +
                $"<b>Proficiencies (+1):</b> {string.Join(", ", profs)}";
        }

        // fade in the box
        if (descCg)
        {
            descriptionBox.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Fade(descCg, 0f, 1f, fadeDuration, () =>
            {
                descCg.interactable = true;
                descCg.blocksRaycasts = true;
            }));
        }

        if (confirmButton) confirmButton.gameObject.SetActive(true);
    }

    void Confirm()
    {
        // save choice and grant proficiencies
        if (GameState.Instance)
        {
            GameState.Instance.SetBackground(SelectedBackground);

            var list = GameState.Instance.current.selectedSkills;
            var toGrant = GetProficiencies(SelectedBackground);
            foreach (var s in toGrant)
                if (!list.Contains(s)) list.Add(s); // background grants +1 by inclusion

            GameState.Instance.current.Touch();
            GameState.Instance.Save();
        }

        // swap panels
        if (abilitiesPanel) abilitiesPanel.SetActive(true);
        if (backgroundPanel) backgroundPanel.SetActive(false);
    }

    // data --------------------------------------------------------------------

    List<Skill> GetProficiencies(BackgroundType bg)
    {
        // BG3 mapping
        switch (bg)
        {
            case BackgroundType.Acolyte: return new List<Skill> { Skill.Insight, Skill.Religion };
            case BackgroundType.Charlatan: return new List<Skill> { Skill.Deception, Skill.SleightOfHand };
            case BackgroundType.Criminal: return new List<Skill> { Skill.Deception, Skill.Stealth };
            case BackgroundType.Entertainer: return new List<Skill> { Skill.Acrobatics, Skill.Performance };
            case BackgroundType.FolkHero: return new List<Skill> { Skill.AnimalHandling, Skill.Survival };
            case BackgroundType.GuildArtisan: return new List<Skill> { Skill.Insight, Skill.Persuasion };
            case BackgroundType.Noble: return new List<Skill> { Skill.History, Skill.Persuasion };
            case BackgroundType.Outlander: return new List<Skill> { Skill.Athletics, Skill.Survival };
            case BackgroundType.Sage: return new List<Skill> { Skill.Arcana, Skill.History };
            case BackgroundType.Soldier: return new List<Skill> { Skill.Athletics, Skill.Intimidation };
            case BackgroundType.Urchin: return new List<Skill> { Skill.SleightOfHand, Skill.Stealth };
            default: return new List<Skill>();
        }
    }

    string GetShortFlavour(BackgroundType bg)
    {
        // brief descriptions (placeholder text)
        switch (bg)
        {
            case BackgroundType.Acolyte: return "A life devoted to service and faith.";
            case BackgroundType.Charlatan: return "A silver tongue and a sleight of hand.";
            case BackgroundType.Criminal: return "Underworld savvy and a knack for stealth.";
            case BackgroundType.Entertainer: return "Performer of stage, song, and spectacle.";
            case BackgroundType.FolkHero: return "Champion of common folk and rustic life.";
            case BackgroundType.GuildArtisan: return "Skilled craftsperson with merchant ties.";
            case BackgroundType.Noble: return "Born to status and trained in courtly ways.";
            case BackgroundType.Outlander: return "A life lived in the wilds and on the road.";
            case BackgroundType.Sage: return "Seeker of knowledge and keeper of lore.";
            case BackgroundType.Soldier: return "Disciplined veteran of many battles.";
            case BackgroundType.Urchin: return "Street-tough survivor with nimble fingers.";
            default: return "";
        }
    }

    // fade helpers ------------------------------------------------------------

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration, System.Action after = null)
    {
        float t = 0f;
        cg.alpha = from;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / Mathf.Max(0.0001f, duration));
            yield return null;
        }
        cg.alpha = to;
        after?.Invoke();
    }
}
