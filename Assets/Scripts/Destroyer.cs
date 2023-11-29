using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] float self_destruction_time = 1f;

    private void Start()
    {
        Invoke(nameof(DestroyObject), self_destruction_time);
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }

}
