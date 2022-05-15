using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CraftingMaterialService
{
    public static CharacterBonus calculateCharacterBonus(CraftingMaterial craftingMaterial)
    {
        CharacterBonus characterBonus = new CharacterBonus();
        int minRange = 0;
        int maxRange = 0;
        switch (craftingMaterial.tier)
        {
            case 1:
                minRange = 0;
                maxRange = 100;
                break;
            default:
                break;
        }
        switch (craftingMaterial.effect)
        {
            case "offense":
                characterBonus.strength = calculateBonus(minRange, maxRange);
                characterBonus.magic = calculateBonus(minRange, maxRange);
                break;
            case "defense":                
                characterBonus.defense = calculateBonus(minRange, maxRange);
                break;
            default:
                break;
        }
        return characterBonus;
    }
    private static int calculateBonus(int minRange, int maxRange)
    {
        return UnityEngine.Random.Range(minRange, maxRange);
    }
}
