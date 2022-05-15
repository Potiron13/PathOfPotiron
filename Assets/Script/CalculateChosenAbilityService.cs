using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CalculateChosenAbilityService
{
    public static int switchCurrentAbility(CharacterFromDB character)
    {
        List<CharacterAbility> abilities = character.abilities.Where(a => a.type != "Aura" && a.type != "Passif").OrderBy(a => a.tier).ToList();
        int tier = getTier();
        List<CharacterAbility> chosenAbilities = abilities.Where(a => a.tier == tier).ToList();
        if(chosenAbilities.Count == 0)
        {
            return 0;
        } else
        {
            CharacterAbility chosenAbility = chosenAbilities[Random.Range(0, chosenAbilities.Count)];
            return character.abilities.FindIndex(a => a.name == chosenAbility.name);
        }
    }

    public static int getTier()
    {
        int dice = Random.Range(1, 100);
        if (dice < 50)
        {
            return 0;
        } else if (dice >= 50 && dice < 80)
        {
            return 1;
        } else
        {
            return 2;
        }
    }
}
