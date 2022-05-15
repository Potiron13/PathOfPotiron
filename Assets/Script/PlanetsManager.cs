using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetsManager : MonoBehaviour
{
    public List<GameObject> planets;
    public GameObject spaceship;

    public void showPlayablePlanets()
    {
        List<Planet> planetsFromDB = LoadDataFromJson.LoadAreaData();
        for (int i = 0; i < planetsFromDB.Count; i++)
        {
            if(i == 0 || i > 0 && planetsFromDB[i-1].done)
            {
                planets.First(p => p.name == planetsFromDB[i].name).SetActive(true);
            }
        }
    }

    public void showPlanets()
    {
        gameObject.SetActive(true);
        showPlayablePlanets();
        spaceship.SetActive(true);
    }    
    
    public void hidePlanets()
    {
        gameObject.SetActive(false);
    }
}
