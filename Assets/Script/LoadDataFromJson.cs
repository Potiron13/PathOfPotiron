using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class LoadDataFromJson
{    
    public static TeamData LoadPlayerData()
    {
        TeamData teamData = new TeamData();
        string playerJson = readJsonFile("/player.json");
        string petsJson = readJsonFile("/pets.json");
        string reservePetsJson = readJsonFile("/reservePets.json");
        string petsToFuseJson = readJsonFile("/petsToFuse.json");
        string reserveEquipementJson = readJsonFile("/reserveEquipements.json");
        CharacterFromDB[] petsFromDB = JsonHelper.FromJson<CharacterFromDB>(petsJson);
        CharacterFromDB[] reservePetsFromDB = JsonHelper.FromJson<CharacterFromDB>(reservePetsJson);
        CharacterFromDB[] petsToFuseFromDB = JsonHelper.FromJson<CharacterFromDB>(petsToFuseJson);
        Equipement[] reserveEquipementFromDB = JsonHelper.FromJson<Equipement>(reserveEquipementJson);
        CharacterFromDB playerFromDB = JsonUtility.FromJson<CharacterFromDB>(playerJson);
        teamData.Player = playerFromDB;
        teamData.Pets = petsFromDB.ToList();
        teamData.ReservePets = reservePetsFromDB.ToList();
        teamData.PetsToFuse = petsToFuseFromDB.ToList();
        teamData.ReserveEquipements = reserveEquipementFromDB.ToList();
        
        return teamData;
    }

    public static List<CharacterAbility> LoadStandardAbilities()
    {
        string abilitiesJson = File.ReadAllText(Application.persistentDataPath + "/abilities.json");
        return JsonHelper.FromJson<CharacterAbility>(abilitiesJson).ToList();
    }

    public static List<Planet> LoadAreaData()
    {
        string areasJson = File.ReadAllText(Application.persistentDataPath + "/areas.json");
        Planet[] planets = JsonHelper.FromJson<Planet>(areasJson);
        return planets.ToList();
    }

    public static List<StandardPet> LoadStandardPets()
    {
        string standardPets = File.ReadAllText(Application.persistentDataPath + "/standardPets.json");
        StandardPet[] standardPetFromDB = JsonHelper.FromJson<StandardPet>(standardPets);
        return standardPetFromDB.ToList();
    }

    public static List<StandardEquipement> LoadStandardEquipements()
    {
        string standardEquipements = File.ReadAllText(Application.persistentDataPath + "/standardEquipements.json");
        StandardEquipement[] standardEquipementsFromDB = JsonHelper.FromJson<StandardEquipement>(standardEquipements);
        return standardEquipementsFromDB.ToList();
    }    

    public static List<FusionMatriceLine> LoadFusionMatrice() { 
        string fusionMatrice = File.ReadAllText(Application.persistentDataPath + "/fusionMatrice.json");
        FusionMatriceLine[] fusionMatriceFromDB = JsonHelper.FromJson<FusionMatriceLine>(fusionMatrice);
        return fusionMatriceFromDB.ToList();
    }    

    public static List<CraftingMaterial> LoadCraftingMaterials() {
        return LoadListFromFile<CraftingMaterial>("/craftingMaterials.json");
    }   

    public static List<SupportCrystal> LoadSupportCrystals() {
        return LoadListFromFile<SupportCrystal>("/supportCrystals.json");
    }    

    public static List<T> LoadListFromFile<T>(string path) { 
        string listAsString = File.ReadAllText(Application.persistentDataPath + path);
        T[] array = JsonHelper.FromJson<T>(listAsString);
        return array.ToList();
    }

    private static string readJsonFile(string path)
    {
        string filePath = Application.persistentDataPath + path;
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        return "";
    }

}

public class TeamData
{
    public CharacterFromDB Player { get; set; }
    public List<CharacterFromDB> Pets { get; set; }    
    public List<CharacterFromDB> ReservePets { get; set; }    
    public List<Equipement> ReserveEquipements { get; set; }
    public List<CharacterFromDB> PetsToFuse { get; set; }
}
