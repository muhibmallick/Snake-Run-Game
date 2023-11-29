using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{

    public void OnCollisionEnter(Collision other)
    {
        var boundingBoxes = other.collider.bounds.extents;
        Debug.Log(boundingBoxes);
    }
}
