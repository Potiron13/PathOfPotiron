using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class FusionService
{
    public static CharacterFromDB fusePets(List<CharacterFromDB> petsToFuse)
    {
        List<FusionMatriceLine> fusionMatrice = LoadDataFromJson.LoadFusionMatrice();
        List<string> petsName = petsToFuse.Select(p => p.name).ToList();
        string petName = petsName[0];
        foreach (FusionMatriceLine line in fusionMatrice)
        {
            if(!string.IsNullOrEmpty(getResult(line, petsName)))
            {
                petName = getResult(line, petsName);
            }
        }
        int tier = Mathf.Max(petsToFuse[0].tier, petsToFuse[1].tier);
        if (petsToFuse[0].tier == petsToFuse[1].tier && petsToFuse[0].tier < 3)
        {
            tier += 1;
        }
        CharacterFromDB fusion = GenerateCharacterService.generateCharacter(petName, Mathf.Min(petsToFuse[0].level, petsToFuse[1].level), null, tier);

        return fusion;
    }

    private static string getResult(FusionMatriceLine line, List<string> petsName) {
        bool firstCombination = line.pet1 == petsName[0] && line.pet2 == petsName[1];
        bool secondCombination = line.pet1 == petsName[1] && line.pet2 == petsName[0];
        if(firstCombination || secondCombination)
        {
            return line.result;
        }
        return "";
    }
}
