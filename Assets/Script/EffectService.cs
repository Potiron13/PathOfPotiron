using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectService
{
    public static void applyEffect(CharacterAbility ability, GameObject targetGO)
    {
        if(ability.effect == "Stop")
        {
            targetGO.GetComponent<EffectController>().isStoped = true;
        }
    }
}
