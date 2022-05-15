
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AbsrobService
{
    private static readonly List<string> rangedAbilities = new List<string>
    {
        "Beam", "Projectile", "SpellAOE", "Travel"
    };
    private static readonly List<string> meleeAbilities = new List<string>
    {
        "Attack", "MeleeAOE", "Passif", "Aura"
    };
    public static void abosrb(CharacterFromDB player, CharacterFromDB pet, int petAbilityIndex, int playerAbilityIndex, int petIndex)
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        CharacterAbility petAbility = pet.abilities[petAbilityIndex];
        CharacterAbility newAbility = null;
        if (playerAbilityIndex < player.abilities.Count)
        {
            CharacterAbility playerAbility = player.abilities[playerAbilityIndex];
            bool playerIsRanged = rangedAbilities.Contains(playerAbility.type);
            bool petIsRanged = rangedAbilities.Contains(petAbility.type);
            bool playerIsMelee = meleeAbilities.Contains(playerAbility.type);
            bool petIsMelee = meleeAbilities.Contains(petAbility.type);
            int tier = System.Math.Max(playerAbility.tier, petAbility.tier);
            if (playerAbility.tier == petAbility.tier && tier <= 2)
            {
                tier += 1;
            }
            List<string> abilityPool = calculateAbilityPool(playerAbility, petAbility, playerIsRanged, petIsRanged, playerIsMelee, petIsMelee);
            List<CharacterAbility> abilities = LoadDataFromJson.LoadStandardAbilities();
            List<CharacterAbility> possibleAbilities = abilities.Where(a => abilityPool.Contains(a.type)).ToList();
            newAbility = possibleAbilities[Random.Range(0, possibleAbilities.Count)];
            newAbility.supportCrystals = playerAbility.supportCrystals;
            if (newAbility.supportCrystals.Count < 2)
            {
                newAbility.supportCrystals.Add(petAbility.supportCrystals[0]);
            }
            player.abilities[playerAbilityIndex] = newAbility;
        } else
        {
            player.abilities.Add(petAbility);
        }
        SaveToJson.savePlayer(player);
        teamData.Pets.RemoveAt(petIndex - 1);
        SaveToJson.savePets(teamData.Pets);
    }

    private static List<string> calculateAbilityPool(CharacterAbility playerAbility, CharacterAbility petAbility, bool playerIsRanged, bool petIsRanged, bool playerIsMelee, bool petIsMelee)
    {
        List<string> abilityPool = new List<string>();
        if (playerAbility.tier == petAbility.tier)
        {
            abilityPool = choseAbilityFromType(playerIsMelee && petIsMelee, playerIsRanged && petIsRanged);
        }
        else
        {
            if (playerAbility.tier > petAbility.tier)
            {
                abilityPool = choseAbilityFromType(playerIsMelee, playerIsRanged);
            }
            else
            {
                abilityPool = choseAbilityFromType(petIsMelee, petIsRanged);
            }
        }

        return abilityPool;
    }

    private static List<string> choseAbilityFromType(bool meleeCondition, bool rangedCondition)
    {
        List<string> abilityPool = new List<string>();
        List<List<string>> abilityPoolList = new List<List<string>> { meleeAbilities, rangedAbilities };
        if (meleeCondition)
        {
            abilityPool = meleeAbilities;
        }
        else if (rangedCondition)
        {
            abilityPool = rangedAbilities;
        } else
        {
            abilityPool = abilityPoolList[Random.Range(0, 1)];
        }

        return abilityPool;
    }
}
