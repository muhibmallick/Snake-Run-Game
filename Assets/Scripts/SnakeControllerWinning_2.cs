using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class SnakeControllerWinning_2 : MonoBehaviour
{
    // todo list
    // when the snake reaches the finishing line, lerp towards the specific point
    // then play the animation for rotating around the drum
    // we need 2 rolling animations, one for left rotating and one for right rotating

    [Tooltip("those drums whose reach points will be used for the snake rolling animations")]
    [SerializeField] Transform[] drums;
    [Tooltip("Animator of the snake to play drum roll animations")]
    [SerializeField] Animator snakeAnimator;
    [Tooltip("Lerp speed towrds the drum point")]
    [SerializeField] float lerpSpeed = 1f;
    [SerializeField] float drumPointDistanceThreshold = .15f;
    [SerializeField] ParticleSystem[] endingBlasts = new ParticleSystem[0];
    [SerializeField] CinemachineVirtualCamera originalVcam;
    [SerializeField] CinemachineVirtualCamera winningCam;
    [Header("Settings for cross limits for scoring")]
    [SerializeField] int scoreLimitForPart_1;
    [SerializeField] int scoreLimitForPart_2;
    [SerializeField] int scoreLimitForPart_3;
    [SerializeField] int scoreLimitForPart_4;
    [SerializeField] float timeForFinalAnimations = 2f;
    int snakeScore;


    float lerpTimer = 0f;
    bool hasReachedTheWinningLine;
    int currentDrumIndex;
    int currentLevel;
    string animationName;
    RigBuilder snakeRigBuilder;
    Transform[] drumReachPoints;
    ParticleSystem[] drumSmokes;

    public bool isAnimatingFinally;


    private void Start()
    {
        // as the snake is the first child of this game object
        snakeAnimator = this.gameObject.transform.GetChild(0).GetComponent<Animator>();
        snakeRigBuilder = this.gameObject.transform.GetChild(0).GetComponent<RigBuilder>();
        hasReachedTheWinningLine = false;

        // as drums are 4 so drumReachPoints will be 4 as well
        drumReachPoints = new Transform[drums.Length];
        drumSmokes = new ParticleSystem[drums.Length];

        // we don't want the snake animator to play animations in the level
        //snakeAnimator.enabled = false;
        //snakeRigBuilder.enabled = false;

        // assign the values of drum points
        for (int i = 0; i < drums.Length; i++)
        {
            // as the drums have only one child
            drumReachPoints[i] = drums[i].transform.GetChild(0);

            // getting the drum smokes 
            drumSmokes[i] = drums[i].transform.GetChild(1).GetComponent<ParticleSystem>();

            // turning them off
            drumSmokes[i].GetComponent<ParticleSystem>().Stop();

        }

        currentDrumIndex = 0;

        // as this is our first animation
        animationName = "leftRoll1";

        // dont play the particle systems at start
        Array.ForEach(endingBlasts, ps => ps.Stop());
        isAnimatingFinally = false;

        // saving currentLevel
        currentLevel = SceneManager.GetActiveScene().buildIndex;
        Debug.Log($"currentLevel is {currentLevel}");
    }

    private void Update()
    {
        if (hasReachedTheWinningLine)
        {
            // start lerping towards the first drum point
            StartLerping();

            Debug.Log($"animation info {snakeAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash}");
        }
    }

    private void StartLerping()
    {
        // we need to reach the drum reach point first
        // code test ..
        lerpTimer += Time.deltaTime;
        float percentage = lerpTimer / lerpSpeed;
        transform.position = Vector3.Lerp(transform.position, drumReachPoints[currentDrumIndex].transform.position, Mathf.SmoothStep(0, 1, percentage));

        // check if this transform has reached the first drum reached point or not
        if (Vector3.Distance(this.transform.position, drumReachPoints[0].transform.position) <= drumPointDistanceThreshold)
        {
            // we need to start the animation
            if (!isAnimatingFinally)
            {
                StartCoroutine(FinalAnimationCoroutine());
            }
        }


    }

    /*bool AnimationIsCompleted(Animator anim, string animationName)
    {
        return 
    }*/

    IEnumerator FinalAnimationCoroutine()
    {
        isAnimatingFinally = true;
        snakeAnimator.enabled = true;
        snakeRigBuilder.enabled = true;
        this.GetComponent<RigBuilder>().enabled = false;
        snakeRigBuilder.Build();
        var snakeController = this.gameObject.GetComponent<SnakeController>();
        var snakeAnimationEvent = snakeAnimator.gameObject.GetComponent<AnimationEvents>();
        int score = this.snakeScore = snakeController.Score();
        Debug.Log($"score is {score} and score limit is {scoreLimitForPart_1}");
        snakeAnimator.SetTrigger("triggerFinalAnimation");
        Array.ForEach(drumSmokes, smoke => smoke.Play());

        if (score <= scoreLimitForPart_1)
        {
            snakeAnimator.SetBool("part_1", true);
            snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[0], Color.green);
        }
        else
        {
            snakeAnimator.SetBool("part_1", false);
            snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[0], Color.green);
            yield return new WaitForSeconds(timeForFinalAnimations);

            if (score <= scoreLimitForPart_2)
            {
                snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[1], Color.blue);
                snakeAnimator.SetBool("part_2", true);
            }
            else
            {
                snakeAnimator.SetBool("part_2", false);
                snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[1], Color.blue);
                yield return new WaitForSeconds(timeForFinalAnimations);
                if (score <= scoreLimitForPart_3)
                {
                    snakeAnimator.SetBool("part_3", true);
                    snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[2], Color.red);

                }
                else
                {
                    snakeAnimator.SetBool("part_3", false);
                    snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[2], Color.red);
                    yield return new WaitForSeconds(timeForFinalAnimations);
                    if (score <= scoreLimitForPart_4)
                    {
                        snakeAnimator.SetBool("part_4", true);
                        snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[3], Color.yellow);

                    }
                    else
                    {
                        snakeAnimator.SetBool("part_4", false);
                        snakeAnimationEvent.ChangeDrumMaterialColor_1(ref drums[3], Color.yellow);

                        yield return new WaitForSeconds(timeForFinalAnimations);
                    }
                }
            }
        }
        // isAnimatingFinally = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("winningLine"))
        {
            // we have touched the winning line
            hasReachedTheWinningLine = true;
            Array.ForEach(endingBlasts, ps => ps.Play());
            //winningCam.Priority = originalVcam.Priority + 1;

            // we need to save the game
            PlayerPrefs.SetInt("currentLevel", currentLevel + 1);
            Debug.Log($"currentLevel is {currentLevel}");

            StartCoroutine(StartNextScene());
        }
    }

    void LoadNextScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;
        if (sceneIndex == totalScenes - 1)
        {
            // next scene doesn't exist as we're on the last scene
        }
        else
        {
            // load the next scene
            SceneManager.LoadScene(sceneIndex + 1);
        }
    }

    IEnumerator StartNextScene()
    {
        if (this.snakeScore <= scoreLimitForPart_1)
        {
            yield return new WaitForSeconds(6f);
            LoadNextScene();
        }
        else if (this.snakeScore > scoreLimitForPart_1 && this.snakeScore <= scoreLimitForPart_2)
        {
            yield return new WaitForSeconds(10f);
            LoadNextScene();

        }
        else if (this.snakeScore > scoreLimitForPart_2 && this.snakeScore <= scoreLimitForPart_3)
        {
            yield return new WaitForSeconds(14f);
            LoadNextScene();

        }
        else
        {
            yield return new WaitForSeconds(18f);
            LoadNextScene();

        }
    }
}
