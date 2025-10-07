using UnityEngine;
using UnityEngine.UI;

public class RaceButtonsUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject racePanel;    // assign RacePanel
    [SerializeField] GameObject classPanel;   // assign ClassPanel 

    [Header("Confirm")]
    [SerializeField] Button confirmButton;    
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
                if (local.img) local.img.color = normalColor;
            }
        }
    }

    void OnPick(RaceButtonTag picked)
    {
        SelectedRace = picked.race;

        // highlight selection
        foreach (var t in tags)
            if (t.img) t.img.color = (t == picked) ? selectedColor : normalColor;

        // remember selection — runtime + disk
        if (GameState.Instance)
        {
            GameState.Instance.SetRace(SelectedRace); // updates GameState.current + timestamp
            GameState.Instance.Save();                // immediate save
            Debug.Log($"Save path: {System.IO.Path.Combine(Application.persistentDataPath, GameState.SaveFileName)}");

        }

        // (Optional legacy UI helpers)
        PlayerPrefs.SetInt("player_race", (int)SelectedRace);
        PlayerPrefs.SetString("player_race_name", SelectedRace.ToString());
        PlayerPrefs.Save();

        ShowConfirm();
    }

    void ShowConfirm()
    {
        if (!confirmButton) return;

        confirmButton.gameObject.SetActive(true);

        
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
