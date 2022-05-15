using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class PlayerModel : NetworkBehaviour
{    
    public HealthBar healthBar;
    [SerializeField]
    private NetworkVariable<long> networkHealth = new NetworkVariable<long>();    
    [SerializeField]
    private NetworkVariable<int> networkMagic = new NetworkVariable<int>();    
    [SerializeField]
    private NetworkVariable<int> networkStrength = new NetworkVariable<int>();    
    [SerializeField]
    private NetworkVariable<int> networkDefense = new NetworkVariable<int>();

    public CharacterFromDB Character { get; set; }

    private int maxHealth;

    public void Init()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        Character = teamData.Player;
        healthBar.setMaxHealth();
        maxHealth = Character.health;
        if (IsOwner)
        {
            InitStatsServerRpc(teamData.Player.health, teamData.Player.strength, teamData.Player.magic, teamData.Player.defense);
        }
    }


    [ServerRpc]
    private void InitStatsServerRpc(int health, int strength, int magic, int defense)
    {
        networkHealth.Value = (long) health;
        networkMagic.Value = (int) magic;
        networkStrength.Value = (int) strength;
        networkDefense.Value = (int) defense;
    }

    public void ApplyDamage(int damage, ulong targetId)
    {
        networkHealth.Value -= (long) damage;
        UpdateHealthBar();
        if (networkHealth.Value <= 0)
        {
            NetworkManager.Singleton.ConnectedClients[targetId].PlayerObject.Despawn();
        }
    }

    public void UpdateHealthBar()
    {
        healthBar.setHealth(networkHealth.Value / (float) maxHealth);
    }

    public void UpdateStats()
    {
        Character.magic = networkMagic.Value;
        Character.strength = networkStrength.Value;
        Character.defense = networkDefense.Value;
    }

    public void ApplyStatChange(string attribute, int orientedValue)
    {
        if (attribute == "magic")
        {
            networkMagic.Value += orientedValue;
        }
        else if (attribute == "strength")
        {
            networkStrength.Value += orientedValue;
        }
        else if (attribute == "defense")
        {
            networkDefense.Value += orientedValue;
        }
    }

    public long GetHealth()
    {
        return networkHealth.Value;
    }
    public int GetMagic()
    {
        return networkMagic.Value;
    }
    public int GetStrength()
    {
        return networkStrength.Value;
    }
    public int GetDefense()
    {
        return networkDefense.Value;
    }
}
