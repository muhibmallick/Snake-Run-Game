using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class SnakeControllerWinning : MonoBehaviour
{
    // this script will be executed once the snake has reached the final spot of the level for slerp animations
    [SerializeField] private List<Transform> drums = new List<Transform>();
    [SerializeField] private float desiredDuration = 4.0f;
    [Tooltip("Minimum distance when the snake starts lerping towards the drum, so we can start rotating")]
    [SerializeField] private float minDistance = 5.0f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float nextSceneStartTime = 2f;
    [SerializeField] CinemachineVirtualCamera winningVcam;



    private bool hasReachedTheWinningLine = false;
    private float elapsedTime = 0f;
    private SnakeController snakeController;
    private int currentDrumIndex = 0;
    private bool isLerping = false;
    private bool isRotating = false;
    private float timer;
    private bool enoughLerping = false;  // when this is true, snake should stop rotating around the drums after winning
    private int totalRotationTodo;
    private int currentRotation = 0;
    private float sceneTimer = 0;



    private void Start()
    {
        hasReachedTheWinningLine = false;
        snakeController = GetComponent<SnakeController>();
        isLerping = true;
        isRotating = false;
        currentDrumIndex = 0;
    }
    private void Update()
    {
        if (hasReachedTheWinningLine && (currentRotation <= totalRotationTodo))
        {
            snakeController.canMove = false;
            // winningVcam.LookAt = snakeController.gameObject.transform;
            winningVcam.Priority = 11;
            // isLerping = true;
            StartLerpingTowardsDrums();
        }

        Timer();
        GetMultiples();

        if (currentRotation >= totalRotationTodo)
        {
            sceneTimer += Time.deltaTime;
            if (sceneTimer >= nextSceneStartTime)
            {
                InitiateNextScene();
            }
        }
    }
    private void DrumColors()
    {

    }
    private void InitiateNextScene()
    {
        // initiate the next scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    private void StartLerpingTowardsDrums()
    {

        if (isLerping)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / desiredDuration;

            transform.position = Vector3.Lerp(transform.position, drums[currentDrumIndex].position, Mathf.SmoothStep(0, 1, percentage));

            if (GetDistanceBetween(transform.position, drums[currentDrumIndex].position) <= minDistance)
            {
                isLerping = false;
                isRotating = true;
                currentRotation++;  // one rotation is complete
            }
        }
        else if (isRotating)
        {
            // checking the relative drum with relative drum index and changing its material colors
            if (currentDrumIndex == 0) drums[currentDrumIndex].gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            else if (currentDrumIndex == 1) drums[currentDrumIndex].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            else if (currentDrumIndex == 2) drums[currentDrumIndex].gameObject.GetComponent<MeshRenderer>().material.color = Color.yellow;
            else drums[currentDrumIndex].gameObject.GetComponent<MeshRenderer>().material.color = Color.white;


            // Calculate the target direction by subtracting the current position from the centerObject's position
            Vector3 targetDirection = drums[currentDrumIndex].position - transform.position;

            // Rotate the object around the centerObject
            var objectToRotateTowards = new Vector3(drums[currentDrumIndex].position.x, drums[currentDrumIndex].position.y, drums[currentDrumIndex].position.z);
            transform.RotateAround(objectToRotateTowards, Vector3.up, rotationSpeed * Time.deltaTime);

            // Face the object towards the target direction
            transform.rotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            GetComponent<Rigidbody>().isKinematic = true;
            vcam.Follow = null;
        }

    }

    private void Timer()
    {
        if (isRotating)
        {

            if (currentDrumIndex == drums.Count - 1)
            {
                // no need to update the index anymore
                return;
            }


            timer += Time.deltaTime;
            if (timer >= 3f)
            {
                // it means the snake should start lerping towards next drum
                isLerping = true;
                isRotating = false;
                timer = 0f;
                elapsedTime = 0f;
                currentDrumIndex++;
            }
        }
    }

    private void GetMultiples()
    {
        // when the score is <= 10, snake should do one rotation
        int score = snakeController.Score();

        if (score <= 10) totalRotationTodo = 1;
        else if (score <= 20) totalRotationTodo = 2;
        else if (score <= 30) totalRotationTodo = 3;
        else totalRotationTodo = 4;




    }
    private float GetDistanceBetween(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b);
    }


    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("winningLine"))
        {
            // getting near the first drum at a specific distance
            hasReachedTheWinningLine = true;
        }
    }



}
