using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterConfig
{
    public List<AbilityConfiguration> abilityConfigurations;
    [System.Serializable]
    public class AbilityConfiguration
    {
        public string buttonName;        
    }
}
