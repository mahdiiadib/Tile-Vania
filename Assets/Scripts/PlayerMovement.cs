using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D rb;
    SpriteRenderer spr;
    Animator animator;
    CapsuleCollider2D bodyCol;
    BoxCollider2D feetCol;
    [SerializeField] float runSpeed, jumpSpeed, climbSpeed;
    [SerializeField] Vector2 deathKick = new Vector2(10, 10);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    float defaultGravityScale;
    bool isAlive = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        bodyCol = GetComponent<CapsuleCollider2D>();
        feetCol = GetComponent<BoxCollider2D>();
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (isAlive)
        {
            if (moveInput.x == 0) animator.SetBool("isRunning", false);
            ClimbLadder();
            Die();
        }
    }

    void OnMove(InputValue value)
    {
        if (isAlive)
        {
            moveInput = value.Get<Vector2>();
            //print(moveInput);
            animator.SetBool("isRunning", true);
            if (moveInput.x < 0)
            {
                //spr.flipX = true;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (moveInput.x > 0)
            {
                //spr.flipX = false;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            rb.velocity = new Vector2(moveInput.x * runSpeed, rb.velocity.y);
        }
    }

    void OnJump(InputValue value)
    {
        if (isAlive && value.isPressed && feetCol.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            rb.velocity += new Vector2(0, jumpSpeed);
        }
    }

    void OnFire(InputValue value)
    {
        if(isAlive)
        {
            Instantiate(bullet, gun.position, transform.rotation);
        }
    }

    void ClimbLadder()
    {
        if (feetCol.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.velocity = new Vector2(rb.velocity.x, moveInput.y * climbSpeed);
            rb.gravityScale = 0;
            if (moveInput.y != 0) animator.SetBool("isClimbing", true);
            else animator.SetBool("isClimbing", false);
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
            animator.SetBool("isClimbing", false);
        }
    }

    void Die()
    {
        if (bodyCol.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")) || feetCol.IsTouchingLayers(LayerMask.GetMask("Hazards")))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            rb.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (feetCol.IsTouchingLayers(LayerMask.GetMask("Enemies")))
        {
            print($"Hit {other.gameObject.name}");
            Destroy(other.gameObject);
        }
    }
}
