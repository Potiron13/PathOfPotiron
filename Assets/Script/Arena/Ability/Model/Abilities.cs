using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Abilities
{
    public List<ProjectileAbility> ProjectileAbilities { get; set; }
    public List<BeamAbility> BeamAbilities { get; set; }
    public List<PassiveAbility> PassiveAbilities { get; set; }

    public int GetMagicPassives()
    {
        throw new NotImplementedException();
    }

    public IDamageAbility GetDamageAbility(string abilityName)
    {
        ProjectileAbility projectile = ProjectileAbilities.FirstOrDefault(a => a.Name == abilityName);
        if(projectile != null)
        {
            return projectile;
        }
        return BeamAbilities.FirstOrDefault(a => a.Name == abilityName);
    }
}
