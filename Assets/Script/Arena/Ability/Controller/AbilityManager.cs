using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class AbilityManager: NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> abilitiePrefabs;

    private PlayerModel playerModel;

    [ServerRpc]
    public void UpdateStatsServerRpc(ulong owerClientId, string attribute, int value)
    {
        NetworkManager.Singleton.ConnectedClients[owerClientId].PlayerObject.GetComponent<PlayerModel>().ApplyStatChange(attribute, value);
    }

    public void UseAbilityClientSide(NavMeshAgent agent, CharacterAbility ability)
    {
        if(ability.type == "Travel")
        {
            agent.Warp(PhysicsService.calculateAbilityPosition(gameObject.transform.position, agent.steeringTarget, ability.range));
        }
    }

    [ServerRpc]
    public void UseAbilityServerRpc(ulong attackerId, string abilityName, Vector3 attackerPosition,
        Vector3 attackerForward, Vector3 attackerRotation, int angleNumber, int attackNumber, Vector3 steeringTargtet, float range)
    {
       StartCoroutine(AbilityCoroutine(abilityName, attackerId, attackNumber, angleNumber, attackerPosition, attackerRotation, attackerForward, steeringTargtet, range));
    }

    public void Init()
    {
        playerModel = GetComponent<PlayerModel>();
    }

    private void SpawnAbility(int angle, ulong attackerId, string abilityName, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, Vector3 steeringTargtet, float range)
    {
        GameObject abilityGO = NetworkObjectPool.Instance.GetNetworkObject(abilitiePrefabs.Find(p => p.name == abilityName)).gameObject;
        abilityGO.GetComponent<IAbilityControl>().UseAbilityServerSide(attackerId, abilityGO, angle, attackerPosition, attackerForward, attackerRotation, abilityName, steeringTargtet, range);        
    }

    IEnumerator AbilityCoroutine(string abilityName, ulong attackerId, int attackNumber, int angleNumber, Vector3 attackerPosition, Vector3 attackerRotation, Vector3 attackerFwd, Vector3 steeringTargtet, float range)
    {
        for (int i = 0; i < attackNumber; i++)
        {
            for (int j = 0; j < angleNumber; j++)
            {
                SpawnAbility(getAnglesFromIndex(j), attackerId, abilityName, attackerPosition, attackerFwd, attackerRotation, steeringTargtet, range);
            }
            yield return new WaitForSeconds(0.25f);
        }
    }

    private int getAnglesFromIndex(int j)
    {
        return j == 0 ? 0 : Mathf.CeilToInt(45 * Mathf.Pow(-1, j));
    }
    
    [ServerRpc]
    public void InstanciateAuraServerRpc(string abilityName, string attribute, int value)
    {
        GameObject abilityGo = NetworkObjectPool.Instance.GetNetworkObject(abilitiePrefabs.Find(p => p.name == abilityName)).gameObject;
        abilityGo.GetComponent<AuraUnitControl>().attribute = attribute;
        abilityGo.GetComponent<AuraUnitControl>().value = value;
        abilityGo.GetComponent<NetworkObject>().Spawn();
        abilityGo.transform.parent = gameObject.transform;
        abilityGo.transform.localPosition = Vector3.zero;
        abilityGo.transform.localRotation = Quaternion.Euler(new Vector3(90,0,0));
    }
}
