using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Cinemachine;

public class EnemySnakes : MonoBehaviour
{
    [Tooltip("Set to either 1 or -1")]
    [SerializeField]
    private Text text;
    private int direction = 1;

    [SerializeField]
    private float moveSpeed = 10.0f;

    [SerializeField]
    private float sphereCastRadius = 5.0f;

    [Tooltip(
        "in case the radius is not on the exact position, we can use this value to adjust the center of the sphere cast"
    )]
    [SerializeField]
    private float y_OffstRadius = 2.0f;

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

    [Tooltip(
        "For giving the player a fast turn either to right or to left, just like the clone app"
    )]
    [SerializeField]
    private float rightLeftMovementMultiplier = 2.0f;

    [SerializeField]
    private int startingScore = 2;
    [SerializeField] Transform[] backBones;  // for inreasing their scales

    private Rigidbody rb;
    private Transform collidedCoin;
    private int score;

    public int GetScore() => score;

    private bool canMove;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        score = startingScore;
        textMeshPro.text = score.ToString();
        canMove = true;
        score = 0;

        // as this is an enemy snake, increase the length 'score' numbre of times
        for (int i = 1; i <= startingScore; i++)
        {
            IncreaseSnakeLength();
            score++;
        }

        // we will not use this for inreasing snakes length on runtime as it deforms the mesh
    }

    private void Update()
    {
        MoveSnake();
        // IncreaseSnakeLength();
        textMeshPro.text = score.ToString();

        /*if (HasCollidedWithCoin())
        {
            score++;
            IncreaseSnakeLength_2();
        }*/
    }




    private void MoveSnake()
    {
        if (!canMove)
            return;
        // if (!IsScreenTouched())
        // {
        //     rb.velocity = Vector3.zero;
        //     return;
        // }

        rb.velocity = new Vector3(
            transform.forward.x * direction * moveSpeed * Time.deltaTime,
            rb.velocity.y,
            transform.forward.z * direction * moveSpeed * Time.deltaTime
        );
        // // moving right
        // rb.velocity = new Vector3(
        //     transform.right.x
        //         * direction
        //         * moveSpeed
        //         * rightLeftMovementMultiplier
        //         * Time.deltaTime,
        //     rb.velocity.y,
        //     rb.velocity.z
        // );
        // // moving left
        // rb.velocity = new Vector3(
        //     -transform.right.x
        //         * direction
        //         * moveSpeed
        //         * rightLeftMovementMultiplier
        //         * Time.deltaTime,
        //     rb.velocity.y,
        //     rb.velocity.z
        // );
    }

    private void IncreaseSnakeLength()
    {
        Debug.Log("Snake collided");
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
        // update the score
    }
    private void IncreaseSnakeLength_2()
    {
        // now take each transform and increase its local scale y value
        Array.ForEach(backBones, bone => bone.localScale = new Vector3(bone.localScale.x, bone.localScale.y + y_scaling_factor, bone.localScale.z));
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
            transform.position.z
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
            Debug.Log("Hit object is " + hit.transform.gameObject.name);
            collidedCoin = hit.transform;
            collidedCoin.transform.gameObject.SetActive(false);
            return true;
        }

        return false;
    }

    public void OnCollisionEnter(Collision collision)
    {
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
                //var prefab = Instantiate(scoreAnimator, collision.gameObject.transform.position + offsetValuesForScoreAnimator, Quaternion.identity);
                //var firstChild = prefab.GetChild(0);
                //var secondChild = firstChild.GetChild(0);

                IncreaseSnakeLength();
            }

        }
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
   /* private void OnTriggerEnter(Collider other)
    {
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
                    DecreaseSnakeLength();
                    this.score--;
                }
            }

            // now destroy that colorMultiplier
            var _parent1 = other.GetComponentInParent<Transform>();
            Destroy(_parent1.gameObject);

        }

    }
*/
    private void ApplyForce()
    {
        var direction = -transform.forward * fenceBumpForceMagntiude;
        rb.AddForce(direction, ForceMode.Impulse);
        StartCoroutine(MovementShuffle(timeShuffleForMovingAgain));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = new Vector3(
            transform.position.x,
            transform.position.y + y_OffstRadius,
            transform.position.z
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
