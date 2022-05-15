using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadCrystalSupport : MonoBehaviour
{
    public GameObject abilityDetail;
    public GameObject crystalDetail;
    public GameObject absorbDetail;
    public List<GameObject> abilitiesToAbsorbGO;
    public PanelManager menuManager;
    public Animator animator;
    public LoadTeamMenu loadTeamMenu;
    private float yDelta = -25f;

    public void init(int characterIndex, int abilityIndex)
    {
        destroyChilds(crystalDetail);
        destroyChilds(abilityDetail);
        absorbDetail.SetActive(false);
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

        GameObject abilityDetailPrefab = LoadFromResourcesService.prefab("AbilityDetail");
        GameObject abilityDetailGO = Instantiate(abilityDetailPrefab, abilityDetail.transform.position, Quaternion.identity, abilityDetail.transform);

        GetValueTextGO(abilityDetailGO, 0).text = character.abilities[abilityIndex].name;
        GetValueTextGO(abilityDetailGO, 1).text = character.abilities[abilityIndex].type;
        GetValueTextGO(abilityDetailGO, 2).text = character.abilities[abilityIndex].tier.ToString();
        GetValueTextGO(abilityDetailGO, 3).text = character.abilities[abilityIndex].range.ToString();
        GetValueTextGO(abilityDetailGO, 4).text = character.abilities[abilityIndex].cooldown.ToString();

        var crystalLines = new List<string>();
        List<string> supportCrystalsIds = character.abilities[abilityIndex].supportCrystals;
        List<SupportCrystal> supportCrystals = LoadDataFromJson.LoadSupportCrystals();

        GameObject crystalDetailPrefab = LoadFromResourcesService.prefab("CrystalDetail");
        GameObject crystalDetailGO = Instantiate(crystalDetailPrefab, crystalDetail.transform.position, Quaternion.identity, crystalDetail.transform);

        for (int i = 0; i < 2; i++)
        {            
            string crystalName = supportCrystalsIds.Count > i ? supportCrystals.First(c => c.id == supportCrystalsIds[i]).name : "none";
            GetValueTextGO(crystalDetailGO, i).text = crystalName;
        }

        if (characterIndex != 0)
        {
            absorbDetail.SetActive(true);
            CharacterFromDB player = teamData.Player;
            for (int i = 0; i < 4 ; i++)
            {
                absorbDetail.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject.SetActive(true);
                fillAbsorbMenu(characterIndex, abilityIndex, character, player, i);
            }
        }
    }

    private void fillAbsorbMenu(int characterIndex, int abilityIndex, CharacterFromDB character, CharacterFromDB player, int i)
    {
        abilitiesToAbsorbGO[i].GetComponent<Text>().text = i.ToString();
        string imageAbilityName = i < player.abilities.Count ? player.abilities[i].name : "EmptyImage";
        abilitiesToAbsorbGO[i].transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LoadFromResourcesService.abilitySprite(imageAbilityName);
        Button absorbButton = abilitiesToAbsorbGO[i].transform.GetChild(0).gameObject.GetComponent<Button>();
        absorbButton.onClick.RemoveAllListeners();
        absorbButton.onClick.AddListener(() =>
        {
            AbsrobService.abosrb(player, character, abilityIndex, i, characterIndex);
            loadTeamMenu.clearMenu();
            menuManager.CloseCurrent();
            menuManager.OpenPanel(animator);
        });
    }

    private Text GetValueTextGO(GameObject PrefabGO, int index)
    {
        return PrefabGO.transform.GetChild(index).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>();
    }   
    
    private void destroyChilds(GameObject toCleanGO)
    {
        foreach (Transform child in toCleanGO.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void generateLine(string libelle, string value, int lineIndex, GameObject parentGO, float[] xCoordinates)
    {
        float y = lineIndex * yDelta;
        GameObject libelleGO = createOrFindGameObjectInHUD("libelle" + libelle, parentGO, xCoordinates[0], y);
        addTextComponent(libelleGO, libelle);
        GameObject valueGO = createOrFindGameObjectInHUD("value" + libelle, parentGO, xCoordinates[1], y);
        addTextComponent(valueGO, value);
    }

    private void generateAbilityAbsorbButton(GameObject parentGO, CharacterFromDB pet, int petAbilityIndex, CharacterFromDB player, int playerAbilityIndex, int characterIndex, float x, float y)
    {
        GameObject CrystalSupportGO = GameObject.Find("absorb" + playerAbilityIndex);
        if (CrystalSupportGO == null)
        {
            GameObject CrystalSupportGOPref = LoadFromResourcesService.prefab("Ability Button");
            CrystalSupportGO = Instantiate(CrystalSupportGOPref, new Vector3(x, y, 0), Quaternion.identity, parentGO.transform);
            CrystalSupportGO.name = "absorb" + playerAbilityIndex;

            RectTransform rectTransform = CrystalSupportGO.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition = new Vector3(x, y, 0);
        }
                
        CrystalSupportGO.transform.parent = parentGO.transform;
        CrystalSupportGO.GetComponent<Image>().sprite = LoadFromResourcesService.abilitySprite(player.abilities[playerAbilityIndex].name);
        Button button = CrystalSupportGO.GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            AbsrobService.abosrb(player, pet, petAbilityIndex, playerAbilityIndex, characterIndex);
            loadTeamMenu.clearMenu();
            menuManager.CloseCurrent();            
            menuManager.OpenPanel(animator);
        });
    }

    private GameObject createGameObject(string name, GameObject parentGO, float x, float y)
    {
        GameObject newGameObject = new GameObject(name);
        newGameObject.transform.parent = parentGO.transform;
        addRectTransform(newGameObject, x, y);
        return newGameObject;
    }

    private GameObject createOrFindGameObjectInHUD(string name, GameObject parentGO, float x, float y)
    {
        Transform childTransform = parentGO.transform.Find(name);
        if (childTransform != null)
        {
            return childTransform.gameObject;
        }
        return createGameObject(name, parentGO, x, y);
    }
    private void addTextComponent(GameObject textGO, string textToDisplay)
    {
        Text text = textGO.GetComponent<Text>() != null ? textGO.GetComponent<Text>() : textGO.AddComponent<Text>();
        text.text = textToDisplay;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
    }

    private void addRectTransform(GameObject newGameObject, float x, float y)
    {
        RectTransform rectTransform = newGameObject.AddComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition = new Vector3(x, y, 0);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 200);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
        
    }

}
