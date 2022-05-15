using Unity.Netcode;
using UnityEngine;

public class TravelControl : NetworkBehaviour, IAbilityControl
{
    public void UseAbilityServerSide(ulong attackerId, GameObject abilityGO, int angle, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName, Vector3 steeringTarget, float range)
    {
        abilityGO.transform.position = PhysicsService.calculateAbilityPosition(attackerPosition, steeringTarget, range);
        abilityGO.GetComponent<NetworkObject>().Spawn();
        Destroy(abilityGO, 1);
    }
}
