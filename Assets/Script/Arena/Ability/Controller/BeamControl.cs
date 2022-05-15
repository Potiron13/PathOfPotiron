using Unity.Netcode;
using UnityEngine;

public class BeamControl : NetworkBehaviour, IAbilityControl
{
    public ulong attackerId;
    public string abilityName;

    public void UseAbilityServerSide(ulong attackerId, GameObject abilityGO, int angle, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName, Vector3 steeringTarget, float range)
    {
        abilityGO.transform.position = new Vector3(attackerPosition.x, 1, attackerPosition.z) + (attackerForward * 2);
        abilityGO.transform.rotation = Quaternion.Euler(CrystalSupportService.rotateVectorOnY(attackerRotation, angle));
        abilityGO.GetComponent<BeamControl>().attackerId = attackerId;
        abilityGO.GetComponent<BeamControl>().abilityName = abilityName;
        abilityGO.GetComponent<NetworkObject>().Spawn();
        Destroy(abilityGO, 1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsClient && IsOwner && other.GetComponent<NetworkObject>() != null)
        {
            ulong targetId = other.GetComponent<NetworkObject>().OwnerClientId;
            PlayerModel targetModel = other.GetComponent<PlayerModel>();
            if(targetModel != null && attackerId != targetId)
            {
                DamageControl.Instance.ApplyDamageServerRpc(targetId, attackerId, abilityName);
            }
        }
    }
}
