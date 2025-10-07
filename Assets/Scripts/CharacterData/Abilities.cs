using System;
using UnityEngine;

[Serializable]
public struct Abilities
{
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int wisdom;
    public int charisma;

    // handy indexer
    public int this[Ability a]
    {
        get => a switch
        {
            Ability.STR => strength,
            Ability.DEX => dexterity,
            Ability.CON => constitution,
            Ability.INT => intelligence,
            Ability.WIS => wisdom,
            Ability.CHA => charisma,
            _ => 0
        };
        set
        {
            switch (a)
            {
                case Ability.STR: strength = value; break;
                case Ability.DEX: dexterity = value; break;
                case Ability.CON: constitution = value; break;
                case Ability.INT: intelligence = value; break;
                case Ability.WIS: wisdom = value; break;
                case Ability.CHA: charisma = value; break;
            }
        }
    }
}
