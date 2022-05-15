using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class AuraUnitControl : NetworkBehaviour, IAbilityControl
{
    public string attribute;
    public int value;

    public void UseAbilityServerSide(ulong attackerId, GameObject abilityGO, int angle, Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName, Vector3 steeringTargtet, float range)
    {
        // TODO
    }
}
