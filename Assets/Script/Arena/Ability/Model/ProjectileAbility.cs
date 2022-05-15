using System;

public class ProjectileAbility : ActiveAbility, IDamageAbility
{
    public int GetDamage(CharacterFromDB attacker, CharacterFromDB target)
    {
        // int totalMagic = attacker.magic + attacker.abilitiesNew.GetMagicPassives();
        int totalMagic = DamageService.calculatePassif(attacker.magic, DamageService.getPassiveAbilities(attacker.abilities, "magic")) +
            (attacker.bonus != null ? attacker.bonus.magic : 0);
        int weapon = attacker.weapon != null ? attacker.weapon.magicValue : 0;
        int attack = totalMagic + weapon;
        int totalDefense = DamageService.calculatePassif(target.defense, DamageService.getPassiveAbilities(attacker.abilities, "defense")) +
            (attacker.bonus != null ? attacker.bonus.defense : 0); ;
        int defense = totalDefense + (target.armor != null ? target.armor.armorValue : 0);
        return (int)Math.Ceiling(attack * (float)100 / (100 + defense));
    }
}