using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// possible resource: https://www.youtube.com/watch?v=AD4JIXQDw0s
// next step:
// 1. search for how to make a good combat enemy AI / gameplay strategies in street fighter
// 2. create an invisible gameObj with BoxCollision to detect whether player is in enemy's attack scope
// 3. create a boss health script
// 4. separate the scripts to make them organized

public class EnemyController : MonoBehaviour
{
    public string sceneToLoad;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private EnemyHealth hp;

    public bool EnemyIsAlive = true;
    public bool EnemyIsJumping = false;
    public bool EnemyIsMoving = false;
    public bool EnemyIsAttacking = false;
    public bool EnemyIsBlocking = false;
    public bool EnemyIsEnraged = false; // to do

    private float jumpForce; // should it be public? should enemy use the same parameters as player?
    private PlayerMovement pm;
    private PlayerAttack pa;
    private float EnemyMoveSpeed;
    private float moveDirection;
    private float EnemyWidth;
    public float attackRange;
    private float distanceBtw;


    [SerializeField] ParticleSystem dust;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        hp = GetComponent<EnemyHealth>();
        sr.flipX = true; // assuming that the enemy is facing right by default (x>0),
                         // then it should be flipped as the game start to face left (facing the player)
        player = GameObject.FindGameObjectWithTag("Player");
        pm = player.GetComponent<PlayerMovement>();
        pa = player.GetComponent<PlayerAttack>();

        EnemyMoveSpeed = (float)(pm.moveSpeed * 0.5);
        EnemyWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        attackRange = EnemyWidth;

    }

    // Update is called once per frame

    bool EnemyShouldJump()
    {
        // to do
        return false;
    }

    bool EnemyShouldMove()
    {
        // to do
        return distanceBtw > attackRange * 0.5;
    }

    bool EnemyShouldAttack()
    {
        // to do
        // if player is within attack scope, and player is not attacking, and is not blocking/about to end blocking
        return distanceBtw < attackRange;
    }

    bool EnemyShouldBlock()
    {
        // to do
        // if enemy is withink player's attack scope, and player is attacking
        return pa.getIsAttacking();
    }

    void EnemyFlip()
    {
        if (moveDirection < 0f)
        {
            sr.flipX = true; // assuming that the enemy is facing right by default (x>0)
        }
        else
        {
            sr.flipX = false;
        }
    }

    void EnemyMove()
    {
        //rb.velocity = new Vector2(EnemyMoveSpeed * moveDirection, rb.velocity.y);

        moveDirection = rb.position.x - player.transform.position.x; // if enemy is on player's right, md > 0
        EnemyFlip();
        Vector2 target = new Vector2(player.transform.position.x, rb.position.y); //always moving towards the player
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, EnemyMoveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        Debug.Log("should a?");

        dust.Play();
    }

    void EnemyAttack()
    {
        // to do
        // if player is withink enemy's attack scope, and player is not blocking, then player's health goes down
        // make sure that player's health only decreases once, either in this script or PlayerHurt.cs
    }

    void Update()
    {
        if (EnemyIsAlive) {
            distanceBtw = Vector2.Distance(player.transform.position, rb.position);

            if (EnemyShouldMove() && !EnemyIsJumping
                && !EnemyIsBlocking) // enemy should be allowed to move while attacking?
            {
                EnemyIsMoving = true;
                EnemyMove();
            }
            else
            {
                EnemyIsMoving = false;
            }

            if (EnemyShouldJump() && !EnemyIsMoving
                && !EnemyIsAttacking && EnemyIsBlocking)
            {
                rb.AddForce(new Vector2(rb.velocity.x, jumpForce));
            }
            

            if (EnemyShouldAttack() && !EnemyIsJumping
                && !EnemyIsMoving && !EnemyIsBlocking)
            {
                EnemyIsBlocking = true;
                EnemyAttack();
            }
            else
            {
                EnemyIsBlocking = false;
            }

            if (EnemyShouldBlock() && !EnemyIsJumping
                && !EnemyIsMoving && !EnemyIsAttacking)
            {
                EnemyIsBlocking = true;
            }
            else
            {
                EnemyIsBlocking = false;
            }
        } else {
            PlayerPrefs.SetInt("Win", 1);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        { // if the obj that player is colliding with has the tag 'ground'
            EnemyIsJumping = false;
        } else if (other.gameObject.CompareTag("Player")) {
            StartCoroutine(FlashRoutine());
            //default damage is 5, maybe change it for normal attack and power attack?
            hp.EnemyIsAttacked(5);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            EnemyIsJumping = true;
        }
    }

    private IEnumerator FlashRoutine() {
        sr.color = Color.black;
        yield return new WaitForSeconds(0.2f);
        sr.color = Color.red;
    }
}
