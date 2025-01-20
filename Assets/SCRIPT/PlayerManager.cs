using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public float moveSpeed = 5;
    private float horizontal;
    public float jumpForce = 5.5f;
    public int numberJump =0;
    public int maxNumberJump = 1;
    private bool isFacingRight = true;

    public LayerMask groundLayer;
    public LayerMask wallLayer;

    //public TextMeshProUGUI livesNumber;
    public float radio = 0.4f;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private bool isJumpingButtonPressed = false;
    private bool isVulnerable = true;
    private float vulnerabilityTime = 0f;

    private Transform actualPlatform = null;

    private bool isWallSliding;
    private float wallSlidingSpeed = 0.5f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(3f, 6f);

    public float dashForce = 6f;
    public float dashCooldown = 2f;
    private float dashCooldownCounter;
    public int maxDashAir = 1;
    public int numberDashAir = 0;

    private LifeManager _lifeManager;
    public GameManager gameManager;

    [SerializeField] private Transform wallCheck;

    public PlayerHealthDisplay healthDisplay;

    private Vector2 spawnPosition;
    bool checkpointReached;
    public Transform bulletSpawnPosition;
    public GameObject bulletPrefab;
    private GameObject currentBullet;
    public float bulletSpeed;

    private void Start()
    {
        currentBullet = null;
        checkpointReached = false;
        spawnPosition = transform.position;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _lifeManager = GetComponent<LifeManager>();
    }

    // Update is called once per frame
    void Update()
    {

        PlayerMovement();
        PlayerJump();
        CheckVulnerability();
        WallSlide();
        WallJump();
        ManageShooting();
        PlayerDash();

        if (!isWallJumping)
        {
            Flip();
        }
    }

    private void PlayerMovement()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        

        Vector2 movement = new Vector2(horizontal, 0) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);

        animator.SetBool("isMoving", horizontal != 0);
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

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        
        if (IsWalled() && !IsInFloor() && horizontal != 0f)
        {
            numberJump = 0;
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            animator.SetBool("isWalled", true);
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("isWalled", false);
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void Dash()
    {
        animator.SetTrigger("Dash");
        rb.velocity = new Vector2(isFacingRight? dashForce: -dashForce, 0);
        rb.constraints = (RigidbodyConstraints2D)6;
        Invoke("UnfreezeY", 0.6f);
        dashCooldownCounter = 0f;

    }

    private void PlayerDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !IsInFloor() && numberDashAir<maxDashAir)
        {
            Dash();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && IsInFloor() && dashCooldownCounter > dashCooldown )
        {
            Dash();
            
        }
        dashCooldownCounter += Time.deltaTime;
    }

    public void UnfreezeY()
    {
        rb.constraints = (RigidbodyConstraints2D)4;
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
    private void ManageShooting()
    {
        
        if (Input.GetMouseButtonDown(0) && currentBullet == null)
        {
            Shoot();
        }
    }
    private void Shoot()
    {

        currentBullet = Instantiate(bulletPrefab, bulletSpawnPosition);
        currentBullet.GetComponent<Rigidbody2D>().velocity = new Vector2(bulletSpeed * transform.localScale.x, 0);
        currentBullet.transform.parent = null;
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
        //Detectar si el player está sobre una plataforma móvil
        if (collision.gameObject.CompareTag("PlataformaMovil"))
        {
            actualPlatform = collision.transform;
            transform.parent = actualPlatform; //hacemos que es player sea hijo de la plataforma
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health -= 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CheckPoint"))
        {
            spawnPosition = collision.transform.position;
            checkpointReached = true;
        }
        if (collision.gameObject.CompareTag("FallLimit"))
        {
            Health -= 1;
            transform.position = spawnPosition;
        }
    }

    public int Health
    {
        get => _lifeManager.Health;
        set
        {
            _lifeManager.Health = value;
            healthDisplay.UpdateDisplay(value);
            if (value == 0)
            {
                Die();
            }

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
    private void Die()
    {
        if (checkpointReached)
        {
            transform.position = spawnPosition;
            Health = _lifeManager.StartingHealth;
        }
        else
        {
            gameManager.DeathScene();
        }


    }



}
