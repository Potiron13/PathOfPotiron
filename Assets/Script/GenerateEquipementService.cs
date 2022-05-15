using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GenerateEquipementService
{
    public static Equipement generate(string name)
    {
        StandardEquipement item = LoadDataFromJson.LoadStandardEquipements().First(e => e.name == name);
        Equipement newItem = new Equipement();
        newItem.armorValue = item.armorValue;
        newItem.hpValue = item.hpValue;
        newItem.id = MockHelper.generateID();
        newItem.magicValue = item.magicValue;
        newItem.name = item.name;
        newItem.strengthValue = item.strengthValue;
        newItem.type = item.type;
        return newItem;
    }
}
