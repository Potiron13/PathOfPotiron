using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CooldownController : MonoBehaviour
{
    public List<float> timeCounter = new List<float>() { 0, 0, 0, 0 };
    public List<Image> cooldownImage = new List<Image>(4);
    public LoadHUDForBattle loadHUDForBattle;
    public CharacterFromDB character;

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < character.abilities.Count; i++)
        {
            if(timeCounter[i] <= character.abilities[i].cooldown + Time.deltaTime)
            {
                if(loadHUDForBattle != null)
                {
                    cooldownImage[i].fillAmount = timeCounter[i] / character.abilities[i].cooldown;
                }
                timeCounter[i] += Time.deltaTime;
            }
        }
    }

    public void init(int characterIndex) {
        List<CharacterAbility> cooldownReductionAbilities = character.abilities.Where(a => a.type == "Passif" && a.effect == "Cooldown").ToList();
        if(cooldownReductionAbilities.Count > 0)
        {
            foreach (CharacterAbility crAbility in cooldownReductionAbilities)
            {
                foreach (CharacterAbility ability in character.abilities)
                {
                    if(ability.cooldown != null && ability.cooldown > 0)
                    {
                        ability.cooldown = ability.cooldown - ability.cooldown / (float)crAbility.value;
                    }
                }
            }
        }
        if (loadHUDForBattle != null)
        {
            cooldownImage = loadHUDForBattle.getHUDChildImages(characterIndex, character.abilities.Count, 4);
        }        
    }

    public void triggerCooldown(int abilityIndex)
    {
        timeCounter[abilityIndex] = 0;
        if (loadHUDForBattle != null)
        {
            cooldownImage[abilityIndex].fillAmount = 0f;
        }
    }

    public bool isCooldownOver(int abilityIndex)
    {
        return timeCounter[abilityIndex] >= character.abilities[abilityIndex].cooldown;
    }
}
