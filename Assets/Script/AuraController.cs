using System.Collections.Generic;
using UnityEngine;

public class AuraController : MonoBehaviour
{
    public List<CharacterAbility> auras;
    public LoadHUDForBattle loadHUDForBattle;

    void Start()
    {
        for (int i = 0; i < auras.Count; i++)
        {
            GameObject auraGO = new GameObject("aura" + gameObject.name + i);
            AuraUnitController auraUnitController = auraGO.AddComponent<AuraUnitController>();
            auraUnitController.aura = auras[i];
            auraUnitController.loadHUDForBattle = loadHUDForBattle;
            auraGO.transform.parent = gameObject.transform;
            auraGO.tag = gameObject.tag;

            auraUnitController.transform.parent = auraGO.transform;
            auraUnitController.transform.position = Vector3.zero;
            auraUnitController.transform.localPosition = Vector3.zero;

            SphereCollider sphereCollider = auraGO.AddComponent<SphereCollider>();
            sphereCollider.radius = auras[i].range;
            sphereCollider.isTrigger = true;
        }
    }


}
