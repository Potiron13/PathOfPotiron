using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadFuseMenu : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject previousButton;
    public GameObject fuseButton;
    public int minIndex;
    public List<Image> petImages;
    public List<Text> petLevels;  
    public Image resultImage;

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
        dragPanel = GameObject.Find("DragPanelFusion");
        minIndex = 0;
        ClearDragPanel();
        InitDragPanel();
        InitDropPanel();        
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
        fuseButton.SetActive(teamData.PetsToFuse.Count == 2);
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
        Sprite emptyImage = LoadFromResourcesService.sprite("EmptyImage");
        for (int i = 0; i < petImages.Count; i++)
        {
            petImages[i].sprite = emptyImage;
            petLevels[i].text = "";
            
        }
        resultImage.sprite = emptyImage;
    }

    public void fillResultPanel()
    {
        CharacterFromDB result = FusionService.fusePets(LoadDataFromJson.LoadPlayerData().PetsToFuse);
        resultImage.sprite = LoadFromResourcesService.monsterSprite(result.name);
    }

    private void generateDragBox(GameObject dragPanel, Vector3 position, CharacterFromDB character)
    {
        GameObject dragBoxPrefab = LoadFromResourcesService.prefab("Drag Box Fuse");
        GameObject dragBox = Instantiate(dragBoxPrefab, position, Quaternion.identity, dragPanel.transform);
        GameObject dragImage = dragBox.transform.GetChild(0).gameObject;
        Image image = dragImage.GetComponent<Image>();
        dragImage.GetComponent<DragMeFusion>().sourcePetId = character.id;

        Text level = dragImage.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        image.sprite = LoadFromResourcesService.monsterSprite(character.name);
        level.text = character.level.ToString();

        Text tier = dragImage.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>();
        tier.text = character.tier.ToString();
    }

    public void InitDropPanel()
    {
        List<CharacterFromDB> petsToFuse = LoadDataFromJson.LoadPlayerData().PetsToFuse;
        for (int i = 0; i < petsToFuse.Count; i++)
        {
            fillDropBox(petsToFuse[i], petImages[i], petLevels[i]);
        }       
        if(petsToFuse.Count >= 2)
        {
            fillResultPanel();
        }
    }

    public void CancelFusion()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        List<CharacterFromDB> petsToFuse = teamData.PetsToFuse;
        teamData.ReservePets.AddRange(petsToFuse);
        SaveToJson.saveReservePets(teamData.ReservePets);
        SaveToJson.savePetsToFuse(new List<CharacterFromDB>());
        ClearDragPanel();
        InitDragPanel();
        ClearDropPanel();
    }

    private void fillDropBox(CharacterFromDB pet, Image petImage, Text petLevel)
    {
        petImage.sprite = LoadFromResourcesService.monsterSprite(pet.name);
        petLevel.text = pet.level.ToString();
        petImage.gameObject.GetComponent<DropMeFusion>().targetPetId = pet.id;
    }
}
