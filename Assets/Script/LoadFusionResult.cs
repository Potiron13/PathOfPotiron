using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadFusionResult : MonoBehaviour
{
    public Image petImage;
    public Text petLevel;
    public Text petHealth;
    public void fustePets()
    {
        List<CharacterFromDB> petsToFuse = LoadDataFromJson.LoadPlayerData().PetsToFuse;
        CharacterFromDB fusedPet = FusionService.fusePets(petsToFuse);
        petImage.sprite = LoadFromResourcesService.monsterSprite(fusedPet.name);
        petLevel.text = fusedPet.level.ToString();
        petHealth.text = fusedPet.health.ToString();

        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        List<CharacterFromDB> reservePets = teamData.ReservePets.ToList();
        reservePets.Add(fusedPet);
        SaveToJson.saveReservePets(reservePets);
        SaveToJson.savePetsToFuse(new List<CharacterFromDB>());

        LoadFuseMenu loadFuseMenu = GameObject.Find("FusionMenu").GetComponent<LoadFuseMenu>();
        loadFuseMenu.ClearDropPanel();
        loadFuseMenu.ClearDragPanel();
        loadFuseMenu.InitDragPanel();
    }


}
