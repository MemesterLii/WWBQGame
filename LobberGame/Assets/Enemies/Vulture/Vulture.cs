using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vulture : Enemy
{
    State currentState = State.Idle;
    Vector2 startPosition;
    [SerializeField] ParticleSystem particles;

    [SerializeField] LayerMask explosionMask;
    float explosionRadius = 5;
    float explosionDuration = 2;
    float explosionForce = 100;

    float horzExtent;
    float speed = 600f;
    bool positionReached = false;

    [SerializeField] LayerMask obstacleMask;
    bool clearLineOfSight = false;

    Vector2 playerDirection;
    float diveSpeed = 1200f;

    enum State
    {
        Idle,
        Flying,
        Watching,
        Diving,
    }

    void CheckForTransition()
    {
        switch (currentState)
        {
            case State.Idle:
                currentState = State.Flying;
                break;
            case State.Flying:
                if (positionReached)
                {
                    currentState = State.Watching;
                }
                break;
            case State.Watching:
                if (transform.position.x > Camera.main.transform.position.x + horzExtent + 2 || transform.position.x < Camera.main.transform.position.x - horzExtent - 2)
                {
                    positionReached = false;
                    currentState = State.Flying;
                }
                if (clearLineOfSight)
                {
                    currentState = State.Diving;
                }
                break;
            case State.Diving:
                break;
        }
    }

    void DoStateAction()
    {
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Flying:
                Flying();
                break;
            case State.Watching:
                Watching();
                break;
            case State.Diving:
                Diving();
                break;
        }
    }

    void Flying()
    {
        if (transform.position.x > Camera.main.transform.position.x + horzExtent - 10)
        {
            rb.velocity = Vector2.left * speed * Time.fixedDeltaTime;
        }
        else if (transform.position.x < Camera.main.transform.position.x - horzExtent + 10)
        {
            rb.velocity = Vector2.right * speed * Time.fixedDeltaTime;
        }
        else
        {
            rb.velocity = Vector2.zero;
            positionReached = true;
        }
    }

    void Watching()
    {
        playerDirection = player.transform.position - transform.position;
        float playerDistance = Mathf.Sqrt((playerDirection.x * playerDirection.x) + (playerDirection.y * playerDirection.y));

        RaycastHit2D hit = Physics2D.BoxCast(hitbox.bounds.center/*start*/, hitbox.bounds.size, 0f, playerDirection.normalized/*direction*/, playerDistance/*magnitude*/, obstacleMask/*end*/);
        if (hit.collider == null)
        {
            clearLineOfSight = true;
        }
    }

    void Diving()
    {
        rb.velocity = playerDirection.normalized * diveSpeed * Time.fixedDeltaTime;
    }

    protected override void Awake()
    {
        maxHealth = 1;
        damage = 7;
        base.Awake();
        startPosition = (Vector2)transform.position;
        horzExtent = Camera.main.orthographicSize * Screen.width / Screen.height; //horizontal distance to edge of screen from center
    }

    protected override void Act()
    {
        CheckForTransition();
        DoStateAction();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    void Explode()
    {
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        spriteRenderer.enabled = false;
        enemyHealthBar.Disable();
        hitbox.enabled = false;
        particles.Play();

        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(explosionMask);
        int numberHit = Physics2D.OverlapCircle(transform.position, explosionRadius, playerFilter, results);

        if (numberHit > 0)
        {
            foreach (Collider2D collider in results)
            {
                //Adding explosion knockback
                if (collider.gameObject.layer != 9)
                {
                    Vector2 explosionDirection = collider.transform.position - transform.position;
                    collider.GetComponent<Rigidbody2D>().AddForce(explosionDirection.normalized * explosionForce);
                }

                //Adding explosion damage
                IDamagable damagable = collider.gameObject.GetComponent<IDamagable>(); //checks to see if damagable
                if (damagable != null && collider.gameObject.layer == 7)
                {
                    damagable.TakeDamage(damage);
                }
            }
        }

        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(explosionDuration);
        Destroy(gameObject);
    }
}
