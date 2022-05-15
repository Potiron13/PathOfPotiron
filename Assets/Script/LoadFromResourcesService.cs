using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LoadFromResourcesService
{    
    public static List<T> loadListFromRessources<T>(string name)
    {
        TextAsset jsonTextFile = Resources.Load<TextAsset>("initialData/" + name);
        T[] array = JsonHelper.FromJson<T>(jsonTextFile.text);
        return array.ToList();
    }

    public static GameObject prefab(string name)
    {
        return (GameObject)Resources.Load(name, typeof(GameObject));
    }
    
    public static Sprite sprite(string name)
    {
        return (Sprite)Resources.Load(name, typeof(Sprite));
    }    
        
    public static Sprite loadingScreenSprite(string name)
    {
        return (Sprite)Resources.Load("loadingScreens/sprites/" + name, typeof(Sprite));
    }    
            
    public static Sprite cursorSprite(string name)
    {
        return (Sprite)Resources.Load("cursor/" + name, typeof(Sprite));
    }    

    public static Sprite areaSprite(string name)
    {
        return (Sprite)Resources.Load("areas/sprites/" + name, typeof(Sprite));
    }

    public static Sprite monsterSprite(string name)
    {
        return (Sprite)Resources.Load("monsters/sprites/" + name, typeof(Sprite));
    }

    public static GameObject monsterPrefab(string name)
    {
        return (GameObject)Resources.Load("monsters/prefabs/" + name, typeof(GameObject));
    }    
    
    public static GameObject lootPrefab(string name)
    {
        return (GameObject)Resources.Load("loot/" + name, typeof(GameObject));
    }

    public static Sprite equipementSprite(string name)
    {
        return (Sprite)Resources.Load("equipements/" + name, typeof(Sprite));
    }    
    
    public static Sprite menuSprite(string name)
    {
        return (Sprite)Resources.Load("menu/" + name, typeof(Sprite));
    }

    public static Sprite abilitySprite(string name)
    {
        return (Sprite)Resources.Load("abilities/sprites/" + name, typeof(Sprite));
    }
    public static GameObject abilityPrefab(string name)
    {
        return (GameObject)Resources.Load("abilities/prefabs/" + name, typeof(GameObject));
    }

    public static GameObject withType(ResourcesTypeEnum resourcesType, string prefabName)
    {
        GameObject prefab;
        switch (resourcesType)
        {
            case ResourcesTypeEnum.MONSTER:
                prefab = monsterPrefab(prefabName);
                break;
            case ResourcesTypeEnum.ABILITY:
                prefab = abilityPrefab(prefabName);
                break;
            case ResourcesTypeEnum.DEFAULT:
            default:
                prefab = (GameObject)Resources.Load(prefabName, typeof(GameObject));
                break;
        }
        return prefab;
    }

    public enum ResourcesTypeEnum
    {
        MONSTER,
        ABILITY,
        EQUIPEMENT,
        DEFAULT
    }
}
