using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileControl : NetworkBehaviour, IAbilityControl
{
    public string abilityName;
    public ulong attackerId;
    private void OnTriggerEnter(Collider other)
    {
        if (IsClient && IsOwner && other.GetComponent<NetworkObject>() != null)
        {
            ulong targetId = other.GetComponent<NetworkObject>().OwnerClientId;
            PlayerModel targetModel = other.GetComponent<PlayerModel>();
            if(targetModel != null && attackerId != targetId)
            {
                DamageControl.Instance.ApplyDamageServerRpc(targetId, OwnerClientId, abilityName);
                DestroyProjectileServerRpc();
            }
        }
    }

    [ServerRpc]
    private void DestroyProjectileServerRpc()
    {
        Destroy(gameObject);
    }

    public void UseAbilityServerSide(ulong attackerId, GameObject abilityGO, int angle, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName, Vector3 steeringTarget, float range)
    {
        abilityGO.transform.position = new Vector3(attackerPosition.x, 1, attackerPosition.z) + (attackerForward * 2);
        abilityGO.transform.rotation = Quaternion.Euler(CrystalSupportService.rotateVectorOnY(attackerRotation, angle));
        abilityGO.GetComponent<Rigidbody>().velocity = abilityGO.transform.forward * 10;
        abilityGO.GetComponent<ProjectileControl>().abilityName = abilityName;
        abilityGO.GetComponent<ProjectileControl>().attackerId = attackerId;
        abilityGO.GetComponent<NetworkObject>().Spawn();
        Destroy(abilityGO, 5);
    }
}
