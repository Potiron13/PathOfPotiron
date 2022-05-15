using Potiron.Core.Singletons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIControl : Singleton<CombatUIControl>
{
    [SerializeField]
    private List<Image> abilitiesImage;

    private List<float> timeCounter = new List<float>() { 0, 0, 0, 0 };
    private CharacterFromDB character;

    private void Start()
    {
        // config
        CharacterConfig.AbilityConfiguration config1 = new CharacterConfig.AbilityConfiguration();
        config1.buttonName = "Fire1";
        CharacterConfig.AbilityConfiguration config2 = new CharacterConfig.AbilityConfiguration();
        config2.buttonName = "Fire2";
        CharacterConfig.AbilityConfiguration config3 = new CharacterConfig.AbilityConfiguration();
        config3.buttonName = "Fire3";
        CharacterConfig.AbilityConfiguration config4 = new CharacterConfig.AbilityConfiguration();
        config4.buttonName = "Fire4";
        LoadSceneData.characterConfig.abilityConfigurations = new List<CharacterConfig.AbilityConfiguration>();
        LoadSceneData.characterConfig.abilityConfigurations.Add(config1);
        LoadSceneData.characterConfig.abilityConfigurations.Add(config2);
        LoadSceneData.characterConfig.abilityConfigurations.Add(config3);
        LoadSceneData.characterConfig.abilityConfigurations.Add(config4);
    }

    private void Update()
    {
        if(character != null)
        {
            for (int i = 0; i < character.abilities.Count; i++)
            {
                if (timeCounter[i] <= character.abilities[i].cooldown + Time.deltaTime)
                {
                    abilitiesImage[i].fillAmount = timeCounter[i] / character.abilities[i].cooldown;
                    timeCounter[i] += Time.deltaTime;
                }
            }
        }        
    }

    public void Init(PlayerModel playerModel)
    {
        List<CharacterAbility> abilities = playerModel.Character.abilities;
        character = playerModel.Character;
        for (int i = 0; i < abilities.Count; i++)
        {
            abilitiesImage[i].sprite = LoadFromResourcesService.abilitySprite(abilities[i].name);
        }

        HandleCooldownReduction();     
    }

    private void HandleCooldownReduction()
    {
        List<CharacterAbility> cooldownReductionAbilities = character.abilities.Where(a => a.type == "Passif" && a.effect == "Cooldown").ToList();
        if (cooldownReductionAbilities.Count > 0)
        {
            foreach (CharacterAbility crAbility in cooldownReductionAbilities)
            {
                foreach (CharacterAbility ability in character.abilities)
                {
                    if (ability.cooldown != null && ability.cooldown > 0)
                    {
                        ability.cooldown = ability.cooldown - ability.cooldown / (float)crAbility.value;
                    }
                }
            }
        }
    }

    public void triggerCooldown(int abilityIndex)
    {
        timeCounter[abilityIndex] = 0;
        abilitiesImage[abilityIndex].fillAmount = 0f;        
    }

    public bool isCooldownOver(int abilityIndex)
    {
        return timeCounter[abilityIndex] >= character.abilities[abilityIndex].cooldown;
    }
}