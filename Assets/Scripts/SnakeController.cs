using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SnakeController : MonoBehaviour
{
    [Tooltip("Apply the head of the snake to make it rotate while moving")]
    [SerializeField] Transform snakeHead;
    [Tooltip("Text animator to pop up when the snake hits or absords an orb")]
    [SerializeField] Transform scoreAnimator;
    [SerializeField] Vector3 offsetValuesForScoreAnimator;
    [Tooltip("Set to either 1 or -1")]
    [SerializeField] private int direction = 1;
    private Text text;


    [SerializeField]
    private float moveSpeed = 10.0f;

    [SerializeField]
    private float sphereCastRadius = 5.0f;

    [Tooltip(
        "in case the radius is not on the exact position, we can use this value to adjust the center of the sphere cast"
    )]
    [SerializeField]
    private float y_OffstRadius = 2.0f;
    [Tooltip(
        "in case the radius is not on the exact position, we can use this value to adjust the center of the sphere cast"
    )]
    [SerializeField]
    private float z_OffstRadius = 2.0f;

    [SerializeField]
    private float maxDistance = 5.0f;

    [SerializeField]
    private LayerMask coinLayerMask;

    [Tooltip(
        "the main bone such as the neck or the backbone to which all other bones are interconnected, use this to increase the z length of the snake"
    )]
    [SerializeField]
    private Transform boneContainer;

    [Tooltip(
        "the value for which the snake's armature should be increased on z length, it must be a little larger than x and y scaling factors"
    )]
    [SerializeField]
    private float z_scaling_factor = 5;

    [SerializeField]
    private float x_scaling_factor = 2;

    [SerializeField]
    private float y_scaling_factor = 2;

    [SerializeField]
    private TextMeshPro textMeshPro;

    [SerializeField]
    private float fenceBumpForceMagntiude = 2.0f;

    [Tooltip(
        "e.g if the snake collides with the fence, after how many seconds it should be able to move again after having a force"
    )]
    [SerializeField]
    private float timeShuffleForMovingAgain;

    [SerializeField]
    private CameraShake cameraShake;

    [Tooltip(
        "For giving the player a fast turn either to right or to left, just like the clone app"
    )]
    [SerializeField]
    private float rightLeftMovementMultiplier = 2.0f;


    [Tooltip("as the bounding box collider has intense scaling, we need to multiply it with z_scaling factor so it gets equally lenghty with the snake main length ")]
    [SerializeField] private float z_sclaing_multiplier_for_boxCollider = 5f;

    [SerializeField] private ParticleSystem bubbleParticleSystemForColorMultiplier;
    private int startingScore = 1;

    private Rigidbody rb;
    private Transform collidedCoin;
    private int score;
    public int Score() => score;
    private bool canBeSliced = false;
    public bool canMove;
    public float canMoveAgainAfterBeingSliced = .5f;


    public Vector3 rot;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = startingScore;
        textMeshPro.text = score.ToString();
        canMove = true;
        startApplyingForce = false;
        canBeSliced = true;
        //bubbleParticleSystemForColorMultiplier.Stop();
    }

    private void Update()
    {
        MoveSnake();
        // IncreaseSnakeLength();
        textMeshPro.text = score.ToString();

        if (HasCollidedWithCoin())
        {
            score++;
            Handheld.Vibrate();
            Instantiate(scoreAnimator, collidedCoin.transform.position, Quaternion.identity);
            IncreaseSnakeLength();
        }

        if (startApplyingForce)
        {
            ApplyFastForce();
        }

        // snakeHead.transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
    }

    private bool IsScreenTouched()
    {
        int totalTouches = Input.touchCount;

        return totalTouches == 1;
    }

    private float SwipeStatus()
    {
        if (!(Input.touchCount > 0))
            return 0.0f;
        var userTouch = Input.GetTouch(0);
        var finalPosition = userTouch.deltaPosition; // change in position
        return finalPosition.x;
    }

    private void MoveSnake()
    {
        if (!canMove)
            return;
        if (!IsScreenTouched())
        {
            rb.velocity = Vector3.zero;
            return;
        }

        rb.velocity = new Vector3(
            transform.forward.x * direction * moveSpeed * Time.deltaTime,
            rb.velocity.y,
            transform.forward.z * direction * moveSpeed * Time.deltaTime
        );

        // moving right
        if (SwipeStatus() > 0.0f)
        {
            rb.velocity = new Vector3(
                transform.right.x
                    * direction
                    * moveSpeed
                    * rightLeftMovementMultiplier
                    * SwipeStatus()
                    * Time.deltaTime,
                rb.velocity.y,
                rb.velocity.z
            );


        }

        // moving left
        if (SwipeStatus() < 0.0f)
        {
            rb.velocity = new Vector3(
                -transform.right.x
                    * direction
                    * moveSpeed
                    * rightLeftMovementMultiplier
                    * Mathf.Abs(SwipeStatus())
                    * Time.deltaTime,
                rb.velocity.y,
                rb.velocity.z
            );

        }
    }

    private void IncreaseSnakeLength()
    {
        var snakeLocalScale = new Vector3(
            boneContainer.transform.localScale.x + x_scaling_factor,
            boneContainer.transform.localScale.y + y_scaling_factor,
            boneContainer.transform.localScale.z + z_scaling_factor
        );
        boneContainer.localScale = new Vector3(
            snakeLocalScale.x,
            snakeLocalScale.y,
            snakeLocalScale.z
        );


        // update the collider as well
        var mainCollider = GetComponent<BoxCollider>();
        var mainSize = mainCollider.size;
        mainCollider.size = new Vector3(-mainCollider.size.x, mainCollider.size.y, mainCollider.size.z + (z_scaling_factor * z_sclaing_multiplier_for_boxCollider));

    }
    private void DecreaseSnakeLength()
    {
        var snakeLocalScale = new Vector3(
            boneContainer.transform.localScale.x - x_scaling_factor,
            boneContainer.transform.localScale.y - y_scaling_factor,
            boneContainer.transform.localScale.z - z_scaling_factor
        );
        boneContainer.localScale = new Vector3(
            snakeLocalScale.x,
            snakeLocalScale.y,
            snakeLocalScale.z
        );
    }
    IEnumerator StartDecreaingSnakeLength()
    {
        canBeSliced = false;
        DecreaseSnakeLength();
        yield return new WaitForSeconds(canMoveAgainAfterBeingSliced);  // the floating axe can atleast not slice the snake again for 1.5f secs
        canBeSliced = true;
    }

    // private bool GetCollidedCoin()
    // {
    //     Vector3 origin = new Vector3(
    //         transform.position.x,
    //         transform.position.y + y_OffstRadius,
    //         transform.position.z
    //     );
    //     Ray ray = new Ray(origin, transform.forward);
    // }

    private bool HasCollidedWithCoin()
    {
        // we need to detect if the snake has collided with the coin, increase its z length
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y + y_OffstRadius,
            transform.position.z + z_OffstRadius
        );

        Ray ray = new Ray(origin, transform.forward);
        if (
            Physics.SphereCast(
                ray,
                sphereCastRadius,
                out RaycastHit hit,
                maxDistance,
                coinLayerMask
            )
        )
        {
            //  we have hit something
            collidedCoin = hit.transform;
            collidedCoin.transform.gameObject.SetActive(false);
            return true;
        }

        return false;
    }

    IEnumerator TurnOffCOlliderForSomeTime(Collider _collider)
    {
        _collider.isTrigger = true;
        yield return new WaitForSeconds(2f);
        _collider.isTrigger = false;
    }


    public void OnCollisionEnter(Collision collision)
    {
        // get the bounds of the collider
        if (collision.gameObject.CompareTag("cutter"))
        {
            var contact = collision.GetContact(0).normal;
            if (contact.z == 1)
            {
                snakeEndPosition = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z + zMagnitude
                );
                startApplyingForce = true;
                elapsedTime = 0f;

                StartCoroutine(ApplyFastForceCoroutine());
                cameraShake.DoCameraShake();
            }
            else if (contact.y > 0 || contact.y < 0)
            {
                // put your logic for cutting the snake in half
                if (canBeSliced)
                {
                    StartCoroutine(StartDecreaingSnakeLength());
                    StartCoroutine(TurnOffCOlliderForSomeTime(collision.collider));

                    // decreasing the score as well
                    score /= 2;
                }
            }
        }
        if (collision.gameObject.CompareTag("fence"))
        {
            var contact = collision.GetContact(0).normal;
            if (contact.z == 1)
            {
                snakeEndPosition = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z + zMagnitude
                );
                startApplyingForce = true;
                elapsedTime = 0f;
                StartCoroutine(ApplyFastForceCoroutine());
                cameraShake.DoCameraShake();
            }
        }
        if (collision.gameObject.CompareTag("enemySnake"))
        {
            // if your snake player touches any of the enemy snake, then get his total score
            var enemy = collision.gameObject;
            int enemyScore = enemy.gameObject.GetComponent<EnemySnakes>().GetScore();
            Handheld.Vibrate();

            // now check if the enemy score is less than the player score or not
            if (score > enemyScore)
            {
                Destroy(enemy);


                // now add this score to your score
                score += enemyScore;

                // instantiate animator as well
                var prefab = Instantiate(scoreAnimator, collision.gameObject.transform.position + offsetValuesForScoreAnimator, Quaternion.identity);
                var firstChild = prefab.GetChild(0);
                var secondChild = firstChild.GetChild(0);

                Debug.Log($"first child -> {firstChild.gameObject.name}, second child -> {secondChild.gameObject.name}");
                var _text = $"+{enemyScore}".ToString();
                secondChild.GetComponent<TextMeshPro>().text = _text;


                IncreaseSnakeLength();
            }
            else if (score <= enemyScore)
            {
                startApplyingForce = true;
                snakeEndPosition = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z + zMagnitude
                );
                elapsedTime = 0f;
                StartCoroutine(ApplyFastForceCoroutine());
                cameraShake.DoCameraShake();
            }
        }
        if (collision.gameObject.CompareTag("floatingAxe"))
        {
            var contact = collision.GetContact(0).normal;

            if (contact.x > 0 || contact.x < 0)
            {
                // the floating axe has probably hit from either the right or left side
                Debug.Log("Bounds are " + collision.collider.bounds.extents);


                // decreasing the snake length by half
                if (canBeSliced)
                {
                    StartCoroutine(StartDecreaingSnakeLength());

                    // decreasing the score as well
                    score /= 2;
                }


            }
        }
        if (collision.gameObject.CompareTag("rollingCutter"))
        {
            var contact = collision.GetContact(0).normal;
            if (contact.z == 1)
            {
                snakeEndPosition = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z + zMagnitude
                );
                startApplyingForce = true;
                elapsedTime = 0f;

                StartCoroutine(ApplyFastForceCoroutine());
                cameraShake.DoCameraShake();
            }
            else if (contact.y > 0 || contact.y < 0)
            {
                // put your logic for cutting the snake in half
                if (canBeSliced)
                {
                    StartCoroutine(StartDecreaingSnakeLength());
                    StartCoroutine(TurnOffCOlliderForSomeTime(collision.collider));


                    // decreasing the score as well
                    score /= 2;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("water"))
        {
            // just make sure that the game is over
            DeathScript deathScript = this.GetComponent<DeathScript>();
            GameObject panel = deathScript.GameOverPanel();
            panel.SetActive(true);
            canMove = false;
        }
        if (other.gameObject.CompareTag("colorMultiplier"))
        {
            Debug.Log("Hit with colorMultiplier");
            // we need to get the score tag and increase the snake length
            var scoreTag = other.transform.GetChild(0);
            var colorText = scoreTag.GetComponent<TextMeshPro>();
            var _score = int.Parse(colorText.text);
            Debug.Log($"score is {_score}");
            if (_score > 0)
            {
                for (int i = 1; i <= _score; i++)
                {
                    IncreaseSnakeLength();
                    this.score++;
                }
            }
            else if (_score < 0)
            {
                for (int i = 1; i <= Mathf.Abs(_score); i++)
                {
                    // as per the rule of the game, we should stop at 1 score
                    if (this.score == 1) break;


                    DecreaseSnakeLength();
                    this.score--;


                }
            }

            // now destroy that colorMultiplier
            var _parent1 = other.GetComponentInParent<Transform>();
            Destroy(_parent1.gameObject);
            Instantiate(bubbleParticleSystemForColorMultiplier, other.transform);
        }

    }


    public float desiredDuration = 2f;
    private float elapsedTime = 0f;
    public bool startApplyingForce = true;
    public float zMagnitude = 2.0f;
    private Vector3 snakeEndPosition;
    public AnimationCurve curve;
    public float timeAfterSnakeShouldStartAgain = 2f;

    private void ApplyFastForce()
    {
        elapsedTime += Time.deltaTime;
        float percentage = (elapsedTime / desiredDuration);
        transform.position = Vector3.Lerp(
            transform.position,
            snakeEndPosition,
            Mathf.SmoothStep(0, 1, curve.Evaluate(percentage))
        );
    }
    private void ApplyFastForceLeftOrRight()
    {

    }

    IEnumerator ApplyFastForceCoroutine()
    {
        startApplyingForce = true;
        yield return new WaitForSeconds(timeAfterSnakeShouldStartAgain);
        startApplyingForce = false;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y + y_OffstRadius,
            transform.position.z + z_OffstRadius
        );
        Gizmos.DrawWireSphere(origin, sphereCastRadius);
    }

    IEnumerator MovementShuffle(float time)
    {
        // after how many seconds, player should be able to move after a certain collision or any other thing
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
}
