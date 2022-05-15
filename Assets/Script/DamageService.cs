using System;
using System.Collections.Generic;
using System.Linq;

public static class DamageService
{

    public static int calculate(CharacterFromDB attacker, CharacterFromDB target, CharacterAbility ability)
    {
        int stat;
        int weapon;
        int totalStrength = calculatePassif(attacker.strength * 2, getPassiveAbilities(attacker.abilities, "strength")) + 
            (attacker.bonus != null ? attacker.bonus.strength : 0);
        int totalMagic = calculatePassif(attacker.magic, getPassiveAbilities(attacker.abilities, "magic")) +
            (attacker.bonus != null ? attacker.bonus.magic : 0);
        switch (ability.type)
        {
            case "Projectile":
                stat = totalMagic;
                weapon = attacker.weapon != null ? attacker.weapon.magicValue : 0;
                break;
            case "Beam":
                stat = 5 * (totalMagic);
                weapon = attacker.weapon != null ? 5 * attacker.weapon.magicValue : 0;
                break;
            case "SpellAOE":                
                stat = (int)Math.Ceiling((float)(totalMagic) / 3);
                weapon = attacker.weapon != null ? (int)Math.Ceiling((float)attacker.weapon.magicValue / 3) : 0;
                break;
            case "Attack":
            case "MeleeAOE":                
                stat = totalStrength;
                weapon = attacker.weapon != null ? attacker.weapon.strengthValue : 0;
                break;
            case "Passif":
                if(ability.effect == "Reflect")
                {
                    stat = attacker.armor != null ? attacker.armor.armorValue : 0;
                    weapon = calculatePassif(attacker.defense, getPassiveAbilities(attacker.abilities, "defense"));
                } else
                {
                    stat = 0;
                    weapon = 0;
                }                
                break;
            default:
                stat = 0;
                weapon = 0;
                break;
        }
        int attack = stat + weapon;
        int totalDefense = calculatePassif(target.defense, getPassiveAbilities(attacker.abilities, "defense")) +
            (attacker.bonus != null ? attacker.bonus.defense : 0); ;
        int defense = totalDefense + (target.armor != null ? target.armor.armorValue : 0);
        return (int)Math.Ceiling(attack * (float)100 / (100 + defense));
    }

    public static int calculatePassif(int attackerAttribute, List<CharacterAbility> passives)
    {
        int total = attackerAttribute;
        foreach (CharacterAbility passif in passives)
        {
            total += passif.value;
        }
        return total;
    }

    public static List<CharacterAbility> getPassiveAbilities(List<CharacterAbility> abilities, string attribute) {
        return abilities.Where(a => a.type == "Passif" && a.attribute == attribute).ToList();
    }

}
