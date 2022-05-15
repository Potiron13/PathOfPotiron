using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadDetailMenu : MonoBehaviour
{
    public GameObject stats;
    public GameObject bonus;
    public void init(int characterIndex)
    {
        destroyChilds(stats);
        destroyChilds(bonus);

        CharacterFromDB character;
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        if (characterIndex == 0)
        {
            character = teamData.Player;
        }
        else
        {
            character = teamData.Pets[characterIndex - 1];
        }

        GameObject statsPrefab = LoadFromResourcesService.prefab("Stats");
        GameObject StatsGO = Instantiate(statsPrefab, stats.transform.position, Quaternion.identity, stats.transform);

        GetTextGO(StatsGO, 0).text = character.health.ToString();
        GetTextGO(StatsGO, 1).text = character.strength.ToString();
        GetTextGO(StatsGO, 2).text = character.magic.ToString();
        GetTextGO(StatsGO, 3).text = character.defense.ToString();
        GetTextGO(StatsGO, 4).text = character.currentExperience.ToString();
        GetTextGO(StatsGO, 5).text = character.experienceForNextLevel.ToString();

        GameObject bonusPrefab = LoadFromResourcesService.prefab("Bonus");
        GameObject bonusGO = Instantiate(bonusPrefab, bonus.transform.position, Quaternion.identity, bonus.transform);

        fillBonus(character, bonusGO);

        GameObject craftButtonPrefab = LoadFromResourcesService.prefab("CraftButton");
        GameObject craftButtonGO = Instantiate(craftButtonPrefab, bonus.transform.position, Quaternion.identity, bonus.transform);

        List<CraftingMaterial> craftingMaterials = LoadDataFromJson.LoadCraftingMaterials();
        for (int i = 0; i < craftingMaterials.Count; i++)
        {
            generateCrafting(characterIndex, character, bonusGO, craftButtonGO, craftingMaterials, i);
        }
    }

    private void generateCrafting(int characterIndex, CharacterFromDB character, GameObject bonusGO, GameObject craftButtonGO, List<CraftingMaterial> craftingMaterials, int i)
    {
        CraftingMaterial craftingMaterial = craftingMaterials[i];
        GetTextGO(craftButtonGO, i).text = craftingMaterial.count.ToString();
        GameObject buttonGO = craftButtonGO.transform.GetChild(i).gameObject.transform.GetChild(0).gameObject;
        buttonGO.SetActive(craftingMaterial.count > 0);
        Button button = buttonGO.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            craftingMaterial.count -= 1;
            SaveToJson.saveCraftingMaterials(craftingMaterials);
            character.bonus = CraftingMaterialService.calculateCharacterBonus(craftingMaterial);
            fillBonus(character, bonusGO);
            GetTextGO(craftButtonGO, i).text = craftingMaterial.count.ToString();
            if (characterIndex == 0)
            {
                SaveToJson.savePlayer(character);
            }
            else
            {
                TeamData teamData = LoadDataFromJson.LoadPlayerData();
                teamData.Pets[characterIndex - 1] = character;
                SaveToJson.savePets(teamData.Pets);
            }
            buttonGO.SetActive(craftingMaterial.count > 0);
        });
    }

   

    private void fillBonus(CharacterFromDB character, GameObject bonusGO)
    {        
        GetTextGO(bonusGO, 0).text = character.bonus.strength.ToString();
        GetTextGO(bonusGO, 1).text = character.bonus.magic.ToString();
        GetTextGO(bonusGO, 2).text = character.bonus.defense.ToString();
    }

    private void destroyChilds(GameObject toCleanGO)
    {
        foreach (Transform child in toCleanGO.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private Text GetTextGO(GameObject StatsGO, int statIndex)
    {
        return StatsGO.transform.GetChild(statIndex).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
    }

    public void HandleBackButton()
    {
        foreach (Transform child in stats.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
