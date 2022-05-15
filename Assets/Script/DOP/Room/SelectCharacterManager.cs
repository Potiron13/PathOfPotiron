using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharacterManager : MonoBehaviour
{    
    public List<GameObject> monsterFrames;
    
    public List<string> monsterNames;

    // Start is called before the first frame update
    public void Init()
    {
        for (int i = 0; i < monsterFrames.Count; i++)
        {
            monsterFrames[i].GetComponent<Frame>().UpdateSprite(LoadFromResourcesService.monsterSprite(monsterNames[i]));
        }
    }
}
