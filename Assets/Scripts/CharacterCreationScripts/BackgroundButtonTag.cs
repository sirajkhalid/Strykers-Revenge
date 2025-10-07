using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class BackgroundButtonTag : MonoBehaviour
{
    [Header("data")]
    public BackgroundType background;

    [Header("cached refs")]
    [HideInInspector] public Button button;
    [HideInInspector] public Image img;

    [Header("safety")]
    [SerializeField] bool autoDisableChildRaycasts = true;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (!img) img = GetComponent<Image>();

        if (autoDisableChildRaycasts)
        {
            var graphics = GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                if (g == img) continue;
                g.raycastTarget = false;
            }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!button) button = GetComponent<Button>();
        if (!img) img = GetComponent<Image>();
    }
#endif
}
