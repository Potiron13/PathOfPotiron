using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image image;    
    
    public void setMaxHealth()
    {
        image.fillAmount = 1;        
    }

    public void setHealth(float amount)
    {        
        image.fillAmount = amount;
    }
}
