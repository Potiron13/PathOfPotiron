using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AuraUnitController : MonoBehaviour
{
    public CharacterAbility aura;
    public LoadHUDForBattle loadHUDForBattle;

    private GameObject auraPrefab = null;
    private List<CharacterFromDB> pets;
    private CharacterFromDB player;


    private void Awake()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        pets = teamData.Pets;
        player = teamData.Player;
    }

    private void Start()
    {
        checkAndUpdate(gameObject.transform.parent.gameObject, true);
    }

    private void OnTriggerEnter(Collider other)
    {             
        checkAndUpdate(other.gameObject, true);
    }

    private void OnTriggerExit(Collider other)
    {
        checkAndUpdate(other.gameObject, false);
    }
    private void checkAndUpdate(GameObject otherGO, bool isEnter)
    {
        GameObject otherGameObject = otherGO;
        if (otherGameObject.tag == gameObject.tag)
        {
            PlayerData playerData = otherGameObject.GetComponent<PlayerData>();
            if (playerData != null)
            {
                updateCharacterStats(aura.attribute, aura.value, playerData.character, isEnter, otherGO);
            }
        }
    }

    private void updateCharacterStats(string attribute, int auraValue, CharacterFromDB character, bool isEnter, GameObject otherGO)
    {
        int orientedValue = isEnter ? auraValue : -auraValue;        
        if (isEnter && otherGO.transform.Find(aura.name + gameObject.name) == null)
        {
            updateCharacter(attribute, character, otherGO, orientedValue);

            auraPrefab = Instantiate(LoadFromResourcesService.abilityPrefab(aura.name), otherGO.transform);
            auraPrefab.name = aura.name + gameObject.name;
            auraPrefab.transform.position = Vector3.zero;
            auraPrefab.transform.localPosition = Vector3.zero;
        } else if (!isEnter && otherGO.transform.Find(aura.name + gameObject.name) != null)
        {
            updateCharacter(attribute, character, otherGO, orientedValue);
            Destroy(otherGO.transform.Find(aura.name + gameObject.name).gameObject);
        }

        if(loadHUDForBattle != null)
        {
            if (character.id == player.id)
            {
                updateAuraIcon(0, isEnter);
            }
            else
            {
                int petIndex = pets.FindIndex(p => p.id == character.id);
                updateAuraIcon(petIndex + 1, isEnter);
            }
        }                
    }

    private static void updateCharacter(string attribute, CharacterFromDB character, GameObject otherGO, int orientedValue)
    {
        if (attribute == "magic")
        {
            character.magic += orientedValue;
        }
        else if (attribute == "strength")
        {
            character.strength += orientedValue;
        }
        else if (attribute == "defense")
        {
            character.defense += orientedValue;
        }
        else if (attribute == "speed")
        {
            otherGO.GetComponent<NavMeshAgent>().speed += orientedValue;
        }
    }

    private void updateAuraIcon(int characterIndex, bool isEnter)
    {
        if (isEnter)
        {
            loadHUDForBattle.addStatusIcon(characterIndex, aura);
        } else
        {
            loadHUDForBattle.removeStatusIcon(characterIndex, aura);
        }
    }
}
