using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame : MonoBehaviour
{
    [SerializeField] private Image frameImage;
    public Button button;

    public void UpdateSprite(Sprite sprite)
    {
        frameImage.sprite = sprite;
    }
}
