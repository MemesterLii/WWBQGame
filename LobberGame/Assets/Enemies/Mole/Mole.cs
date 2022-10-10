using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mole : Enemy
{
    protected override bool vulnerable { get { return base.vulnerable && !underground; } }

    Vector2 playerPosition;

    Vector2 startPosition;
    float spawnPlayerDistance;
    float disableDistance = 60f;

    [SerializeField] string aboveGround = "Enemy";
    [SerializeField] string belowGround = "UndergroundMole";

    [SerializeField] Animator moleAnimator;

    float distance;
    bool notUnderPlayer = true;

    Vector2 moveDirection;
    float speed = 150f;

    bool readyToTunnel = true;
    bool underground = false;

    float hLookDistance = 0.5f;
    float vLookDistance = 0.4f;
    float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundMask;

    protected override void Awake()
    {
        base.Awake();
        moleAnimator.SetBool("IsAboveGround", true);
        startPosition = (Vector2)transform.position;
    }

    void Update()
    {
        playerPosition = player.transform.position;
        spawnPlayerDistance = Vector2.Distance(playerPosition, startPosition);

        if (spawnPlayerDistance > disableDistance)
        {
            transform.position = startPosition;
            Disable();
        }
    }

    protected override void Act()
    {
        //Stops mole from falling off platform
        Vector2 leftOrigin = (Vector2)transform.position + Vector2.down * vLookDistance + Vector2.left * hLookDistance;
        RaycastHit2D leftHit = Physics2D.Raycast(leftOrigin, Vector2.down, groundCheckDistance, groundMask);
        Vector2 rightOrigin = (Vector2)transform.position + Vector2.down * vLookDistance + Vector2.right * hLookDistance;
        RaycastHit2D rightHit = Physics2D.Raycast(rightOrigin, Vector2.down, groundCheckDistance, groundMask);

        if (leftHit.collider == null)
        {
            rb.velocity = Vector2.right * (speed - 100f);
        }
        else if (rightHit.collider == null)
        {
            rb.velocity = Vector2.left * (speed - 100f);
        }

        if (readyToTunnel)
        {
            StartCoroutine(digAndChase());
        }

        if (!underground || spriteRenderer.color == Color.black)
        {
            rb.velocity = Vector2.zero;
        }

        distance  = Vector2.Distance(playerPosition, gameObject.transform.position);
        if (distance < 1 && distance > -1)
        {
            notUnderPlayer = false;
        }
        else
        {
            notUnderPlayer = true;
        }
    }

    IEnumerator digAndChase()
    {
        readyToTunnel = false;

        underground = true;
        gameObject.layer = LayerMask.NameToLayer(belowGround);
        enemyHealthBar.Disable();
        moleAnimator.SetBool("IsAboveGround", false); //placeholder for tunnelling animation
        moleAnimator.SetBool("IsDiggingDown", true);
        yield return new WaitForSeconds(1.5f);
        moleAnimator.SetBool("IsDiggingDown", false);
        moleAnimator.SetBool("IsUnderground", true);

        while (notUnderPlayer && AIEnabled)
        {
            if (player.transform.position.x < transform.position.x)
            {
                moveDirection = Vector2.left;
            }
            else
            {
                moveDirection = Vector2.right;
            }
            rb.velocity = moveDirection * speed * Time.fixedDeltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;
        moleAnimator.SetBool("IsUnderground", false);
        moleAnimator.SetBool("IsPoppingOut", true);//placeholder for tunnelling animation
        yield return new WaitForSeconds(0.5f);
        gameObject.layer = LayerMask.NameToLayer(aboveGround);
        underground = false;
        enemyHealthBar.Enable();
        moleAnimator.SetBool("IsPoppingOut", false);
        moleAnimator.SetBool("IsAboveGround", true);

        yield return new WaitForSeconds(1.5f);

        readyToTunnel = true;
    }
}
