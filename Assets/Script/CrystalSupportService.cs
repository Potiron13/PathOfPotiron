using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CrystalSupportService
{
    public class CrystalBonus
    {
        public List<int> offsetAngles = new List<int>() { 0 };
        public int angleNumber = 1;
        public int attackNumber = 1;
    }
    
    public static CrystalBonus handleSupportCrystals(CharacterAbility characterAbilitie)
    {
        CrystalBonus bonus = new CrystalBonus();
        if (characterAbilitie.supportCrystals.Contains("1"))
        {
            bonus.offsetAngles.Add(45);
            bonus.offsetAngles.Add(-45);
            bonus.angleNumber = 3;
        }
        if (characterAbilitie.supportCrystals.Contains("2"))
        {
            bonus.attackNumber = 2;
        }
        return bonus;
    }  

    public static Vector3 rotateVectorOnY(Vector3 startRotation, int offsetAngle)
    {
        Vector3 rotation = new Vector3(0, startRotation.y + offsetAngle, 0);
        return rotation;
    }

    public static List<string> generateCrystalSupports()
    {
        List<SupportCrystal> supportCrystals = LoadDataFromJson.LoadSupportCrystals();
        int crystalIndex = Random.Range(0, supportCrystals.Count);
        return new List<string> { supportCrystals[crystalIndex].id };
    }
}
