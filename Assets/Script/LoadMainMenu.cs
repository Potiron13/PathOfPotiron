using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadMainMenu : MonoBehaviour
{
    public Texture2D cursor;

    void Start()
    {
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
        copyInitialData<CharacterAbility>("abilities");
        copyInitialData<Planet>("areas");
        copyInitialData<CraftingMaterial>("craftingMaterials");
        copyInitialData<FusionMatriceLine>("fusionMatrice");
        copyInitialData<StandardEquipement>("standardEquipements");
        copyInitialData<StandardPet>("standardPets");
        copyInitialData<SupportCrystal>("supportCrystals");
        if (!File.Exists(Application.persistentDataPath + "/player.json"))
        {
            SaveToJson.savePlayer(GenerateCharacterService.generateCharacter("Potiron", 5, null));
        }
    }

    private static void copyInitialData<T>(string name)
    {
        if (!File.Exists(Application.persistentDataPath + "/" + name + ".json"))
        {
            List<T> characterAbilities = LoadFromResourcesService.loadListFromRessources<T>(name);
            SaveToJson.saveList(characterAbilities, "/" + name + ".json");
        }
    }
}
