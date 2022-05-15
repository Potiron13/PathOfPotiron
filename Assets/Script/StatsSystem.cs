using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterFromDB;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles the stats of a CharacterData. It stores the health and strength/agility/defense stats.
/// This class contains various functions for interacting with stats, by adding stat modifications, elemental
/// effects or damage.
/// </summary>
[System.Serializable]
public class StatsSystem
{      
    /// <summary>
    /// Store the stats, which are composed of 4 values : health, strength, agility and defense.
    /// It also contains elemental protections and boost (1 for each elements defined by the DamageType enum)
    /// </summary>
    [System.Serializable]
    public class Stats
    {
        //Integer for simplicity, may switch to float later on. For now everything is integer
        public int strength;
        public int health;
        public int magic;
        public int maxRange;

        public List<CharacterAbility> abilities;

        public void Copy(Stats other)
        {
            health = other.health;
            maxRange = other.maxRange;
        }
    }


    public Stats stats;

    public int CurrentHealth { get; private set; }
    public int MaxRange { get; private set; }
    PlayerData m_Owner;
       
    public void Init(PlayerData owner)
    {
        stats.Copy(stats);
        CurrentHealth = stats.health;
        MaxRange = stats.maxRange;
        m_Owner = owner;
    }            

    /// <summary>
    /// Change the health by the given amount : negative amount damage, positive amount heal. The function will
    /// take care of clamping the value in the range [0...MaxHealth]
    /// </summary>
    /// <param name="amount"></param>
    public void ChangeHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, stats.health);        
    }

    public void Damage(int damage)
    {
        ChangeHealth(-damage);            
    }
}
