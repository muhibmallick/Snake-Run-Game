using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AnimationEvents : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] vcam = new CinemachineVirtualCamera[0];
    [SerializeField] CinemachineVirtualCamera originalVcam;


    public CinemachineVirtualCamera currentActiveVcam;
    private Animator anim;

    private void Start()
    {
        currentActiveVcam = originalVcam;
        anim = GetComponent<Animator>();
    }

    public void TurnCinemachineCamera(int index)
    {
        vcam[index].Priority = currentActiveVcam.Priority + 1;
        currentActiveVcam = vcam[index];
    }
    public void ChangeDrumMaterialColor_1(ref Transform drum, Color color)
    {
        drum.GetComponent<MeshRenderer>().material.color = color;
    }

    private void Update()
    {
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("part_1_continued") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("part_1_finished") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            )
        {
            vcam[0].Priority = currentActiveVcam.Priority + 1;
            currentActiveVcam = vcam[0];
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("part_2_continued") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("part_2_finished") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            )
        {
            vcam[1].Priority = currentActiveVcam.Priority + 1;
            currentActiveVcam = vcam[1];
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("part_3_continued") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
           || (anim.GetCurrentAnimatorStateInfo(0).IsName("part_3_finished") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
           )
        {
            vcam[2].Priority = currentActiveVcam.Priority + 1;
            currentActiveVcam = vcam[2];
        }
        if ((anim.GetCurrentAnimatorStateInfo(0).IsName("part_4_continued") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            || (anim.GetCurrentAnimatorStateInfo(0).IsName("part_4_finished") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= .1f)
            )
        {
            vcam[3].Priority = currentActiveVcam.Priority + 1;
            currentActiveVcam = vcam[3];
        }
    }

    public float maxDistance = 2f;
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
        int[] newArray = new int[10];

        // colliders


    }

}
