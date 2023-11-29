using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Tooltip("Vcam for the player")]
    [SerializeField]
    private CinemachineVirtualCamera vcam;

    [Tooltip("Noise asset")]
    [SerializeField]
    private NoiseSettings noiseSettings;

    [SerializeField]
    private float shakeTime;

    public void DoCameraShake()
    {
        StartCoroutine(CameraShakeCoroutine());
    }

    IEnumerator CameraShakeCoroutine()
    {
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile =
            noiseSettings;
        yield return new WaitForSeconds(shakeTime);
        vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_NoiseProfile = null;
    }



}
