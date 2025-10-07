using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("panels")]
    [SerializeField] GameObject panel_NoSave;     // New, Options, Quit
    [SerializeField] GameObject panel_WithSave;   // New, Continue, Options, Quit
    [SerializeField] GameObject optionsPanel;     // full-screen options UI

    [Header("buttons (no-save panel)")]
    [SerializeField] Button newGame_NoSave;
    [SerializeField] Button options_NoSave;
    [SerializeField] Button quit_NoSave;

    [Header("buttons (with-save panel)")]
    [SerializeField] Button newGame_WithSave;
    [SerializeField] Button continue_WithSave;
    [SerializeField] Button options_WithSave;
    [SerializeField] Button quit_WithSave;

    [Header("buttons (options panel)")]
    [SerializeField] Button optionsClose;

    [Header("scenes")]
    [SerializeField] string introScene = "Intro";
    [SerializeField] string gameScene = "Game";

    [Header("fade")]
    [SerializeField, Min(0f)] float fadeDuration = 0.35f; // seconds

    bool hasSave;

    void Awake()
    {
        hasSave = SaveSystem.Exists(GameState.SaveFileName);

        // ensure initial states
        if (panel_NoSave) panel_NoSave.SetActive(!hasSave);
        if (panel_WithSave) panel_WithSave.SetActive(hasSave);
        if (optionsPanel) optionsPanel.SetActive(false);

        // wire buttons (no Inspector OnClick needed)
        Wire(newGame_NoSave, OnNewGame);
        Wire(options_NoSave, OpenOptions);
        Wire(quit_NoSave, OnQuit);

        Wire(newGame_WithSave, OnNewGame);
        Wire(continue_WithSave, OnContinue);
        Wire(options_WithSave, OpenOptions);
        Wire(quit_WithSave, OnQuit);

        Wire(optionsClose, CloseOptions);

        // fade in whichever main panel is active
        if (panel_WithSave && panel_WithSave.activeSelf) StartFadeIn(panel_WithSave);
        else if (panel_NoSave && panel_NoSave.activeSelf) StartFadeIn(panel_NoSave);
    }

    void Wire(Button b, UnityEngine.Events.UnityAction cb)
    {
        if (!b) return;
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(cb);
    }

    public void OnNewGame()
    {
        if (GameState.Instance)
        {
            GameState.Instance.ResetCharacter();
            GameState.Instance.Save();
        }
        SceneManager.LoadScene(introScene);
    }

    public void OnContinue()
    {
        if (GameState.Instance)
        {
            var data = SaveSystem.Load(GameState.SaveFileName);
            if (data != null) GameState.Instance.current = data;
        }
        SceneManager.LoadScene(gameScene);
    }

    public void OpenOptions()
    {
        if (!optionsPanel) return;

        // hide both menus immediately and fade in options
        if (panel_NoSave) panel_NoSave.SetActive(false);
        if (panel_WithSave) panel_WithSave.SetActive(false);

        optionsPanel.SetActive(true);
        StartFadeIn(optionsPanel);
    }

    public void CloseOptions()
    {
        if (!optionsPanel) return;

        // fade out options then restore the correct menu with a fade in
        StartCoroutine(FadeOutThen(optionsPanel, () =>
        {
            optionsPanel.SetActive(false);

            if (hasSave && panel_WithSave)
            {
                panel_WithSave.SetActive(true);
                StartFadeIn(panel_WithSave);
            }
            else if (panel_NoSave)
            {
                panel_NoSave.SetActive(true);
                StartFadeIn(panel_NoSave);
            }
        }));
    }

    public void OnQuit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    // fade helpers ------------------------------------------------------------

    void StartFadeIn(GameObject go)
    {
        var cg = EnsureCanvasGroup(go);
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        StartCoroutine(Fade(cg, 0f, 1f, fadeDuration, () =>
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }));
    }

    IEnumerator FadeOutThen(GameObject go, System.Action after)
    {
        var cg = EnsureCanvasGroup(go);
        cg.interactable = false;
        cg.blocksRaycasts = false;
        yield return Fade(cg, cg.alpha, 0f, fadeDuration);
        after?.Invoke();
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration, System.Action after = null)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // UI fades independent of timescale
            float a = Mathf.Lerp(from, to, t / Mathf.Max(0.0001f, duration));
            cg.alpha = a;
            yield return null;
        }
        cg.alpha = to;
        after?.Invoke();
    }

    CanvasGroup EnsureCanvasGroup(GameObject go)
    {
        var cg = go.GetComponent<CanvasGroup>();
        if (!cg) cg = go.AddComponent<CanvasGroup>();
        return cg;
    }
}
