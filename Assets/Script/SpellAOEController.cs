using System.Collections;
using UnityEngine;

public class SpellAOEController : MonoBehaviour
{
    public PlayerData attacker;
    public CharacterAbility ability;
    private void OnTriggerEnter(Collider other)
    {
        GameObject otherGameObject = other.gameObject;
        PlayerData playerData = otherGameObject.GetComponent<PlayerData>();
        if (playerData != null)
        {
            StartCoroutine(applyDamageOverTime(playerData, otherGameObject));
        }
    }

    IEnumerator applyDamageOverTime(PlayerData playerData, GameObject targetGameObject)
    {
        int count = 10;
        while (count >= 1)
        {   
            if (targetGameObject != null && targetGameObject.tag == gameObject.tag && ability.effect == "Heal")
            {
                playerData.Heal(attacker.character, ability);
            }
            else if (targetGameObject != null && targetGameObject.tag != gameObject.tag)
            {
                playerData.Attack(attacker.character, ability);
            }
            count--;
            yield return new WaitForSeconds(1f);
        }        
    }
}
