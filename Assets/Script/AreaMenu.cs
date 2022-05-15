using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AreaMenu : MonoBehaviour
{
    public Planet planet;
    public GameObject nextButton;
    public GameObject previousButton;
    public int minIndex;
    public List<GameObject> panelAreas;
    public PlanetsManager planetsManager;
    public GameObject loadingScreen;
    public Slider slider;
    public GameObject areasWindow;
    public GameObject loadingCaption; 

    private AsyncOperation asyncOperation;

    void Update()
    {
        if(asyncOperation != null && asyncOperation.progress == 0.9f)
        {
            if(Input.anyKey)
                asyncOperation.allowSceneActivation = true;
            if (!loadingCaption.activeSelf)
                loadingCaption.SetActive(true);
        }    
    }

    public void init()
    {
        planetsManager.hidePlanets();
        for (int i = 0; i < panelAreas.Count; i++)
        {
            if(i == 0 || i > 0 && planet.areas.Count > i && planet.areas[i-1].done)
            {
                fillArea(i);
                panelAreas[i].SetActive(true);
            } else
            {
                panelAreas[i].SetActive(false);
            }
        }
    }

    private void fillArea(int i)
    {
        GameObject panel = panelAreas[i];
        AreaFromDB area = planet.areas[i];
        getChild(panel, 0).GetComponent<Text>().text = area.name;
        getChild(panel, 1).GetComponent<Image>().sprite = LoadFromResourcesService.areaSprite(area.name);
        getChild(getChild(getChild(panel, 2), 0), 0).GetComponent<Text>().text = area.petLoot;
        getChild(getChild(getChild(panel, 2), 1), 0).GetComponent<Text>().text = area.petLootLevel.ToString();
        getChild(getChild(getChild(panel, 2), 2), 0).GetComponent<Text>().text = area.tier.ToString();
        getChild(panel, 3).GetComponent<Button>().onClick.AddListener(() =>
        {
            LoadArea(area);
        });
    }

    private GameObject getChild(GameObject go, int childIndex)
    {
        return go.transform.GetChild(childIndex).gameObject;
    }

    public void LoadArea(AreaFromDB area)
    {
        LoadSceneData.planet = planet;
        LoadSceneData.area = area;       
        LoadSceneData.enemies = new List<List<CharacterFromDB>>();
        // pour chaque zone/pack
        for (int i = 0; i < area.packCount; i++)
        {
            List<CharacterFromDB> pack = new List<CharacterFromDB>();
            // pour chaque monstre dans un pack
            for (int j = 0; j < area.packSize; j++)
            {
                CharacterFromDB monster = GenerateCharacterService.generateCharacter(area.monsters[Random.Range(0, area.monsters.Count)], area.level + area.tier * 5, null, area.tier);
                pack.Add(monster);
            }
            LoadSceneData.enemies.Add(pack);
        }
        // boss
        LoadSceneData.bosses = new List<CharacterFromDB>();
        for (int j = 0; j < area.bosses.Count; j++)
        {
            LoadSceneData.bosses.Add(GenerateCharacterService.generateCharacter(area.bosses[j], area.level + area.tier * 5, null, area.tier));
        }
        // loot
        LoadSceneData.petLoot = area.petLoot;
        LoadSceneData.petLootLevel = area.petLootLevel;
        LoadSceneData.equipementLoot = area.equipementLoot;
        // config
        CharacterConfig.AbilityConfiguration config1 = new CharacterConfig.AbilityConfiguration();
        config1.buttonName = "Fire1";
        CharacterConfig.AbilityConfiguration config2 = new CharacterConfig.AbilityConfiguration();
        config2.buttonName = "Fire2";        
        CharacterConfig.AbilityConfiguration config3 = new CharacterConfig.AbilityConfiguration();
        config3.buttonName = "Fire3";       
        CharacterConfig.AbilityConfiguration config4 = new CharacterConfig.AbilityConfiguration();
        config4.buttonName = "Fire4";
        LoadSceneData.characterConfig.abilityConfigurations = new List<CharacterConfig.AbilityConfiguration>();
        LoadSceneData.characterConfig.abilityConfigurations.Add(config1);
        LoadSceneData.characterConfig.abilityConfigurations.Add(config2);
        LoadSceneData.characterConfig.abilityConfigurations.Add(config3);
        LoadSceneData.characterConfig.abilityConfigurations.Add(config4);
        StartCoroutine(loadAsyncScene(area));
    } 

    IEnumerator loadAsyncScene(AreaFromDB area)
    {
        areasWindow.SetActive(false);
        loadingScreen.GetComponent<Image>().sprite = LoadFromResourcesService.loadingScreenSprite(planet.loadingscreen);
        loadingScreen.SetActive(true);
        asyncOperation = SceneManager.LoadSceneAsync(area.name);
        asyncOperation.allowSceneActivation = false;
        while (!asyncOperation.isDone)
        {
            slider.value = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            yield return null;
        }
    }
}
