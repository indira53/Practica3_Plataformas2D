using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemiesManager : MonoBehaviour
{
    [SerializeField]
    private int speed;
    const int moveSpeed = 3;
    private bool canChangeDirection = true;
    private float verticalPointContact;
    private GameObject player;
    private bool isLeft = true;
    private Animator animator;
    private LifeManager lifeManager;
    public float attackCooldown;
    private float attackCooldownCounter;
    private bool isFacingRight = true;
    private Rigidbody2D rb;



    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        lifeManager = GetComponent<LifeManager>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isFalling = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, LayerMask.GetMask("Ground")).collider == null;
        if (!isFalling)
        {
            Flip();
        }

        var playerDirection = FindPlayer(5f);
        var obstacleDirection = FindObstacles();

        switch (playerDirection)
        {
            /*case  0:
                playerDetected = false;
            break;*/
            case < 0:
                isLeft = true;
                speed = moveSpeed * -1; //Mover a la izquierda
                animator.SetBool("isMoving", true);
                break;
            case > 0:
                isLeft = false;
                speed = moveSpeed; //Mover a la derecha;
                animator.SetBool("isMoving", true);
                break;
        }
        if (attackCooldownCounter > attackCooldown && 0 < Mathf.Abs(playerDirection) && Mathf.Abs(playerDirection) < 2.5f)
        {
            animator.SetTrigger("Attacks");
            attackCooldownCounter = 0f;
            rb.constraints = (RigidbodyConstraints2D)5;

        }
        attackCooldownCounter += Time.deltaTime;




        if (obstacleDirection != 0 && isLeft ? obstacleDirection == -1 :  obstacleDirection == 1)
        {
            speed = 0;
            animator.SetBool("isMoving", false);
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }


    //Busca al player en izquierda y derecha y devuelve -1 si está a la izquierda, 1 si está a la derecha o 0 si no lo detecta
    float FindPlayer(float distance)
    {
        RaycastHit2D detectedPlayerLeft = Physics2D.Raycast(transform.position, Vector2.left, distance, LayerMask.GetMask("Player"));
        RaycastHit2D detectedPlayerRight = Physics2D.Raycast(transform.position, Vector2.right, distance, LayerMask.GetMask("Player"));
        if (detectedPlayerLeft.collider != null)
        {

            return detectedPlayerLeft.distance * -1;
        }
        if (detectedPlayerRight.collider != null)
        {
            return detectedPlayerRight.distance;
        }
        return 0;
    }

    int FindObstacles()
    {
        RaycastHit2D detectedWallLeft = Physics2D.Raycast(transform.position, Vector2.left, 1f, LayerMask.GetMask("Wall"));
        RaycastHit2D detectedWallRight = Physics2D.Raycast(transform.position, Vector2.right, 1f, LayerMask.GetMask("Wall"));
        RaycastHit2D detectedGroundlLeft = Physics2D.Raycast(transform.position, new Vector2(-1, -1), 3f, LayerMask.GetMask("Ground"));
        RaycastHit2D detectedGroundRight = Physics2D.Raycast(transform.position, new Vector2(1, -1), 3f, LayerMask.GetMask("Ground"));
        if (detectedWallLeft.collider != null || detectedGroundlLeft.collider == null)
        {
            return -1;
        }
        if (detectedWallRight.collider != null || detectedGroundRight.collider == null)
        {
            return 1;
        }
        return 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        verticalPointContact = collision.contacts[0].normal.y;

        if (collision.gameObject.tag == "Player")
        {
            if (verticalPointContact < -0.5f)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * 4;

            } 
            else
            {
                if (player.GetComponent<PlayerManager>().GetVulnerability())
                {
                    player.GetComponent<PlayerManager>().SetVulnerability();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            lifeManager.Health -= 1;
            animator.SetTrigger("Hurt");
            if (lifeManager.Health == 0)
            {
                speed = 0;
                Invoke("EnemyDie", 0.5f);
            }
        }
    }
    private void Flip()
    {
        //voltear al enemigo

        if (isFacingRight && speed < 0f || !isFacingRight && speed > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void EnemyDie()
    {
        Destroy(this.gameObject);   
    }
}


