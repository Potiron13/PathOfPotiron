using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadTeamMenu : MonoBehaviour
{
    public List<GameObject> pets;
    public GameObject player;
    public Animator crystalSupportAnimator;
    public Animator detailAnimator;
    public LoadDetailMenu loadDetailMenu;    
    public LoadCrystalSupport loadCrystalSupport;
    public GameObject detailWindow;
    public GameObject abilityWindow;

    private float[] xCoordinate = new float[] { -125, -75, -25, 25, 50, 75, 100, 125 };
    private float[] yCoordinate = new float[] { 30, 0, -30 };
    private GameObject bodyGO;
    private List<GameObject> toRescaleGOs;

    public void initMenu()
    {
        toRescaleGOs = new List<GameObject>();
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        bodyGO = GameObject.Find("Body");
        List<CharacterFromDB> characters = teamData.Pets;
        characters.Insert(0, teamData.Player);

        for (int i = 0; i < characters.Count; i++)
        {
            generateMenuLine(i, bodyGO, characters[i], xCoordinate, yCoordinate[i]);
        }
    }

    public void HandleBackButton()
    {
        clearMenu();
    }

    public void clearMenu()
    {
        foreach (Transform child in bodyGO.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void generateMenuLine(int lineIndex, GameObject bodyGO, CharacterFromDB character, float[] xCoordinate, float y)
    {
        GameObject lineGO = generateGameObjectInHUD("line" + lineIndex, bodyGO, 0, y, 50, 50);
        toRescaleGOs.Add(lineGO);

        GameObject portaitGO = generateGameObjectInHUD("line" + lineIndex + "Portrait", lineGO, xCoordinate[0], y, 50, 50);
        portaitGO.AddComponent<Image>().sprite = LoadFromResourcesService.sprite("PortraitFrame");
        Button button = portaitGO.AddComponent<Button>();
        button.onClick.AddListener(() =>
        {
            rescale();
            changeScale(lineGO, 1.2f, 1.2f);
            abilityWindow.SetActive(false);
            detailWindow.SetActive(true);
            loadDetailMenu.init(lineIndex);
        });
        
        GameObject maskGO = generateGameObjectInHUD("line" + lineIndex + "Mask", portaitGO, 0, 0, 40, 40);
        maskGO.AddComponent<Image>().sprite = LoadFromResourcesService.sprite("ProtraitMask");
        maskGO.AddComponent<Mask>();

        GameObject imageGO = generateGameObjectInHUD("line" + lineIndex + "Image", maskGO, 0, 0, 40, 40);        
        imageGO.AddComponent<Image>().sprite = LoadFromResourcesService.monsterSprite(character.name);

        GameObject levelLibelleGO = generateGameObjectInHUD("line" + lineIndex + "LevelLibelle", lineGO, xCoordinate[1], y + 10, 50, 20);
        addTextComponent(levelLibelleGO, "Lvl.");
        GameObject levelValueGO = generateGameObjectInHUD("line" + lineIndex + "LevelValue", lineGO, xCoordinate[1] + 25, y + 10, 50, 20);
        addTextComponent(levelValueGO, character.level.ToString());

        GameObject heartGO = generateGameObjectInHUD("line" + lineIndex + "Heart", lineGO, xCoordinate[1] - 15, y - 10, 20, 20);
        heartGO.AddComponent<Image>().sprite = LoadFromResourcesService.menuSprite("Heart");
        GameObject maxHealthGO = generateGameObjectInHUD("line" + lineIndex + "MaxHealth", lineGO, xCoordinate[1] + 25, y - 12, 50, 20);
        addTextComponent(maxHealthGO, character.health.ToString());

        for (int i = 0; i < character.abilities.Count; i++)
        {
            generateAbilityButton(lineGO, lineIndex, character, i, xCoordinate[3 + i], y);
        }
    }

    private void changeScale(GameObject GO, float x, float y)
    {
        GO.GetComponent<RectTransform>().localScale = new Vector3(x, y);
    }

    private void rescale()
    {
        foreach (GameObject line in toRescaleGOs)
        {
            line.GetComponent<RectTransform>().localScale = new Vector3(1, 1);
        }
    }

    private void generateAbilityButton(GameObject parentGO, int lineIndex, CharacterFromDB character, int abilityIndex, float x, float y)
    {
        GameObject CrystalSupportGOPref = LoadFromResourcesService.prefab("Ability Button");
        GameObject CrystalSupportGO = Instantiate(CrystalSupportGOPref, new Vector3(x,y,0), Quaternion.identity, parentGO.transform);
        toRescaleGOs.Add(CrystalSupportGO);

        RectTransform rectTransform = CrystalSupportGO.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(x, y, 0);
        rectTransform.localScale = Vector3.one;

        CrystalSupportGO.transform.parent = parentGO.transform;        
        CrystalSupportGO.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LoadFromResourcesService.abilitySprite(character.abilities[abilityIndex].name);
        Button button = CrystalSupportGO.GetComponent<Button>();
        button.onClick.AddListener(() =>
            {
                detailWindow.SetActive(false);
                abilityWindow.SetActive(true);
                rescale();
                changeScale(CrystalSupportGO, 1.2f, 1.2f);
                loadCrystalSupport.init(lineIndex, abilityIndex);
            }
        );                
    }

    private void addTextComponent(GameObject textGO, string textToDisplay)
    {
        Text text = textGO.AddComponent<Text>();
        text.text = textToDisplay;
        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
    }

    private GameObject generateGameObjectInHUD(string name, GameObject parentGO, float x, float y, int xBorder, int yBorder)
    {
        GameObject newGameObject = new GameObject(name);
        newGameObject.transform.parent = parentGO.transform;
        addRectTransform(newGameObject, x, y, xBorder, yBorder);

        return newGameObject;
    }

    private void addRectTransform(GameObject newGameObject, float x, float y, int xBorder, int yBorder)
    {
        RectTransform rectTransform = newGameObject.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(x, y, 0);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xBorder);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, yBorder);
        rectTransform.localScale = Vector3.one;
    }
}
