using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]


public class ClassButtonTag : MonoBehaviour
{
    public ClassType classType;
    [HideInInspector] public Button button;
    [HideInInspector] public Image img;

    void Awake()
    {
        button = GetComponent<Button>();
        img = GetComponent<Image>();
    }
}
