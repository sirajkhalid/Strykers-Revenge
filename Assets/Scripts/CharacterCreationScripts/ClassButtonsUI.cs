using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClassButtonsUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] GameObject classPanel;   // assign ClassPanel
    [SerializeField] GameObject backgroundPanel;   // assign BackgroundPanel 

    [Header("UI")]
    [SerializeField] Button confirmButton;    // starts inactive
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color selectedColor = new Color(0.90f, 0.85f, 0.70f, 1f);

    [Header("Optional")]
    [SerializeField] TMP_Text classDesc;

    public ClassType SelectedClass { get; private set; }
    ClassButtonTag[] tags;

    void Awake()
    {
        if (!classPanel) classPanel = gameObject;
        if (backgroundPanel) backgroundPanel.SetActive(false);

        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }

        tags = GetComponentsInChildren<ClassButtonTag>(true);
        foreach (var t in tags)
        {
            var local = t; // capture
            if (local.button) local.button.onClick.AddListener(() => OnPick(local));
            if (local.img) local.img.color = normalColor;
        }
    }

    void OnPick(ClassButtonTag picked)
    {
        SelectedClass = picked.classType;

        // Highlight selection
        foreach (var t in tags)
            if (t.img) t.img.color = (t == picked) ? selectedColor : normalColor;

        // Remember in GameState + disk (JSON)
        if (GameState.Instance)
        {
            GameState.Instance.SetClass(SelectedClass); // updates GameState.current + timestamp
            GameState.Instance.Save();                  // immediate save 
            Debug.Log($"Save path: {System.IO.Path.Combine(Application.persistentDataPath, GameState.SaveFileName)}");

        }

        // (Optional legacy UI helpers)
        PlayerPrefs.SetInt("player_class", (int)SelectedClass);
        PlayerPrefs.SetString("player_class_name", SelectedClass.ToString());
        PlayerPrefs.Save();

        if (classDesc) classDesc.text = GetDesc(SelectedClass);
        if (confirmButton) confirmButton.gameObject.SetActive(true);
    }

    void Confirm()
    {
        if (backgroundPanel) backgroundPanel.SetActive(true);
        if (classPanel) classPanel.SetActive(false);
    }

    string GetDesc(ClassType c) => c switch
    {
        ClassType.Barbarian => "Rage-fueled melee bruiser.",
        ClassType.Ranger => "Skilled tracker and archer.",
        ClassType.Fighter => "Versatile weapons specialist.",
        ClassType.Wizard => "Glass cannon spellcaster.",
        ClassType.Cleric => "Healer with divine magic.",
        ClassType.Rogue => "Stealthy striker and trickster.",
        _ => ""
    };
}
