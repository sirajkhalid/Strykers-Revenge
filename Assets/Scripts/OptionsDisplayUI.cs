using UnityEngine;
using UnityEngine.UI;

public class OptionsDisplayUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Toggle fullscreenToggle;

    [Header("Windowed fallback size")]
    [SerializeField] int windowedWidth = 1600;
    [SerializeField] int windowedHeight = 900;

    const string PP_FULLSCREEN = "video_fullscreen";

    void Awake()
    {
        // load saved state (default ON)
        bool isFull = PlayerPrefs.GetInt(PP_FULLSCREEN, 1) == 1;

        if (fullscreenToggle)
        {
            fullscreenToggle.SetIsOnWithoutNotify(isFull);
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        // apply on startup
        SetFullscreen(isFull);
    }

    public void SetFullscreen(bool on)
    {
        if (on)
        {
            // Borderless fullscreen 
            var res = Screen.currentResolution;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);
            PlayerPrefs.SetInt(PP_FULLSCREEN, 1);
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(windowedWidth, windowedHeight, FullScreenMode.Windowed);
            PlayerPrefs.SetInt(PP_FULLSCREEN, 0);
        }
    }
}
