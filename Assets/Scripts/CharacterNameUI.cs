using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

public class CharacterNameUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_Text errorText;
    [SerializeField] Button confirmButton;

    [Header("Panels")]
    [SerializeField] GameObject namePanel;   // assign your NamePanel 
    [SerializeField] GameObject racePanel;   // assign your RacePanel 

    [Header("Flow (optional fallback)")]
    [SerializeField] string nextSceneName = ""; 

    [Header("Rules")]
    [SerializeField] int minLen = 1;
    [SerializeField] int maxLen = 16;

    // optional starter names if user leaves it empty
    string[] fallbackNames = { "Aerin", "Ronan", "Nyra", "Kael", "Mira", "Thorne" };
    static readonly Regex allowed = new Regex(@"^[A-Za-z0-9 '\-]+$");

    void Awake()
    {
        if (!nameInput) nameInput = GetComponentInChildren<TMP_InputField>(true);
        if (!confirmButton) confirmButton = GetComponentInChildren<Button>(true);
        if (!errorText)
        {
            var texts = GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in texts) if (t.name == "ErrorText") { errorText = t; break; }
        }

        if (!namePanel) namePanel = gameObject;
        if (racePanel) racePanel.SetActive(false);

        // Prefill from last time
        string saved = PlayerPrefs.GetString("player_name", string.Empty);
        nameInput.SetTextWithoutNotify(saved);

        nameInput.onValueChanged.AddListener(OnNameChanged);
        confirmButton.onClick.AddListener(Confirm);
        OnNameChanged(nameInput.text);

        nameInput.Select();
        nameInput.ActivateInputField();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            && confirmButton.interactable)
        {
            Confirm();
        }
    }

    void OnNameChanged(string value)
    {
        string trimmed = (value ?? "").Trim();

        if (trimmed.Length < minLen)
        { SetError($"Enter a name ({minLen}+ chars)."); confirmButton.interactable = false; return; }

        if (trimmed.Length > maxLen)
        { SetError($"Max {maxLen} characters."); confirmButton.interactable = false; return; }

        if (!allowed.IsMatch(trimmed))
        { SetError("Use letters, numbers, space, ' or -"); confirmButton.interactable = false; return; }

        ClearError();
        confirmButton.interactable = true;
    }

    void SetError(string msg) { if (errorText) errorText.text = msg; }
    void ClearError() { if (errorText) errorText.text = ""; }

    void Confirm()
    {
        string trimmed = (nameInput.text ?? "").Trim();
        if (trimmed.Length < minLen) trimmed = fallbackNames[Random.Range(0, fallbackNames.Length)];
        if (trimmed.Length > maxLen) trimmed = trimmed.Substring(0, maxLen);

        PlayerPrefs.SetString("player_name", trimmed);
        PlayerPrefs.Save();
        if (GameState.Instance) GameState.Instance.playerName = trimmed;

        // >>> New: swap to Race panel if assigned; otherwise fall back to scene load
        if (racePanel)
        {
            racePanel.SetActive(true);
            if (namePanel) namePanel.SetActive(false);
        }
        else if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
