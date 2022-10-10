using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tortoise : Enemy
{
    [SerializeField] Animator tortoiseAnimator;
    [SerializeField] HealthBar tortoiseHealthBar;

    [SerializeField] GameObject bullet;

    Vector2 playerPosition;
    float distanceFromPlayer;
    float disableDistance = 60f;

    bool readyToSwitch = true;
    bool inShell = false;

    bool readyToTryUnstuck = true;

    protected override bool vulnerable { get { return base.vulnerable && !inShell; } }

    int shellDuration;

    float bulletSpeed = 500;
    float bulletFireRate = 1;
    float nextTimeToShoot;

    float moveDirection = 1;
    float moveSpeed = 100;
    float hLookDistance = 0.5f;
    float vLookDistance = 0.4f;
    float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundMask;

    bool facingRight = true;

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        playerPosition = player.transform.position;
        distanceFromPlayer = Vector2.Distance(playerPosition, gameObject.transform.position);

        if (distanceFromPlayer > disableDistance)
        {
            Disable();
        }
    }

    protected override void Act()
    {
        if (player.dead != true)
        {
            tortoiseAnimator.SetBool("IsVulnerable", vulnerable);

            Vector2 origin = (Vector2)transform.position + Vector2.down * vLookDistance + new Vector2(moveDirection, 0) * hLookDistance;
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, groundMask);

            if (rb.velocity.x == 0 && readyToTryUnstuck && !inShell)
            {
                readyToTryUnstuck = false;

                StartCoroutine(TryUnstuck());
            }

            if (!hit.collider && !facingRight)
            {
                Flip();
            }
            else if (!hit.collider && facingRight)
            {
                Flip();
            }

            if (hit.collider == null)
            {
                rb.velocity = new Vector2(-moveDirection * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
                moveDirection = -moveDirection;
            }

            if (readyToSwitch && !inShell)
            {
                shellDuration = Random.Range(1, 3);
                StartCoroutine(Shell());
            }

            if (!inShell && !player.dead)
            {
                if (Time.time > nextTimeToShoot)
                {
                    nextTimeToShoot = Time.time + 1 / bulletFireRate;
                    Shoot();
                }

                Walk();
            }
            else
            {
                rb.velocity = Vector2.zero;
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

    IEnumerator TryUnstuck()
    {
        yield return new WaitForSeconds(0.3f);
        
        if (rb.velocity.x == 0 && !inShell)
        {
            rb.velocity += Vector2.up;
        }

        readyToTryUnstuck = true;
    }

    IEnumerator Shell()
    {
        inShell = true;
        readyToSwitch = false;
        tortoiseHealthBar.Disable();
        tortoiseAnimator.SetBool("IsShelling", true);

        yield return new WaitForSeconds(1);

        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        if (inShell)
        {
            tortoiseAnimator.SetBool("IsShelling", false);
            tortoiseAnimator.SetBool("IsHiding", true);

            yield return new WaitForSeconds(shellDuration);

            StartCoroutine(ComeOutShell());
        }
        else
        {
            tortoiseAnimator.SetBool("IsOuting", false);
            tortoiseAnimator.SetBool("IsVulnerable", true);

            yield return new WaitForSeconds(shellDuration);

            readyToSwitch = true;
        }
    }

    IEnumerator ComeOutShell()
    {
        tortoiseAnimator.SetBool("IsHiding", false);
        tortoiseAnimator.SetBool("IsOuting", true);

        yield return new WaitForSeconds(1);

        inShell = false;
        tortoiseHealthBar.Enable();
        StartCoroutine(Wait());
    }

    void Shoot()
    {
        GameObject BulletClone = Instantiate(bullet, transform.position, Quaternion.identity);
        BulletClone.transform.position = (Vector2)transform.position + new Vector2(0, 1.1f);
        Vector2 direction = player.transform.position - transform.position;
        BulletClone.GetComponent<Rigidbody2D>().AddForce(direction.normalized * bulletSpeed);
    }

    void Walk()
    {
        rb.velocity = new Vector2(moveDirection * moveSpeed * Time.fixedDeltaTime, rb.velocity.y);
    }
}
