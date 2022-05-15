using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CrystalDropDown : MonoBehaviour
{    
    public Dropdown dropdown;       
    private CharacterFromDB characterFromDB;
    private TeamData teamData;
    private int characterIndex;
    private int abilityIndex;

    public void InitDropdown(int characterIndex, int abilityIndex, int dropdownIndex)
    {
        characterFromDB = getCharacter(characterIndex, abilityIndex);
        CharacterAbility ability = characterFromDB.abilities[abilityIndex];
        List<string> crystalsNames = LoadDataFromJson.LoadSupportCrystals().Where(c => c.types.Contains(ability.type)).Select(c => c.name).ToList();
        fillDropdown(crystalsNames);
        dropdown.onValueChanged.AddListener(delegate { ItemDropdownChanged(dropdown, dropdownIndex); });
        int.TryParse(characterFromDB.abilities[abilityIndex].supportCrystals[dropdownIndex], out int crystalIndex);
        dropdown.value = crystalIndex;
    }

    private CharacterFromDB getCharacter(int characterIndex, int abilityIndex)
    {        
        teamData = LoadDataFromJson.LoadPlayerData();
        this.abilityIndex = abilityIndex;
        this.characterIndex = characterIndex;
        return characterIndex == 0 ? teamData.Player : teamData.Pets[characterIndex - 1];        
    }

    private void ItemDropdownChanged(Dropdown dropdown, int dropdownIndex)
    {
        characterFromDB.abilities[abilityIndex].supportCrystals[dropdownIndex] = dropdown.value.ToString();                
        if(characterIndex == 0)
        {
            SaveToJson.savePlayer(characterFromDB);            
        } else
        {
            teamData.Pets[characterIndex - 1] = characterFromDB;
            SaveToJson.savePets(teamData.Pets);            
        }        
    }

    private void fillDropdown(List<string> optionalTextList)
    {
        dropdown.ClearOptions();
        addOption("none");
        foreach (string text in optionalTextList)
        {
            addOption(text);
        }
    }

    private void addOption(string text)
    {
        Dropdown.OptionData optionData = new Dropdown.OptionData();
        optionData.text = text;
        dropdown.options.Add(optionData);
    }
}
