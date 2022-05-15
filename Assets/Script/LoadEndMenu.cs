using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadEndMenu : MonoBehaviour
{
    public GameObject pet1;
    public GameObject pet2;
    public GameObject player;
    public Text petLoot;
    public Text equipementLoot;
    public Image backgroundImage;

    private TeamData teamData;
    private List<EndMenuViewModel> endMenuHUDs = new List<EndMenuViewModel>();
    private Planet planet;
    private AreaFromDB areaFromDB;
    private List<Planet> planets;

    void Start()
    {
        planets = LoadDataFromJson.LoadAreaData();
        planet = planets.First(p => p.name == LoadSceneData.planet.name);
        areaFromDB = planet.areas.First(a => a.name == LoadSceneData.area.name);

        backgroundImage.sprite = LoadFromResourcesService.areaSprite(areaFromDB.name);

        teamData = LoadDataFromJson.LoadPlayerData();

        addToHUD(player, teamData.Player);
        addToHUDIfExists(teamData.Pets);

        foreach (EndMenuViewModel hud in endMenuHUDs)
        {
            fillMenuHUD(hud);
            StartCoroutine(updateXPBarCoroutine(hud));
        }

        if (!string.IsNullOrEmpty(LoadSceneData.petLoot))
        {
            petLoot.text = LoadSceneData.petLoot;
            teamData.ReservePets.Add(GenerateCharacterService.generateCharacter(LoadSceneData.petLoot, LoadSceneData.petLootLevel, null, areaFromDB.tier));
        }

        if (!string.IsNullOrEmpty(LoadSceneData.equipementLoot) && areaFromDB.done == false)
        {
            equipementLoot.text = LoadSceneData.equipementLoot;
            Equipement item = GenerateEquipementService.generate(LoadSceneData.equipementLoot);
            teamData.ReserveEquipements.Add(item);
        }
    }

    private void addToHUD(GameObject character, CharacterFromDB characterFromDB)
    {
        character.SetActive(true);
        EndMenuViewModel endMenuHUD = new EndMenuViewModel();
        endMenuHUD.portrait = character.transform.GetChild(0).gameObject.transform.GetChild(0)
            .gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
        endMenuHUD.xpBar = character.transform.GetChild(1).GetComponent<Image>();
        endMenuHUD.level = character.transform.GetChild(2).GetComponent<Text>();
        endMenuHUD.characterFromDB = characterFromDB;
        endMenuHUDs.Add(endMenuHUD);
    }

    private void addToHUDIfExists(List<CharacterFromDB> pets)
    {
        if (pets.Count > 0)
        {
            addToHUD(pet1, teamData.Pets[0]);
        }
        if (pets.Count > 1)
        {
            addToHUD(pet2, teamData.Pets[1]);
        }
    }

    private void fillMenuHUD(EndMenuViewModel hud)
    {
        hud.portrait.sprite = LoadFromResourcesService.monsterSprite(hud.characterFromDB.name);
        hud.level.text = hud.characterFromDB.level.ToString();
        hud.xpBar.fillAmount = hud.characterFromDB.currentExperience / hud.characterFromDB.experienceForNextLevel;
    }

    public void quit()
    {
        CharacterFromDB player = GenerateCharacterService.generateCharacter(teamData.Player.name, teamData.Player.level, teamData.Player);
        player.currentExperience = endMenuHUDs[0].characterFromDB.currentExperience;
        SaveToJson.savePlayer(player);

        for (int i = 0; i < teamData.Pets.Count; i++)
        {
            teamData.Pets[i] = GenerateCharacterService.generateCharacter(teamData.Pets[i].name, teamData.Pets[i].level, teamData.Pets[i]);
            teamData.Pets[i].currentExperience = endMenuHUDs[i + 1].characterFromDB.currentExperience;
        }
        SaveToJson.savePets(teamData.Pets);
        SaveToJson.saveReservePets(teamData.ReservePets);
        SaveToJson.saveReserveEquipements(teamData.ReserveEquipements);

        areaFromDB.done = true;
        if(areaFromDB.tier < 3 )
        {
            areaFromDB.tier += 1;
        }        
        if(planet.areas.All(p=>p.done))
        {
            planet.done = true;
        }
        SaveToJson.saveArea(planets);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator updateXPBarCoroutine(EndMenuViewModel hud)
    {

        List<CharacterFromDB> enemies = LoadSceneData.enemies.SelectMany(c => c).ToList();
        hud.xpBar.fillAmount = (float)hud.characterFromDB.currentExperience / hud.characterFromDB.experienceForNextLevel;

        int totalExp = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            totalExp += enemies[i].experienceGiven;
        }
        for (int i = 0; i < LoadSceneData.bosses.Count; i++)
        {
            totalExp += LoadSceneData.bosses[i].experienceGiven;
        }

        for (int i = 0; i < 10; i++)
        {
            yield return updateXPBarWithOneEnemi(hud.characterFromDB, (int)Math.Ceiling((float)totalExp / 10), hud.xpBar, hud.level);
        }
    }

    IEnumerator updateXPBarWithOneEnemi(CharacterFromDB characterFromDB, int experienceGiven, Image xpBar, Text level)
    {
        float amoutCalculatedWithEnemy = xpBar.fillAmount + (float)experienceGiven / characterFromDB.experienceForNextLevel;
        if (amoutCalculatedWithEnemy >= 1)
        {
            characterFromDB.level += 1;
            characterFromDB = GenerateCharacterService.generateCharacter(characterFromDB.name, characterFromDB.level, characterFromDB);
            level.text = characterFromDB.level.ToString();
            experienceGiven = experienceGiven - (int)((1 - xpBar.fillAmount) * characterFromDB.experienceForNextLevel);
            xpBar.fillAmount = 0;
            yield return updateXPBarWithOneEnemi(characterFromDB, experienceGiven, xpBar, level);
        }
        else
        {
            yield return new WaitForSeconds(0.1f);
            characterFromDB.currentExperience += experienceGiven;
            xpBar.fillAmount += (float)experienceGiven / characterFromDB.experienceForNextLevel;
        }
    }

    public class EndMenuViewModel
    {
        public Image portrait;
        public Image xpBar;
        public Text level;
        public CharacterFromDB characterFromDB;
    }
}
