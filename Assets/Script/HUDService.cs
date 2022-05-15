using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class HUDService
{
    public static GameObject displayText(string text, Vector3 position, GameObject damageHolder)
    {                
        damageHolder.name = "DamagePopUpHolder";
        damageHolder.transform.position = position;
        TextMesh textMesh = damageHolder.GetComponent<TextMesh>();
        textMesh.characterSize = 0.05f;
        textMesh.fontSize = 255;
        textMesh.text = text;
        damageHolder.transform.LookAt(Camera.main.transform);
        damageHolder.transform.Rotate(new Vector3(0, 180, 0));        
        return damageHolder;
    }
}
