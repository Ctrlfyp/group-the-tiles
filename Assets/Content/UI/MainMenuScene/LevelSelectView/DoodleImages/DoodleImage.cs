using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoodleImage : MonoBehaviour
{

    public Image imageContainer;

    public void SetImage(Sprite sprite, int size, int angle, Color color)
    {
        imageContainer.sprite = sprite;
        imageContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(size, size);
        imageContainer.rectTransform.localEulerAngles = new Vector3(0, 0, angle);
        imageContainer.color = color;
    }
}
