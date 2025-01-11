using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemiesManager : MonoBehaviour
{
    public bool playerDetected;
    [SerializeField]
    private int speed = 10;
    private bool canChangeDirection = true;
    private float verticalPointContact;
    private GameObject player;
    private bool isLeft = true;


    // Start is called before the first frame update
    void Start()
    {
        playerDetected = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerDetected)
        {
            //Detectar al jugador a la izquierda
            RaycastHit2D detectedLeft = Physics2D.Raycast(transform.position, Vector2.left, 30f, LayerMask.GetMask("Player"));
            if (detectedLeft.collider !=null)
            {
                Debug.Log("player detectado a la izquierda");
                playerDetected=true;
                isLeft = true;
                speed = Mathf.Abs(speed) * -1; //Mover a la izquierda
                Flip();
            }
            //Detectar al jugador a la derecha
            RaycastHit2D detectedRight = Physics2D.Raycast(transform.position, Vector2.right, 30f, LayerMask.GetMask("Player"));
            if (detectedRight.collider != null)
            {
                Debug.Log("player detectado a la derecha");
                playerDetected = true;
                isLeft = false;
                speed = Mathf.Abs(speed) * -1; //Mover a la izquierda
                Flip();
            }
        }
        else
        {
            CheckChangeDirection();
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
        //Verificaar si está cayendo
        IsFalling();
    }

    void IsFalling()
    {
        RaycastHit2D detectedBottom = Physics2D.Raycast(transform.position, Vector2.down, 1.3f, LayerMask.GetMask("Ground"));
        if (detectedBottom.collider == null)
        {
            canChangeDirection = false;
        }
        else
        {
            canChangeDirection = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        verticalPointContact = collision.contacts[0].normal.y;

        if (collision.gameObject.tag == "Player")
        {
            if (verticalPointContact < -0.5f)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * 4;
                speed = 0;
                //this.GetComponent<Animator>().SetBool("isDead", true);
                Invoke("EnemyDie", 0.5f);
            } 
            else
            {
                if (player.GetComponent<PlayerManager>().GetVulnerability())
                {
                    //player.GetComponent<Animator>().SetBool("isDead", true);
                    //player.GetComponent<PlayerManager>().SubstractLives();
                    player.GetComponent<PlayerManager>().SetVulnerability();
                    Debug.Log("Has Muerto");
                }
            }
        }
    }

    private void Flip()
    {
        //voltear al enemigo
        this.GetComponent<SpriteRenderer>().flipX ^= true;
    }

    private void CheckChangeDirection()
    {
        if(canChangeDirection)
        {
            RaycastHit2D detected;
            if (isLeft)
            {
                detected = Physics2D.Raycast(transform.position, Vector2.left, 1f, LayerMask.GetMask("Ground"));
            }
            else
            {
                detected = Physics2D.Raycast(transform.position, Vector2.right, 1f, LayerMask.GetMask("Ground"));
            }
            if (detected)
            {
                isLeft = !isLeft;
                speed *= -1;
                Flip();
            }
        }
    }

    private void EnemyDie()
    {
        Destroy(this.gameObject);   
    }
}


