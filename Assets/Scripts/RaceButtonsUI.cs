using UnityEngine;
using UnityEngine.UI;

public class RaceButtonsUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject racePanel;   // assign your RacePanel
    [SerializeField] GameObject classPanel;  // assign your ClassPanel (set INACTIVE in scene)

    [Header("UI")]
    [SerializeField] Button confirmButton;   // starts inactive
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = new Color(0.90f, 0.85f, 0.70f, 1f);

    public Race SelectedRace { get; private set; }

    RaceButtonTag[] tags;

    void Awake()
    {
        if (!racePanel) racePanel = transform.parent?.gameObject; // fallback if ButtonsBox is under RacePanel
        if (classPanel) classPanel.SetActive(false);

        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }

        tags = GetComponentsInChildren<RaceButtonTag>(true);
        foreach (var t in tags)
        {
            var local = t; // capture
            local.button.onClick.AddListener(() => OnPick(local));
            var img = local.GetComponent<Image>();
            if (img) img.color = normalColor;
        }
    }

    void OnPick(RaceButtonTag picked)
    {
        SelectedRace = picked.race;

        // highlight selection
        foreach (var t in tags)
        {
            var img = t.GetComponent<Image>();
            if (img) img.color = (t == picked) ? selectedColor : normalColor;
        }

        // remember selection immediately
        PlayerPrefs.SetInt("player_race", (int)SelectedRace);
        PlayerPrefs.SetString("player_race_name", SelectedRace.ToString());
        PlayerPrefs.Save();

        
        if (GameState.Instance) GameState.Instance.playerRace = SelectedRace;

        if (confirmButton) confirmButton.gameObject.SetActive(true);
    }

    void Confirm()
    {
        // swap panels
        if (classPanel) classPanel.SetActive(true);
        if (racePanel) racePanel.SetActive(false);
    }
}
