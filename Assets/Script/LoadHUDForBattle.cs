using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadHUDForBattle : MonoBehaviour
{
    public List<GameObject> charactersHUD;
    public PlayerControl playerControl;
    public List<CharacterControl> characterControls;

    private List<List<GameObject>> abilityButtons = new List<List<GameObject>>()
    {
        new List<GameObject>(),
        new List<GameObject>(),
        new List<GameObject>()
    };

    void Start()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        List<CharacterFromDB> pets = teamData.Pets;        

        initCharacterCombatHUD(teamData.Player, charactersHUD[0], 0);
        for (int i = 0; i < pets.Count; i++)
        {
            initCharacterCombatHUD(teamData.Pets[i], charactersHUD[i + 1], i+1);
        }
    }

    public void triggerCooldown(int characterIndex, int abilityIndex)
    {
        charactersHUD[characterIndex].transform.GetChild(4).gameObject.transform.GetChild(abilityIndex).gameObject.GetComponent<Image>().fillAmount = 1f;
    }

    private void initCharacterCombatHUD(CharacterFromDB character, GameObject characterHUD, int characterIndex)
    {
        characterHUD.SetActive(true);
        characterHUD.transform.GetChild(0).gameObject.GetComponent<Text>().text = character.name;
        characterHUD.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LoadFromResourcesService.monsterSprite(character.name);
        characterHUD.transform.GetChild(3).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = character.level.ToString();
        for (int i = 0; i < character.abilities.Count; i++)
        {
            generateAbilityButton(character, characterHUD, characterIndex, i);
        }
    }

    private void generateAbilityButton(CharacterFromDB character, GameObject characterHUD, int characterIndex, int abilityIndex)
    {
        GameObject abilityButton = characterHUD.transform.GetChild(4).gameObject.transform.GetChild(abilityIndex).gameObject;
        abilityButtons[characterIndex].Add(abilityButton);
        abilityButton.SetActive(true);
        abilityButton.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LoadFromResourcesService.abilitySprite(character.abilities[abilityIndex].name);
        if(character.abilities[abilityIndex].type != "Passif" && character.abilities[abilityIndex].type != "Aura")
        {
            abilityButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (characterIndex == 0)
                {
                    playerControl.switchCurrentAbility(abilityIndex);
                }
                else
                {
                    characterControls[characterIndex - 1].switchCurrentAbility(abilityIndex);
                }
                foreach (GameObject abilityButton in abilityButtons[characterIndex])
                {
                    abilityButton.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f);
                }
                abilityButton.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f);
            });
        }
    }

    public void removeStatusIcon(int characterIndex, CharacterAbility aura)
    {
        List<Image> images = getHUDChildImages(characterIndex, 4, 5);
        Image image = images.FirstOrDefault(i => i.sprite.name == aura.name);
        if(image != null)
        {
            image.sprite = LoadFromResourcesService.abilitySprite("EmptyImage");
        }
    }

    public void addStatusIcon(int characterIndex, CharacterAbility aura)
    {
        List<Image> images = getHUDChildImages(characterIndex, 4, 5);
        Image image = images.FirstOrDefault(i => i.sprite.name == "EmptyImage");
        Image existaingAura = images.FirstOrDefault(i => i.sprite.name == aura.name);
        if(image != null && existaingAura == null)
        {
            image.sprite = LoadFromResourcesService.abilitySprite(aura.name);            
        }
    }

    public List<Image> getHUDChildImages(int characterIndex, int imageNumber, int HUDChildIndex)
    {
        List<Image> images = new List<Image>();
        for (int i = 0; i < imageNumber; i++)
        {
            images.Add(charactersHUD[characterIndex].transform.GetChild(HUDChildIndex).gameObject.transform.GetChild(i).gameObject
                .transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>());
        }
        return images;
    }
}
