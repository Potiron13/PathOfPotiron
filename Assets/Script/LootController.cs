using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootController : MonoBehaviour
{
    public string lootId;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "player")
        {
            List<CraftingMaterial> craftingMaterials = LoadDataFromJson.LoadCraftingMaterials();
            CraftingMaterial craftingMaterial = craftingMaterials.First(c => c.id == lootId);
            craftingMaterial.count += 1;
            SaveToJson.saveCraftingMaterials(craftingMaterials);
            GameObject damageHolderPrefab = LoadFromResourcesService.prefab("DamagePopUp");
            GameObject damageHolder = Instantiate(damageHolderPrefab, gameObject.transform.position, Quaternion.identity, gameObject.transform);
            Destroy(HUDService.displayText(craftingMaterial.name, gameObject.transform.position, damageHolder), 1f);
            Destroy(gameObject, 1f);
        }
    }
}
