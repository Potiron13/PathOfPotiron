using Unity.Netcode;
using UnityEngine;

public class MeleeAOEControl : NetworkBehaviour, IAbilityControl
{
    public ulong attackerId;
    public string abilityName;

    public void UseAbilityServerSide(ulong attackerId, GameObject abilityGo, int angle, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName, Vector3 steeringTarget, float range)
    {
        abilityGo.transform.position = attackerPosition + Quaternion.AngleAxis(angle, Vector3.up) * (attackerForward * 2);
        abilityGo.GetComponent<MeleeAOEControl>().attackerId = attackerId;
        abilityGo.GetComponent<MeleeAOEControl>().abilityName = abilityName;
        abilityGo.GetComponent<NetworkObject>().Spawn();
        Destroy(abilityGo, 3);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsClient && IsOwner && other.GetComponent<NetworkObject>() != null)
        {
            ulong targetId = other.GetComponent<NetworkObject>().OwnerClientId;
            PlayerModel targetModel = other.GetComponent<PlayerModel>();
            if (targetModel != null && attackerId != targetId)
            {
                DamageControl.Instance.ApplyDamageServerRpc(other.GetComponent<NetworkObject>().OwnerClientId, attackerId, abilityName);
            }
        }
    }
}
