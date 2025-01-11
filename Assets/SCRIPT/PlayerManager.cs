using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 5;
    private float horizontal;
    public float jumpForce = 5.5f;
    public int numberJump =0;
    public int maxNumberJump = 1;
    public bool canJump = false;
    //public float maxTimeJump = 0.5f;
    //private float timeActualJump = 0f;

    public LayerMask groundLayer;

    //public TextMeshProUGUI livesNumber;
    public float radio = 0.4f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private bool isJumpingButtonPressed = false;
    private int direction = 1;
    private float originalXScale;
    //private float xVelocity;
    //public int lives = 3;
    private bool isVulnerable = true;
    private float vulnerabilityTime = 0f;

    private Transform actualPlatform = null;


    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalXScale = transform.localScale.x;
        //livesNumber.text = lives.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMovement();
        PlayerJump();
        //CheckVulnerability();
    }

    private void PlayerMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        

        Vector2 movement = new Vector2(horizontal, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

       
        if (horizontal != 0)
        {
            animator.SetBool("isMoving", true);
        }

        else
        {
            animator.SetBool("isMoving", false);
        }

        if (horizontal < 0)
        {
            spriteRenderer.flipX = true;
        }

        else
        {
            spriteRenderer.flipX = false;
        }
       
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumpingButtonPressed = true;
    }

    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsInFloor())
            {
                Jump();
            }
            else if (numberJump < maxNumberJump)
            {
                Jump();
                numberJump ++ ; 
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumpingButtonPressed = false;
        }

        if (!isJumpingButtonPressed && IsInFloor())
        {
            numberJump = 0;
        }
    }

    public bool GetVulnerability()
    {
        return isVulnerable;
    }

    public void SetVulnerability()
    {
        isVulnerable = true;
        vulnerabilityTime = Time.deltaTime;
    }

    private void CheckVulnerability()
    {
        if (!isVulnerable && (Time.deltaTime - vulnerabilityTime) > 3f)
        {
            isVulnerable = true;
        }
    }

    private bool IsInFloor()
    {
        float weightPlayer = GetComponent<Collider2D>().bounds.extents.x;
        Vector2 centerOrigin = new Vector2(transform.position.x, transform.position.y);

        RaycastHit2D hitCentro = Physics2D.Raycast(centerOrigin, Vector2.down, 1.1f, groundLayer);
        Collider2D floor = Physics2D.OverlapCircle(centerOrigin, radio, groundLayer);

        Debug.DrawRay(centerOrigin, Vector2.down * 1.1f, Color.yellow);

        return hitCentro.collider != null || floor != null;
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Detectar si el player est� sobre una plataforma m�vil
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            actualPlatform = collision.transform;
            transform.parent = actualPlatform; //hacemos que es player sea hijo de la plataforma
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //Detectar si el player deja de estar sobre la plataforma
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            transform.parent = null; //hacemos que es player deje de ser hijo de la plataforma
            actualPlatform = null;
        }
    }
    /*private void Die()
    {

    }*/



}
