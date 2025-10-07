using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))] // parent image drives tint/mask; required for consistent visuals
public class RaceButtonTag : MonoBehaviour
{
    // data
    public Race race;

    // cached refs
    [HideInInspector] public Button button;  // cached Button for wiring clicks
    [HideInInspector] public Image img;     // cached parent Image for highlight/tint

    // safety
    [Tooltip("turns off raycastTarget on child Graphics so clicks reach the Button on this object")]
    [SerializeField] bool autoDisableChildRaycasts = true;

    void Awake()
    {
        CacheRefs();

        if (!button || !img)
            Debug.LogWarning($"RaceButtonTag: missing Button or Image on {name}");

        if (autoDisableChildRaycasts)
            DisableChildRaycasts();
    }

#if UNITY_EDITOR
    void OnValidate() { CacheRefs(); }
#endif

    void CacheRefs()
    {
        if (!button) button = GetComponent<Button>();
        if (!img) img = GetComponent<Image>();
    }

    void DisableChildRaycasts()
    {
        // disables raycasts on all child Graphics except the parent Image on this object
        var graphics = GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics)
        {
            if (g == img) continue;  // keep parent image as the raycast target
            g.raycastTarget = false;
        }
    }
}
