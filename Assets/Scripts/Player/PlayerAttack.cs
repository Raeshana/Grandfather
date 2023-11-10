using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerJump jump;
    private PlayerMovement move;
    private PlayerBlock block;

    public static bool isAttacking; // EnemyController.cs needs this bool so I changed it to public -- Cheyu

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        jump = GetComponent<PlayerJump> ();
        move = GetComponent<PlayerMovement> ();
        block = GetComponent<PlayerBlock> ();
        anim = GetComponent<Animator> ();

        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {        
        if (!jump.getIsJumping() && !move.getIsMoving() && !block.getIsBlocking() && Input.GetButtonUp("Fire1"))
        {
            Debug.Log("Attack");
            isAttacking = true;
            anim.SetTrigger("canAttack");
            // I created a EnemyIsAttacked(int damage) function in EnemyHealth.cs. It probably can be used here. -- Cheyu
        }
        else
        {
            isAttacking = false; // last frame of attack animation
        }
    }

    public bool getIsAttacking()
    {
        return isAttacking;
    }
}