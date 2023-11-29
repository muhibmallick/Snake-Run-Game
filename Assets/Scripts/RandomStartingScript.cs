using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomStartingScript : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
        Invoke("Enable_Disable_Animator", Random.Range(1f, 5f));
    }

    void Enable_Disable_Animator()
    {
        animator.enabled = true;
    }


}
