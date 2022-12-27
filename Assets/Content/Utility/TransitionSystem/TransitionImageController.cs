using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionImageController : MonoBehaviour
{
    public GameObject transitionImage;

    public void HandleAnimationEnd()
    {
        transitionImage.SetActive(false);
    }
}
