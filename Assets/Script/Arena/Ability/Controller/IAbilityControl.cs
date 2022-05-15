using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAbilityControl
{
    void UseAbilityServerSide(ulong attackerId, GameObject abilityGO, int angle, 
        Vector3 attackerPosition, Vector3 attackerForward, Vector3 attackerRotation, string abilityName,
        Vector3 steeringTargtet, float range);

}
