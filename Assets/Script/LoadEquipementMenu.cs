using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LoadEquipementMenu : MonoBehaviour
{
    public GameObject nextButton;
    public GameObject previousButton;
    public int minIndex;
    public List<Image> dropImages;
    public List<Text> equipementNames;
    public List<GameObject> equipementStats;

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
    void Start()
    {        
        dragPanel = GameObject.Find("DragPanel");
        minIndex = 0;
        InitDragPanel();
        InitDropPanel();
    }

    public void InitDropPanel()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();

        List<Equipement> equipements = new List<Equipement> {
            teamData.Player.weapon,
            teamData.Player.armor,
            teamData.Player.gem
        };

        for (int i = 0; i < equipements.Count; i++)
        {
            fillDropBox(equipements[i], dropImages[i], equipementNames[i], equipementStats[i]);
        }
    }

    public void InitDragPanel()
    {
        TeamData teamData = LoadDataFromJson.LoadPlayerData();
        List<Equipement> equipements = teamData.ReserveEquipements.Where(e => !string.IsNullOrEmpty(e.id)).ToList();
        for (int i = 0; i < 8; i++)
        {
            if (equipements.Count > i + minIndex)
            {
                generateDragBox(dragPanel, transform.TransformPoint(dragBoxCoordonates[i][0], dragBoxCoordonates[i][1], dragBoxCoordonates[i][2]), equipements[i + minIndex]);
            }
        }

        nextButton.SetActive(equipements.Count > minIndex + 8);
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

    private void fillDropBox(Equipement equipement, Image equipementImage, Text equipementName, GameObject equipementStat)
    {        
        if(!string.IsNullOrEmpty(equipement.id))
        {
            equipementImage.sprite = LoadFromResourcesService.equipementSprite(equipement.name);
            equipementImage.gameObject.GetComponent<DropMeEquipement>().targetEquipementId = equipement.id;
            equipementName.text = equipement.name;
            generateEquipementDescription(equipement, equipementStat);
        }
    }

    private void generateEquipementDescription(Equipement equipement, GameObject equipementStat)
    {
            displayStat(0, equipement.hpValue.ToString(), equipementStat);
            displayStat(1, equipement.armorValue.ToString(), equipementStat);
            displayStat(2, equipement.strengthValue.ToString(), equipementStat);
            displayStat(3, equipement.magicValue.ToString(), equipementStat);
    }

    private void displayStat(int index, string stat, GameObject equipementStat)
    {
        GameObject wrapperGO = equipementStat.transform.GetChild(index).gameObject;
        wrapperGO.transform.GetChild(0).gameObject.GetComponent<Text>().text = stat;
    }

    private void generateDragBox(GameObject dragPanel, Vector3 position, Equipement equipement)
    {
        GameObject dragBoxPrefab = LoadFromResourcesService.prefab("Drag Box Equipement");
        GameObject dragBox = Instantiate(dragBoxPrefab, position, Quaternion.identity, dragPanel.transform);
        GameObject dragImage = dragBox.transform.GetChild(0).gameObject;
        Image image = dragImage.GetComponent<Image>();        
        dragImage.GetComponent<DragMeEquipement>().sourceEquipementId = equipement.id;
        dragImage.GetComponent<DragMeEquipement>().sourceEquipementType = equipement.type;
        image.sprite = LoadFromResourcesService.equipementSprite(equipement.name);        
    }
}
