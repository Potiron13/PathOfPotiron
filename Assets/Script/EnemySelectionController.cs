using UnityEngine;

public class EnemySelectionController : MonoBehaviour
{
    public GameObject arrowGO;
    public PlayerControl playerControl;

    private void OnMouseEnter()
    {
        GameObject arrowPrefab = LoadFromResourcesService.prefab("Arrow");
        Vector3 arrowPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 2, gameObject.transform.position.z);
        arrowGO = Instantiate(arrowPrefab, arrowPosition, Quaternion.Euler(180, 0, 0), gameObject.transform);        
    }

    private void OnMouseExit()
    {
        if (arrowGO != null && playerControl.target == null)
        {
            Destroy(arrowGO);
        }
    }
}
