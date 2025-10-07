using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))] // parent Image drives tint + Mask (rounded)
public class ClassButtonTag : MonoBehaviour
{
    public ClassType classType;

    [HideInInspector] public Button button;
    [HideInInspector] public Image img;

    [Header("Safety")]
    [Tooltip("Turn OFF raycastTarget on all child Graphics so clicks reach the Button.")]
    [SerializeField] bool autoDisableChildRaycasts = true;

    void Awake()
    {
        CacheRefs();
        if (!button || !img)
        {
            Debug.LogWarning($"ClassButtonTag: Missing Button or Image on {name}. Button highlighting/tint may not work.");
        }

        if (autoDisableChildRaycasts)
            DisableChildRaycasts();
    }

#if UNITY_EDITOR
    // Keep references in sync while editing
    void OnValidate()
    {
        CacheRefs();
    }
#endif

    void CacheRefs()
    {
        if (!button) button = GetComponent<Button>();
        if (!img) img = GetComponent<Image>();
    }

    void DisableChildRaycasts()
    {
        // Disable raycasts on ALL Graphics under this button EXCEPT the parent Image.
        var graphics = GetComponentsInChildren<Graphic>(true);
        foreach (var g in graphics)
        {
            if (g == img) continue;     // keep parent Image as the raycast target
            g.raycastTarget = false;    // children won't steal clicks
        }
    }
}
