using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected Collider2D hitbox;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected SpriteRenderer spriteRenderer;

    [SerializeField] protected Player player;
    [SerializeField] protected Bomb bomb;

    [SerializeField] protected bool AIEnabled;

    protected bool iFrames = false;
    protected int health;
    [SerializeField] protected int maxHealth = 20;
    [SerializeField] protected HealthBar enemyHealthBar;

    protected int damage = 5;
    protected float stunDuration = 1.5f;
    protected int knockbackForce = 500;

    Vector2 startPosition;

    protected Color currentColor = Color.white;
    protected Color flashColor = Color.red;
    protected float flashDuration = 0.2f;

    [SerializeField] protected GameObject healthDrop;

    protected virtual bool vulnerable { get { return !iFrames; } }

    void OnBecameVisible()
    {
        transform.position = startPosition;
        Enable();
    }

    protected void Enable()
    {
        AIEnabled = true;
    }

    protected void Disable()
    {
        AIEnabled = false;
    }

    protected virtual void Awake()
    {
        health = maxHealth;
        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (AIEnabled && !player.dead)
        {
            Act();
        }

        if (player.dead)
        {
            Death();
        }
    }

    protected virtual void Act()
    {
        //Function specified in inheritor classes.
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            player.TakeDamage(damage);

            StartCoroutine(Knockback());

            StartCoroutine(Stun());
        }
    }

    IEnumerator Knockback()
    {
        Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
        float dot = Vector2.Dot(knockbackDirection, Vector2.right);
        if (dot > 0)
        {
            knockbackDirection = new Vector2(1, 0.41f);
        }
        else
        {
            knockbackDirection = new Vector2(-1, 0.41f);
        }
        player.GetComponent<Rigidbody2D>().AddForce(knockbackDirection.normalized * knockbackForce);

        yield return new WaitForSeconds(stunDuration - 1);

        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    IEnumerator Stun()
    { 
        player.stunned = true;
        yield return new WaitForSeconds(stunDuration);
        player.stunned = false;
    }

    public void TakeDamage(int amount)
    {
        if (vulnerable && AIEnabled)
        {
            iFrames = true;
            StartCoroutine(DamageFlash());

            health -= amount;
            enemyHealthBar.SetHealth(health);
        }

        if (health <= 0)
        {
            Death();
        }
    }

    IEnumerator DamageFlash()
    {
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = currentColor;
        iFrames = false;
    }

    protected virtual void Death()
    {
        Instantiate(healthDrop, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
