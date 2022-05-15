using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveToJson
{            
    public static void savePlayer(CharacterFromDB player)
    {
        string playerString = JsonUtility.ToJson(player, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/player.json", playerString);
    }

    public static void savePets(List<CharacterFromDB> pets)
    {
        string petsString = JsonHelper.ToJson(pets.ToArray(), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/pets.json", petsString);
    }

    public static void saveReservePets(List<CharacterFromDB> reservePets)
    {
        string reservePetsString = JsonHelper.ToJson(reservePets.ToArray(), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/reservePets.json", reservePetsString);
    }
    
    public static void savePetsToFuse(List<CharacterFromDB> petsToFuse)
    {
        string petsToFuseAsString = JsonHelper.ToJson(petsToFuse.ToArray(), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/petsToFuse.json", petsToFuseAsString);
    }

    public static void saveReserveEquipements(List<Equipement> reserveEquipements)
    {
        string reserveEquipementsString = JsonHelper.ToJson(reserveEquipements.ToArray(), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/reserveEquipements.json", reserveEquipementsString);
    }

    public static void saveArea(List<Planet> planets)
    {
        string planetsString = JsonHelper.ToJson(planets.ToArray(), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/areas.json", planetsString);
    }

    public static void saveCraftingMaterials(List<CraftingMaterial> craftingMaterials)
    {
        saveList(craftingMaterials, "/craftingMaterials.json");
    }
    
    public static void saveList<T>(List<T> list, string path)
    {
        string arrayAsString = JsonHelper.ToJson(list.ToArray(), true);
        System.IO.File.WriteAllText(Application.persistentDataPath + path, arrayAsString);
    }
}
