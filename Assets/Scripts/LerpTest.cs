using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTest : MonoBehaviour
{
    //  we'll test lerping and make it move in a circulat path by using different in between lerps

    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;
    public Transform pointE;
    public float desiredDuration = 4.0f;
    private float elapsedTime = 0f;

    void Update()
    {
        DoLerp();
    }

    void DoLerp()
    {
        elapsedTime += Time.deltaTime;
        float percentage = elapsedTime / desiredDuration;

        pointE.transform.position = Vector3.Lerp(pointB.transform.position, pointC.transform.position, Mathf.SmoothStep(0, 1, percentage));
        pointD.transform.position = Vector3.Lerp(pointA.transform.position, pointE.transform.position, Mathf.SmoothStep(0, 1, percentage));

    }
}
