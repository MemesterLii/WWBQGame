using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class Player : MonoBehaviour, IDamagable
{
    //Basic setup
    public Collider2D hitbox;
    public Rigidbody2D rb;
    public Animator playerAnimator;
    [SerializeField] SpriteRenderer spriteRenderer;

    //Bomb
    [SerializeField] GameObject crosshair;
    [SerializeField] TrailRenderer trajectoryPrediction;
    [SerializeField] GameObject explosionSite;

    [SerializeField] LayerMask bombMask;

    [SerializeField] Bomb bombPrefab;

    //Player health
    [SerializeField] int health;
    int maxHealth = 20;
    [SerializeField] HealthBar playerHealthBar;
    public bool dead = false;
    [SerializeField] GameOver GameOverScreen;

    int healAmount;
    public bool stunned = false;

    //Score
    [SerializeField] GameObject score;
    [SerializeField] TextMeshProUGUI scoreValue;

    //Jump variables
    [SerializeField] LayerMask groundMask;
    float groundCheckDistance = 0.1f;

    int coyoteFrames = 5;
    int currentCoyoteFrames;

    bool jumpPressed = false;
    KeyCode jumpKey = KeyCode.Space;

    public float jumpForce = 8f;

    float fallMultiplier = 5f;
    float lowJumpMultiplier = 2.5f;

    //Horizontal movement variables
    float hInput;
    public float speed = 10f;
    [SerializeField] TrailRenderer playerTrail;
    bool facingRight = true;

    //Dash variables
    float dashForce = 40;
    bool dashPressed = false;
    KeyCode dashKey = KeyCode.LeftShift;
    float dashTime = 0.2f;
    float dashDirection;
    bool isDashing = false;
    int dashCooldown = 30;
    [SerializeField] DashBar dashBar;

    //Throwing variables
    bool throwPressed;
    KeyCode throwKey = KeyCode.Mouse0;
    float throwMultiplier = 3;
    public int maxThrowCooldown = 30;
    int throwCooldown = 30;
    [SerializeField] BombBar bombBar;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        health = maxHealth;
        playerHealthBar.SetHealth(health);
        playerTrail.enabled = false;
        bombPrefab.transform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        //Input buffering switches

        if(!jumpPressed)
        {
            jumpPressed = Input.GetKeyDown(jumpKey);
        }

        if (!dashPressed && dashCooldown == 0)
        {
            dashPressed = Input.GetKeyDown(dashKey);
        }

        if (!throwPressed && throwCooldown == 0)
        {
            throwPressed = Input.GetKeyDown(throwKey);
        }
    }

    //Called once every 0.02 seconds (50 times a second)
    void FixedUpdate()
    {
        hInput = Input.GetAxisRaw("Horizontal");

        //Flipping the player based on horizontal direction
        if (hInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (hInput < 0 && facingRight)
        {
            Flip();
        }

        if (dead)
        {
            bombPrefab.DisableSprite();

            spriteRenderer.enabled = false;
            playerAnimator.enabled = false;
            rb.simulated = false;
            hitbox.enabled = false;

            crosshair.SetActive(false);
            trajectoryPrediction.enabled = false;
            explosionSite.SetActive(false);

            playerHealthBar.Disable();
            bombBar.Disable();
            dashBar.Disable();

            int scoreValueInt = Int32.Parse(scoreValue.text);
            score.SetActive(false);

            Cursor.visible = true;

            GameOverScreen.Show(scoreValueInt);
        }
        else
        {
            playerAnimator.SetBool("IsStunned", stunned);
            PredictTrajectory();

            if (!stunned) 
            {
                playerAnimator.SetFloat("PlayerVVelocity", rb.velocity.y);
                playerAnimator.SetBool("IsOnGround", IsOnGround());
                playerAnimator.SetBool("IsDashing", isDashing);

                //Horizontal movement
                if (!isDashing)
                {
                    rb.velocity = new Vector2(hInput * speed, rb.velocity.y);
                    playerAnimator.SetFloat("PlayerHVelocity", Mathf.Abs(rb.velocity.x));
                }

                //Dash movement
                if (dashCooldown > 0)
                {
                    dashCooldown--;
                    dashBar.SetCooldown(dashCooldown);
                }

                if (dashPressed && Input.GetAxisRaw("Horizontal") != 0)
                {
                    dashPressed = false;
                    dashCooldown = 50;

                    isDashing = true;
                    rb.velocity = Vector2.zero;
                    dashDirection = Input.GetAxisRaw("Horizontal");

                    StartCoroutine(Dash());
                }

                IEnumerator Dash()
                {
                    rb.velocity = new Vector2(dashForce, 0) * dashDirection;
                    yield return new WaitForSeconds(dashTime);
                    rb.velocity = Vector2.zero;
                    isDashing = false;
                }

                //Controls coyote frame count
                if (IsOnGround())
                {
                    currentCoyoteFrames = coyoteFrames;
                }
                else if (currentCoyoteFrames > 0)
                {
                    currentCoyoteFrames--;
                }

                //Jumping
                if (jumpPressed)
                {
                    jumpPressed = false;
                    if (currentCoyoteFrames > 0 && !isDashing)
                    {
                        //MovementTutorial.SetActive(false);

                        rb.velocity = Vector2.up * jumpForce;
                    }
                }

                //Mario jumping
                if (rb.velocity.y < 0)
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }
                else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
                {
                    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                }

                //Throwing
                if (throwCooldown > 0)
                {
                    throwCooldown--;
                    bombBar.SetCooldown(throwCooldown);
                }

                if (throwPressed)
                {
                    //CameraBombTutorial.SetActive(false);

                    throwPressed = false;

                    throwCooldown = maxThrowCooldown;

                    Bomb bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);

                    bomb.rb.velocity = GetBombVelocity();
                }
            }
        }
    }

    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        facingRight = !facingRight;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            TakeDamage(-5);
            Destroy(collision.gameObject);
        }
    }
    public void TakeDamage(int amount)
    {
        healAmount = amount;
        StartCoroutine(DamageFlash());

        health -= amount;
        if (health > 20)
        {
            health = maxHealth;
        }

        playerHealthBar.SetHealth(health);

        if (health <= 0)
        {
            dead = true;
        }
    }

    IEnumerator DamageFlash()
    {
        if (healAmount < 0)
        {
            spriteRenderer.color = Color.green;
        }
        else
        {
            spriteRenderer.color = Color.red;
        }
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    void PredictTrajectory()
    {
        trajectoryPrediction.Clear();

        trajectoryPrediction.AddPosition(transform.position);

        Vector2 velocity = GetBombVelocity();

        Vector2 lastPosition = transform.position;

        for (int i = 0; i < 500; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(lastPosition, velocity, velocity.magnitude * Time.fixedDeltaTime, bombMask);
            if (hit.collider != null)
            {
                lastPosition = hit.point;
                break;
            }
            else {
                lastPosition += velocity * Time.fixedDeltaTime;
                velocity += Vector2.down * 9.81f * Time.fixedDeltaTime;
                trajectoryPrediction.AddPosition(new Vector3(lastPosition.x, lastPosition.y, transform.position.z));
            }
        }

        trajectoryPrediction.transform.position = lastPosition;
    }

    Vector2 GetBombVelocity()
    {
        Vector3 lookPosition = Input.mousePosition;
        lookPosition.z = transform.position.z - Camera.main.transform.position.z;

        lookPosition = Camera.main.ScreenToWorldPoint(lookPosition); //gets mouse position in world

        Vector2 lookDirection = lookPosition - transform.position;

        return lookDirection * throwMultiplier;
    }

    bool IsOnGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(hitbox.bounds.center/*start*/, hitbox.bounds.size, 0f, Vector2.down/*direction*/, groundCheckDistance/*magnitude*/, groundMask/*end*/);
        return hit.collider != null;
    }
}
