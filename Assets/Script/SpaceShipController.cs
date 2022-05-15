using System.Linq;
using UnityEngine;

public class SpaceShipController : MonoBehaviour
{
    public Animator mainMenuAnimator;
    public PanelManager menuManager;
    public PlanetsManager planetsManager;

    private void Update()
    {
        int x = Random.Range(0, 25);
        int y = Random.Range(0, 25);
        int z = Random.Range(0, 25);
        transform.Rotate(x * Time.deltaTime, y * Time.deltaTime, z * Time.deltaTime);
    }

    private void OnMouseEnter()
    {
        gameObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
    private void OnMouseExit()
    {
        gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    }

    private void OnMouseDown()
    {
        planetsManager.hidePlanets();
        menuManager.OpenPanel(mainMenuAnimator);        
    }
}
