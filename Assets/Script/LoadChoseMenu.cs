using UnityEngine;
using UnityEngine.UI;

public class LoadChoseMenu : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject previousButton;
    public int minIndex;
    public Image pet1Image;
    public Image pet2Image;
    public Text pet1Level;
    public Text pet2Level;

    private GameObject dragPanel;
    int[][] dragBoxCoordonates =
        {
        new int[] { -250, 150, 0 },
        new int[] { -100, 150, 0 },
        new int[] { 50, 150, 0 },
        new int[] { 200, 150, 0 },
        new int[] { -250, 50, 0 },
        new int[] { -100, 50, 0 },
        new int[] { 50, 50, 0 },
        new int[] { 200, 50, 0 }
    };
    // Start is called before the first frame update
    public void init()
    {        
        dragPanel = GameObject.Find("DragPanel");
        minIndex = 0;
        ClearDragPanel();        
        InitDragPanel();
        InitDropPanel();
    }

    public void InitDropPanel()
    {
        ClearDropPanel();
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        if(teamData.Pets.Count > 0)
        {
            fillDropBox(teamData.Pets[0], pet1Image, pet1Level);
        }
        if(teamData.Pets.Count > 1)
        {
            fillDropBox(teamData.Pets[1], pet2Image, pet2Level);
        }
    }

    public void InitDragPanel()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();        
        for (int i = 0; i < 8; i++)
        {
            if (teamData.ReservePets.Count > i + minIndex)
            {
                generateDragBox(dragPanel, transform.TransformPoint(dragBoxCoordonates[i][0], dragBoxCoordonates[i][1], dragBoxCoordonates[i][2]), teamData.ReservePets[i + minIndex]);
            }
        }

        nextButton.SetActive(teamData.ReservePets.Count > minIndex + 8);
        previousButton.SetActive(minIndex != 0);
    }

    public void HandleNextButton()
    {
        minIndex += 8;
        ClearDragPanel();
        InitDragPanel();
    }
    
    public void HandlePreviousButton()
    {
        minIndex -= 8;
        ClearDragPanel();
        InitDragPanel();
    }

    public void ClearDragPanel()
    {
        foreach (Transform child in dragPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearDropPanel()
    {
        clearDropBox(pet1Image, pet1Level);
        clearDropBox(pet2Image, pet2Level);
    }

    private void clearDropBox(Image petImage, Text petLevel)
    {
        petImage.sprite = LoadFromResourcesService.sprite("EmptyImage");
        petLevel.text = "";
    }

    private void fillDropBox(CharacterFromDB pet, Image petImage, Text petLevel)
    {        
        petImage.sprite = LoadFromResourcesService.monsterSprite(pet.name);
        petLevel.text = pet.level.ToString();
        petImage.gameObject.GetComponent<DropMe>().targetPetId = pet.id;
    }

    private void generateDragBox(GameObject dragPanel, Vector3 position, CharacterFromDB character)
    {
        GameObject dragBoxPrefab = LoadFromResourcesService.prefab("Drag Box");
        GameObject dragBox = Instantiate(dragBoxPrefab, position, Quaternion.identity, dragPanel.transform);
        GameObject dragImage = dragBox.transform.GetChild(0).gameObject;
        Image image = dragImage.GetComponent<Image>();
        dragImage.GetComponent<DragMe>().sourcePetId = character.id;

        Text level = dragImage.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        image.sprite = LoadFromResourcesService.monsterSprite(character.name);
        level.text = character.level.ToString();

        Text tier = dragImage.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        tier.text = character.tier.ToString();
    }
}
