using System;
using System.Collections.Generic;

[Serializable]
public class CharacterFromDB
{
    public string id;
    public string name;
    public List<CharacterAbility> abilities;
    public Abilities abilitiesNew;
    public CharacterBonus bonus;
    public Equipement weapon;
    public Equipement armor;
    public Equipement gem;
    public int strength;
    public int defense;
    public int health;
    public int magic;
    public int level;
    public int experienceGiven;
    public int currentExperience;
    public int experienceForNextLevel;
    public CraftingMaterial craftingMaterialLoot;
    public int tier;
}

[Serializable]
public class CharacterBonus
{
    public int strength;
    public int defense;
    public int magic;
}

[Serializable]
public class Equipement : StandardEquipement
{
    public string id;
}

[Serializable]
public class StandardEquipement
{
    public string name;
    public EquipementTypeEnum type;
    public int hpValue;
    public int armorValue;
    public int strengthValue;
    public int magicValue;
}

[Serializable]
public enum EquipementTypeEnum
{
    NONE,
    WEAPON,
    ARMOR,
    GEM
}

[Serializable]
public class CraftingMaterial
{
    public string id;
    public string name;
    public string effect;
    public int tier;
    public int count;

    public string getLootPrefabName()
    {
        return effect + tier;
    }
}

[Serializable]
public class CharacterAbility : Ability
{
    public List<string> supportCrystals;

    public CharacterAbility()
    {
        if(supportCrystals == null || 0.Equals(supportCrystals.Count))
        {
            supportCrystals = new List<string>();
        }
    }
}

[Serializable]
public class Ability
{
    public string name;
    public string type;
    public float cooldown;
    public float castTime;
    public float range;
    public int tier;
    public int value;
    public string attribute;
    public string effect;
}

[Serializable]
public class SupportCrystal
{
    public string id;
    public string name;
    public List<string> types;
}

[Serializable]
public class StandardPet
{
    public string name;
    public List<string> abilities;
    public int magic;
    public int hp;
    public int strength;
    public int defense;
    public string type;
    public int experienceGieven;    
    public string craftingLootId;    
}

[Serializable]
public class Planet
{
    public string name;
    public List<AreaFromDB> areas;
    public bool done;
    public string loadingscreen;
}

[Serializable]
public class AreaFromDB
{
    public string name;
    public int packSize;
    public int packCount;
    public int lineCount;
    public int level;
    public List<string> monsters;
    public List<string> bosses;
    public string petLoot;
    public int petLootLevel;
    public string equipementLoot;
    public int tier;
    public bool done;
}

[Serializable]
public class FusionMatriceLine
{
    public string pet1;
    public string pet2;
    public string result;
}