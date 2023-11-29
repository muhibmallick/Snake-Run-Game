using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouthBehaviour : MonoBehaviour
{
    [SerializeField] float coinDetectionRadius;
    [SerializeField] LayerMask coinDetectionLayermask;
    [SerializeField] Vector3 positionOffset;
    [SerializeField] Animator anim;



    private void Start()
    {
        // anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Physics.CheckSphere(transform.position + positionOffset, coinDetectionRadius, coinDetectionLayermask))
        {
            // write down the code for playing the mouth animation here
            anim.SetBool("openMouth", true);
        }
        else anim.SetBool("openMouth", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + positionOffset, coinDetectionRadius);
    }
}
