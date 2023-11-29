using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlerpTest : MonoBehaviour
{
    public Transform sunrise;
    public Transform sunset;

    // Time to move from sunrise to sunset position, in seconds.
    public float journeyTime = 1.0f;


    // while making the center, this value will increase the arc
    public float extraYFactor = 2.0f;

    // The time at which the animation started.
    private float elapsedTime = 0f;
    public AnimationCurve curve;

    void Start()
    {
        // Note the time at the start of the animation.
    }

    void Update()
    {
        // The center of the arc
        Vector3 center = (sunrise.position + sunset.position) * 0.5F;

        // move the center a bit downwards to make the arc vertical
        center -= new Vector3(0, extraYFactor, 0);

        // Interpolate over the arc relative to center
        Vector3 riseRelCenter = sunrise.position - center;
        Vector3 setRelCenter = sunset.position - center;

        // The fraction of the animation that has happened so far is
        // equal to the elapsed time divided by the desired time for
        // the total journey.
        elapsedTime += Time.deltaTime;
        float percentage = elapsedTime / journeyTime;

        transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, curve.Evaluate(percentage));
        transform.position += center;
    }

}
