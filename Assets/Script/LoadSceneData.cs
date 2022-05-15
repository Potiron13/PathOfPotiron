using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoadSceneData
{
    public static List<List<CharacterFromDB>> enemies = new List<List<CharacterFromDB>>();
    public static List<CharacterFromDB> bosses = new List<CharacterFromDB>();
    public static List<CharacterFromDB> pets = new List<CharacterFromDB>();
    public static CharacterFromDB playerFromDB = new CharacterFromDB();
    public static CharacterConfig characterConfig = new CharacterConfig();
    public static string petLoot = "";
    public static string equipementLoot = "";
    public static int petLootLevel = 1;
    public static AreaFromDB area = new AreaFromDB();
    public static Planet planet = new Planet();
    
}


