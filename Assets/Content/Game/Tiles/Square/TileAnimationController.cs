using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimationController : MonoBehaviour
{
    public Animator animator;

    public void Play(string name)
    {
        this.gameObject.SetActive(true);
        animator.Play(name);
    }

    public void OnAnimationFinish()
    {
        this.gameObject.SetActive(false);
    }

}
