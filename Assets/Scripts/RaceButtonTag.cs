using UnityEngine;
using UnityEngine.UI;

public enum Race { Human, Elf, Dwarf, HellSpawn, DragonHybrid }

[RequireComponent(typeof(Button))]
public class RaceButtonTag : MonoBehaviour
{
    public Race race;
    [HideInInspector] public Button button;
    [HideInInspector] public Image img;

    void Awake() { button = GetComponent<Button>(); img = GetComponent<Image>(); }
}
