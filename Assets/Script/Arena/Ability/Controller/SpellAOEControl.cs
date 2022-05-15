using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpellAOEControl : NetworkBehaviour, IAbilityControl
{
    public ulong attackerId;
    public string abilityName;

    public void UseAbilityServerSide(ulong attackerId, GameObject abilityGO, int angle, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName, Vector3 steeringTarget, float range)
    {
        abilityGO.transform.position = attackerPosition + Quaternion.AngleAxis(angle, Vector3.up) * (attackerForward * 2);
        abilityGO.GetComponent<SpellAOEControl>().attackerId = attackerId;
        abilityGO.GetComponent<SpellAOEControl>().abilityName = abilityName;
        abilityGO.GetComponent<NetworkObject>().Spawn();
        Destroy(abilityGO, 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsClient && IsOwner && other.GetComponent<NetworkObject>() != null)
        {
            ulong targetId = other.GetComponent<NetworkObject>().OwnerClientId;
            PlayerModel targetModel = other.GetComponent<PlayerModel>();
            if (targetModel != null && attackerId != targetId)
            {
                DamageControl.Instance.ApplyDamageOverTimeServerRpc(targetId, attackerId, abilityName);
            }
        }
    }
}
