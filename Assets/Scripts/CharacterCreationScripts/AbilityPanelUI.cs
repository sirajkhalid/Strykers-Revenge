// AbilitiesPanelUI.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilitiesPanelUI : MonoBehaviour
{
    [Header("panels")]
    [SerializeField] GameObject abilitiesPanel;   // this panel 
    [SerializeField] GameObject skillsPanel;      // next panel 

    [Header("ui - numbers")]
    [SerializeField] TMP_Text abilityPointsNum;   // remaining pool
    [SerializeField] TMP_Text strNum;
    [SerializeField] TMP_Text dexNum;
    [SerializeField] TMP_Text conNum;
    [SerializeField] TMP_Text intNum;
    [SerializeField] TMP_Text wisNum;
    [SerializeField] TMP_Text chaNum;

    [Header("ui - buttons")]
    [SerializeField] Button strPlus; [SerializeField] Button strMinus;
    [SerializeField] Button dexPlus; [SerializeField] Button dexMinus;
    [SerializeField] Button conPlus; [SerializeField] Button conMinus;
    [SerializeField] Button intPlus; [SerializeField] Button intMinus;
    [SerializeField] Button wisPlus; [SerializeField] Button wisMinus;
    [SerializeField] Button chaPlus; [SerializeField] Button chaMinus;
    [SerializeField] Button randomButton;
    [SerializeField] Button confirmButton;        // start INACTIVE

    [Header("preview (optional)")]
    [SerializeField] TMP_Text rightBoxDescription;

    [Header("point-buy rules")]
    [SerializeField, Min(0)] int totalPoints = 27;
    const int MIN_SCORE = 8;
    const int MAX_SCORE = 15;

    // state
    readonly int[] scores = new int[6]; // base (pre-bonus) scores
    int remaining;
    bool jiggling;

    void Awake()
    {
        if (!abilitiesPanel) abilitiesPanel = gameObject;
        if (skillsPanel) skillsPanel.SetActive(false);

        // init all bases to 8 and pool to 27
        for (int i = 0; i < 6; i++) scores[i] = MIN_SCORE;
        remaining = totalPoints;

        // wire buttons 
        Wire(strPlus, () => TryRaise(Ability.STR));
        Wire(dexPlus, () => TryRaise(Ability.DEX));
        Wire(conPlus, () => TryRaise(Ability.CON));
        Wire(intPlus, () => TryRaise(Ability.INT));
        Wire(wisPlus, () => TryRaise(Ability.WIS));
        Wire(chaPlus, () => TryRaise(Ability.CHA));

        Wire(strMinus, () => TryLower(Ability.STR));
        Wire(dexMinus, () => TryLower(Ability.DEX));
        Wire(conMinus, () => TryLower(Ability.CON));
        Wire(intMinus, () => TryLower(Ability.INT));
        Wire(wisMinus, () => TryLower(Ability.WIS));
        Wire(chaMinus, () => TryLower(Ability.CHA));

        Wire(randomButton, DoRandomize);

        if (confirmButton)
        {
            confirmButton.gameObject.SetActive(false);
            confirmButton.onClick.AddListener(Confirm);
        }

        UpdateAllUI();
    }

    void Wire(Button b, UnityEngine.Events.UnityAction cb)
    {
        if (!b) return;
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(cb);
    }

    // ---------------- point-buy math ----------------

    int CostToRaise(int current)
    {
        if (current >= MAX_SCORE) return int.MaxValue;
        // 8->13 costs 1 per step, 13->14 and 14->15 cost 2
        if (current <= 12) return 1;
        return 2;
    }

    int RefundToLower(int current)
    {
        if (current <= MIN_SCORE) return 0;
        // reverse: lowering from 15 or 14 refunds 2, otherwise 1
        if (current >= 14) return 2;
        return 1;
    }

    void TryRaise(Ability a)
    {
        if (jiggling) return;
        int i = (int)a;
        int cost = CostToRaise(scores[i]);
        if (cost <= remaining)
        {
            scores[i]++;
            remaining -= cost;
            UpdateAllUI();
        }
    }

    void TryLower(Ability a)
    {
        if (jiggling) return;
        int i = (int)a;
        int refund = RefundToLower(scores[i]);
        if (refund > 0)
        {
            scores[i]--;
            remaining += refund;
            UpdateAllUI();
        }
    }

    // ---------------- randomize with jiggle ----------------

    void DoRandomize()
    {
        if (jiggling) return;
        StartCoroutine(RandomizeCoroutine());
    }

    IEnumerator RandomizeCoroutine()
    {
        jiggling = true;

        // reset to base for clean roll
        for (int i = 0; i < 6; i++) scores[i] = MIN_SCORE;
        remaining = totalPoints;
        UpdateAllUI();

        // jiggle for effect
        float jiggleTime = 1.0f;
        float t = 0f;
        while (t < jiggleTime)
        {
            t += Time.unscaledDeltaTime;
            for (int i = 0; i < 6; i++)
            {
                int fake = Random.Range(MIN_SCORE, MAX_SCORE + 1);
                SetUIText((Ability)i, fake);
            }
            yield return null;
        }

        // build a valid random distribution that spends the pool
        for (int i = 0; i < 6; i++) scores[i] = MIN_SCORE;
        remaining = totalPoints;

        int guard = 10000;
        while (remaining > 0 && guard-- > 0)
        {
            int pick = Random.Range(0, 6);
            int cost = CostToRaise(scores[pick]);

            int tries = 12;
            while (cost > remaining && tries-- > 0)
            {
                pick = Random.Range(0, 6);
                cost = CostToRaise(scores[pick]);
            }
            if (cost > remaining) break;

            scores[pick]++;
            remaining -= cost;
        }

        UpdateAllUI();
        jiggling = false;
    }

    // ---------------- UI sync ----------------

    void UpdateAllUI()
    {
        if (abilityPointsNum) abilityPointsNum.text = remaining.ToString();

        SetUIText(Ability.STR, scores[(int)Ability.STR]);
        SetUIText(Ability.DEX, scores[(int)Ability.DEX]);
        SetUIText(Ability.CON, scores[(int)Ability.CON]);
        SetUIText(Ability.INT, scores[(int)Ability.INT]);
        SetUIText(Ability.WIS, scores[(int)Ability.WIS]);
        SetUIText(Ability.CHA, scores[(int)Ability.CHA]);

        if (confirmButton) confirmButton.gameObject.SetActive(remaining == 0);

        if (rightBoxDescription) rightBoxDescription.text = BuildPreviewText();
    }

    void SetUIText(Ability a, int value)
    {
        TMP_Text t = a switch
        {
            Ability.STR => strNum,
            Ability.DEX => dexNum,
            Ability.CON => conNum,
            Ability.INT => intNum,
            Ability.WIS => wisNum,
            Ability.CHA => chaNum,
            _ => null
        };
        if (t) t.text = value.ToString();
    }

    string BuildPreviewText()
    {
        // hook for racial/class/background bonuses later
        var bonus = GetAbilityBonusesFromChoices();

        return
            $"<b>Current Allocation</b> (pre-bonus)\n" +
            $"STR {scores[(int)Ability.STR]}  (+{bonus.GetValueOrDefault(Ability.STR, 0)})\n" +
            $"DEX {scores[(int)Ability.DEX]}  (+{bonus.GetValueOrDefault(Ability.DEX, 0)})\n" +
            $"CON {scores[(int)Ability.CON]}  (+{bonus.GetValueOrDefault(Ability.CON, 0)})\n" +
            $"INT {scores[(int)Ability.INT]}  (+{bonus.GetValueOrDefault(Ability.INT, 0)})\n" +
            $"WIS {scores[(int)Ability.WIS]}  (+{bonus.GetValueOrDefault(Ability.WIS, 0)})\n" +
            $"CHA {scores[(int)Ability.CHA]}  (+{bonus.GetValueOrDefault(Ability.CHA, 0)})";
    }

    Dictionary<Ability, int> GetAbilityBonusesFromChoices()
    {
        // placeholder: read GameState.Instance.current.race/classType/background
        // and return per-ability modifiers when you add those rules.
        var d = new Dictionary<Ability, int>
        {
            [Ability.STR] = 0,
            [Ability.DEX] = 0,
            [Ability.CON] = 0,
            [Ability.INT] = 0,
            [Ability.WIS] = 0,
            [Ability.CHA] = 0
        };
        return d;
    }

    // ---------------- confirm ----------------

    void Confirm()
    {
        if (remaining != 0) return;

        if (GameState.Instance)
        {
            // write base scores into CharacterData
            var cd = GameState.Instance.current;
            var a = cd.abilities;
            a.strength = scores[(int)Ability.STR];
            a.dexterity = scores[(int)Ability.DEX];
            a.constitution = scores[(int)Ability.CON];
            a.intelligence = scores[(int)Ability.INT];
            a.wisdom = scores[(int)Ability.WIS];
            a.charisma = scores[(int)Ability.CHA];
            cd.abilities = a;
            cd.Touch();
            GameState.Instance.Save();
        }

        if (skillsPanel) skillsPanel.SetActive(true);
        if (abilitiesPanel) abilitiesPanel.SetActive(false);
    }
}
