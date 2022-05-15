using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public PlayerData attacker;
    public CharacterAbility ability;
    private void OnTriggerEnter(Collider other)
    {
        GameObject otherGameObject = other.gameObject;
        if (otherGameObject.tag != gameObject.tag)
        {
            PlayerData playerData = otherGameObject.GetComponent<PlayerData>();
            if(playerData != null)
            {
                playerData.Attack(attacker.character, ability);
                Destroy(gameObject);
            }
        }
    }
}
