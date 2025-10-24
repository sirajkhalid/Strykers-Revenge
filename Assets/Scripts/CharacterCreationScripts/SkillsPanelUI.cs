// SkillsPanelUI.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsPanelUI : MonoBehaviour
{
    [Header("panels")]
    [SerializeField] GameObject skillsPanel;
    [SerializeField] GameObject nextPanel;  // whatever comes after

    [Header("ui")]
    [SerializeField] TMP_Text skillPointsNum;
    [SerializeField] TMP_Text descriptionBoxText;

    [SerializeField] Button randomButton;
    [SerializeField] Button confirmButton;

    [Header("skill rows")]
    [SerializeField] SkillRow[] skillRows; // each row contains labels, plus/minus buttons, value text

    [Header("rules")]
    [SerializeField, Min(0)] int totalPoints = 5;
    const int MIN_VALUE = 0;
    const int MAX_VALUE = 2;

    int remaining;
    bool jiggling;

    void Awake()
    {
        if (!skillsPanel) skillsPanel = gameObject;
        if (nextPanel) nextPanel.SetActive(false);

        remaining = totalPoints;
        foreach (var row in skillRows)
        {
            row.Initialize(this);
        }

        Wire(randomButton, OnRandomize);
        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(OnConfirm);
        }

        UpdateAllUI();
    }

    void Wire(Button b, UnityEngine.Events.UnityAction cb)
    {
        if (!b) return;
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(cb);
    }

    public void TryChange(SkillRow row, int delta)
    {
        if (jiggling) return;

        int newValue = row.Value + delta;
        if (newValue < MIN_VALUE || newValue > MAX_VALUE) return;

        int cost = delta > 0 ? 1 : 1; // simple: +1 point per +1
        if (delta > 0 && cost > remaining) return;

        row.SetValue(newValue);
        remaining -= delta > 0 ? cost : -cost;
        UpdateAllUI();
    }

    void OnRandomize()
    {
        if (jiggling) return;
        StartCoroutine(RandomizeCoroutine());
    }

    IEnumerator RandomizeCoroutine()
    {
        jiggling = true;
        foreach (var row in skillRows) row.SetValue(0);
        remaining = totalPoints;
        UpdateAllUI();

        float jiggle = 1.0f;
        float t = 0f;
        while (t < jiggle)
        {
            t += Time.unscaledDeltaTime;
            foreach (var row in skillRows)
            {
                int fake = Random.Range(MIN_VALUE, MAX_VALUE + 1);
                row.SetTempText(fake.ToString());
            }
            yield return null;
        }

        foreach (var row in skillRows) row.SetValue(0);
        remaining = totalPoints;

        int guard = 10000;
        while (remaining > 0 && guard-- > 0)
        {
            int pick = Random.Range(0, skillRows.Length);
            if (skillRows[pick].Value < MAX_VALUE)
            {
                skillRows[pick].SetValue(skillRows[pick].Value + 1);
                remaining -= 1;
            }
        }

        UpdateAllUI();
        jiggling = false;
    }

    void UpdateAllUI()
    {
        if (skillPointsNum) skillPointsNum.text = remaining.ToString();

        foreach (var row in skillRows) row.RefreshUI();

        if (confirmButton) confirmButton.gameObject.SetActive(remaining == 0);

        if (descriptionBoxText)
        {
            descriptionBoxText.text = BuildDescriptionText();
        }
    }

    string BuildDescriptionText()
    {
        // show race/class/background bonuses (example)
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("<b>Bonuses from choices:</b>");
        sb.AppendLine($"Race: {GameState.Instance.current.race}");
        sb.AppendLine($"Class: {GameState.Instance.current.classType}");
        sb.AppendLine($"Background: {GameState.Instance.current.background}");
        sb.AppendLine("\n<color=#ffaa00>Skills with +1 from those choices:</color>");
        var profs = GetBonusSkillsForCurrent();
        foreach (var s in profs) sb.AppendLine($"• {s} (+1)");
        return sb.ToString();
    }

    List<Skill> GetBonusSkillsForCurrent()
    {
        // replicate same mapping as you used earlier for background etc.
        // e.g., if background = Acolyte → Insight/Religion
        return BackgroundButtonsUI.GetProficiencies(GameState.Instance.current.background);
    }

    void OnConfirm()
    {
        if (remaining != 0) return;

        if (GameState.Instance)
        {
            GameState.Instance.current.selectedSkills.Clear();
            foreach (var row in skillRows)
            {
                if (row.Value > 0)
                {
                    GameState.Instance.current.selectedSkills.Add(row.SkillType);
                }
            }
            GameState.Instance.current.Touch();
            GameState.Instance.Save();
        }

        if (nextPanel) nextPanel.SetActive(true);
        if (skillsPanel) skillsPanel.SetActive(false);
    }
}

[System.Serializable]
public class SkillRow
{
    public Skill SkillType;           // Skill name (enum)
    public TMP_Text nameText;         // Left label (e.g., "Athletics")
    public TMP_Text valueText;        // Number display (e.g., "0")
    public Button plusButton;         // "+" button
    public Button minusButton;        // "–" button

    int value;
    SkillsPanelUI parent;             // assigned when initialized

    public int Value => value;

    public void Initialize(SkillsPanelUI parentUI)
    {
        parent = parentUI;
        value = 0;

        // Set up button listeners
        if (plusButton)
        {
            plusButton.onClick.RemoveAllListeners();
            plusButton.onClick.AddListener(() => parent.TryChange(this, +1));
        }

        if (minusButton)
        {
            minusButton.onClick.RemoveAllListeners();
            minusButton.onClick.AddListener(() => parent.TryChange(this, -1));
        }

        RefreshUI();
    }

    public void SetValue(int newVal)
    {
        value = newVal;
        RefreshUI();
    }

    public void SetTempText(string txt)
    {
        if (valueText) valueText.text = txt;
    }

    public void RefreshUI()
    {
        if (valueText) valueText.text = value.ToString();
    }
}
