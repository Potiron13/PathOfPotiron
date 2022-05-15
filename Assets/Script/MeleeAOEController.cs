using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAOEController : MonoBehaviour
{
    public PlayerData attacker;
    public CharacterAbility ability;
    private void OnTriggerEnter(Collider other)
    {
        GameObject otherGameObject = other.gameObject;
        if (otherGameObject.tag != gameObject.tag)
        {
            PlayerData playerData = otherGameObject.GetComponent<PlayerData>();
            if(playerData != null && attacker != null)
            {
                playerData.Attack(attacker.character, ability);
            }
        }
    }
}
