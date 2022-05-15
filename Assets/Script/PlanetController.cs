using System.Linq;
using UnityEngine;

public class PlanetController : MonoBehaviour
{
    public Animator continuAnimator;
    public PanelManager menuManager;
    public AreaMenu areaMenu;

    private void Update()
    {
        int x = Random.Range(0, 25);
        int y = Random.Range(0, 25);
        int z = Random.Range(0, 25);
        transform.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }

    private void OnMouseEnter()
    {
        gameObject.transform.localScale = new Vector3(2, 2, 2);
    }
    private void OnMouseExit()
    {
        gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnMouseDown()
    {
        areaMenu.planet = LoadDataFromJson.LoadAreaData().First(p => p.name == gameObject.name);        
        menuManager.OpenPanel(continuAnimator);
        areaMenu.init();
    }
}
