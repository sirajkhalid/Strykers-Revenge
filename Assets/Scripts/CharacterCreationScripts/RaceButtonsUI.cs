using UnityEngine;
using UnityEngine.UI;

public class RaceButtonsUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject racePanel;    // assign your RacePanel (fallback = this GO)
    [SerializeField] GameObject classPanel;   // assign your ClassPanel (starts INACTIVE)

    [Header("Confirm")]
    [SerializeField] Button confirmButton;    // make sure this exists in scene
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = new Color(0.90f, 0.85f, 0.70f, 1f);

    public Race SelectedRace { get; private set; }

    RaceButtonTag[] tags;

    void Awake()
    {
        if (!racePanel) racePanel = gameObject;
        if (classPanel) classPanel.SetActive(false);

        // Find confirm if not assigned
        if (!confirmButton)
        {
            var go = GameObject.Find("ConfirmButton");
            if (go) confirmButton = go.GetComponent<Button>();
        }

        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }
        else
        {
            Debug.LogWarning("RaceButtonsUI: Confirm Button not assigned and not found in scene.");
        }

        // Wire all race buttons by tag
        tags = GetComponentsInChildren<RaceButtonTag>(true);
        if (tags == null || tags.Length == 0)
        {
            Debug.LogWarning("RaceButtonsUI: No RaceButtonTag found under this object. Add RaceButtonTag to each race button.");
        }
        else
        {
            foreach (var t in tags)
            {
                var local = t; // capture
                if (local.button) local.button.onClick.AddListener(() => OnPick(local));
                var img = local.GetComponent<Image>();
                if (img) img.color = normalColor;
            }
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

        // remember selection
        PlayerPrefs.SetInt("player_race", (int)SelectedRace);
        PlayerPrefs.SetString("player_race_name", SelectedRace.ToString());
        PlayerPrefs.Save();
        if (GameState.Instance) GameState.Instance.playerRace = SelectedRace;

        ShowConfirm();
    }

    void ShowConfirm()
    {
        if (!confirmButton) return;

        confirmButton.gameObject.SetActive(true);

        // Make absolutely sure it's visible and interactive
        var cg = confirmButton.GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        // If confirm lives under a parent CanvasGroup that was disabled somewhere,
        // ensure that parent allows interaction:
        var parentCg = confirmButton.GetComponentInParent<CanvasGroup>();
        if (parentCg)
        {
            parentCg.alpha = Mathf.Max(parentCg.alpha, 1f);
            parentCg.blocksRaycasts = true;
        }
    }

    void Confirm()
    {
        if (classPanel) classPanel.SetActive(true);
        if (racePanel) racePanel.SetActive(false);
    }
}
