using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBasic
{
    public string DisplayName { get; private set; }
    public string Region { get; private set; }
    public int Level { get; private set; }

    public CharacterBasic(string displayName, string region, int level)
    {
        DisplayName = displayName;
        Region = region;
        Level = level;
    }
}
