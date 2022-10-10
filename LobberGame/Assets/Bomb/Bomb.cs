using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bomb : MonoBehaviour
{
    public Collider2D hitbox;
    public Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] TrailRenderer goldTrailRenderer;
    [SerializeField] ParticleSystem particles;

    float zRotationSpeed = -360;

    bool exploded;

    [SerializeField] LayerMask explosionMask;

    float explosionRadius = 5;
    float explosionDuration = 2;
    float explosionForce = 100;
    public int explosionDamage = 5;

    private void Awake()
    {
        spriteRenderer.enabled = true;
    }

    void Update()
    {
        if (!exploded)
        {
            Quaternion zRotation = Quaternion.Euler(0, 0, zRotationSpeed * Time.deltaTime);
            gameObject.transform.rotation *= zRotation;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    void Explode()
    {
        //Removes bomb and makes explosion animation
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        exploded = true;
        trailRenderer.enabled = false;
        goldTrailRenderer.enabled = false;
        spriteRenderer.enabled = false;
        hitbox.enabled = false;
        particles.Play();

        //Gets all explodable objects that were hit
        List<Collider2D> results = new List<Collider2D>();
        ContactFilter2D enemyFilter = new ContactFilter2D();
        enemyFilter.SetLayerMask(explosionMask);
        int numberHit = Physics2D.OverlapCircle(transform.position, explosionRadius, enemyFilter, results);

        //Debug.Log(numberHit);

        //HashSet<Collider2D> resultsSet = new HashSet<Collider2D>(from r in results select r);

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
                if (damagable != null && collider.gameObject.layer != 7)
                {
                    damagable.TakeDamage(explosionDamage);
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

    public void DisableSprite()
    {
        spriteRenderer.enabled = false;
    }
}
