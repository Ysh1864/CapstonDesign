using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPotalSprite : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        ActivatePotal();
    }

    public void ActivatePotal()
    {/*
        if (FindObjectOfType<EndPotalTrigger>().isKey)
            animator.SetBool("isKey", true);
            */
    }
}
