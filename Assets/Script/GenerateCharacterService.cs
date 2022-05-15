using System;
using System.Collections.Generic;
using System.Linq;

public static class GenerateCharacterService
{
    private static readonly IDictionary<string, StandardPet> cache = new Dictionary<string, StandardPet>();
    public static CharacterFromDB generateCharacter(string name, int level, CharacterFromDB character, int tier = 0)
    {
        StandardPet standardPet = getStandarPet(name);
        List<CharacterAbility> standardAbilities = LoadDataFromJson.LoadStandardAbilities();

        CharacterFromDB characterFromDB = new CharacterFromDB();
        characterFromDB.abilities = new List<CharacterAbility>();        
        if (character == null)
        {
            characterFromDB.tier = name == "Potiron" ? 3 : tier;
            for (int i = 0; i < characterFromDB.tier + 1; i++)
            {
                if(standardPet.abilities.Count > i)
                {
                    CharacterAbility ability = standardAbilities.Where(a => a.name == standardPet.abilities[i]).First();
                    ability.supportCrystals = CrystalSupportService.generateCrystalSupports();
                    characterFromDB.abilities.Add(ability);
                }
            }
        }
        else
        {
            characterFromDB.tier = character.tier;            
            characterFromDB.bonus = new CharacterBonus();
            characterFromDB.abilities = character.abilities;            
        }

        if (standardPet.craftingLootId != null)
        {
            characterFromDB.craftingMaterialLoot = LoadDataFromJson.LoadCraftingMaterials().Where(c => c.id == standardPet.craftingLootId).FirstOrDefault();
        }
        characterFromDB.name = name;
        characterFromDB.id = MockHelper.generateID();
        characterFromDB.level = level;
        characterFromDB.currentExperience = 0;
        characterFromDB.experienceGiven = (int)Math.Ceiling(standardPet.experienceGieven * (float)level / 700);
        characterFromDB.experienceForNextLevel = 10 * level ^ 3;
        BaseStats baseStats = calculateStats(standardPet, level, character);
        characterFromDB.health = baseStats.hp;
        characterFromDB.magic = baseStats.magic;
        characterFromDB.defense = baseStats.defense;
        characterFromDB.strength = baseStats.strength;
        if (character != null)
        {
            characterFromDB.armor = character.armor;
            characterFromDB.gem = character.gem;
            characterFromDB.weapon = character.weapon;
            characterFromDB.currentExperience = character.currentExperience;
        }
        characterFromDB.strength = baseStats.strength;

        return characterFromDB;
    }

    private static StandardPet getStandarPet(string name)
    {
        if (!cache.ContainsKey(name))
        {
            StandardPet standardPet = LoadDataFromJson.LoadStandardPets().First(pet => pet.name == name);
            cache.Add(name, standardPet);
        }
        return cache[name];
    }

    private static BaseStats calculateStats(StandardPet standardPet, int level, CharacterFromDB character)
    {
        BaseStats baseStats = new BaseStats();
        switch (standardPet.type)
        {
            case "mage":
                baseStats.hp += calculateHp(standardPet.hp, level);
                baseStats.magic += calculateStat(standardPet.magic, level, level);
                baseStats.strength += calculateStat(standardPet.strength, level, 0);
                baseStats.defense += calculateStat(standardPet.defense, level, 1);
                return baseStats;
            case "warrior":
                baseStats.hp += calculateHp(standardPet.hp, level);
                baseStats.magic += calculateStat(standardPet.magic, level, 0);
                baseStats.strength += calculateStat(standardPet.strength, level, 3);
                baseStats.defense += calculateStat(standardPet.defense, level, 4);
                return baseStats;
            default:
                return baseStats;
        }
    }

    private static int calculateStat(int stat, int level, int bonus)
    {
        return (int)Math.Ceiling((float)(stat * 2 * level / 50) + 5 + bonus);
    }

    private static int calculateHp(int hp, int level)
    {
        return (int)Math.Ceiling((float)(hp * 2 * level / 50) + level + 10);
    }

    class BaseStats
    {
        public int hp;
        public int strength;
        public int magic;
        public int defense;
    }

}
