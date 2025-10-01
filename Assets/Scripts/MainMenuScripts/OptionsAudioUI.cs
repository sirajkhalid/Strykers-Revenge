using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsAudioUI : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] string musicParam = "MusicVolume"; 
    [SerializeField] string sfxParam = "SFXVolume";   

    [Header("UI Sliders")]
    [SerializeField] Slider musicSlider; //  MusicSlider
    [SerializeField] Slider sfxSlider;   //  SFXSlider

    const string PP_MUSIC = "vol_music";
    const string PP_SFX = "vol_sfx";

    void Awake()
    {
        if (musicSlider)
        {
            float v = PlayerPrefs.GetFloat(PP_MUSIC, 0.8f);
            musicSlider.SetValueWithoutNotify(v);
            SetMusic(v);
            musicSlider.onValueChanged.AddListener(SetMusic);
        }

        if (sfxSlider)
        {
            float v = PlayerPrefs.GetFloat(PP_SFX, 0.8f);
            sfxSlider.SetValueWithoutNotify(v);
            SetSFX(v);
            sfxSlider.onValueChanged.AddListener(SetSFX);
        }
    }

    static float ToDecibels(float linear)
    {
        linear = Mathf.Clamp(linear, 0.0001f, 1f);
        return Mathf.Log10(linear) * 20f;  // 0..1 → -80..0 dB
    }

    public void SetMusic(float v)
    {
        if (mixer) mixer.SetFloat(musicParam, ToDecibels(v));
        PlayerPrefs.SetFloat(PP_MUSIC, v);
    }

    public void SetSFX(float v)
    {
        if (mixer) mixer.SetFloat(sfxParam, ToDecibels(v));
        PlayerPrefs.SetFloat(PP_SFX, v);
    }
}
