using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SnakeControllerRigDamping : MonoBehaviour
{
    // this script is responsible for rig damping
    [SerializeField]
    private List<Transform> rigDampings = new List<Transform>();

    private SnakeController snakeController;
    private bool bumpHappened = false; // so when the startApplyingFastForce is turned on from snakeController, it reduces the dampings

    private void Start()
    {
        snakeController = GetComponent<SnakeController>();
        bumpHappened = snakeController.startApplyingForce;
    }

    private void Update()
    {
        bumpHappened = snakeController.startApplyingForce;

        // check for reduce dampings connected to rigs
        if (bumpHappened)
            ReduceDampings();
        else
            ImproveDampings();
    }

    private void ReduceDampings()
    {
        foreach (var dt in rigDampings)
        {
            var weight = dt.GetComponent<DampedTransform>();
            weight.weight = 0f;
        }
    }

    private void ImproveDampings()
    {
        foreach (var dt in rigDampings)
        {
            var weight = dt.GetComponent<DampedTransform>();
            weight.weight = 1f;
        }
    }
}
