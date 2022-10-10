using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] Collider2D hitbox;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;

    [SerializeField] TrailRenderer trailRenderer;

    float bulletDuration = 3;
    int bulletDamage = 2;

    void Start()
    {
        StartCoroutine(WaitAndDestroy());
    }

    IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(bulletDuration);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HitAndDestroy(collision.gameObject);
    }

    void HitAndDestroy(GameObject target)
    {
        IDamagable damagable = target.GetComponent<IDamagable>(); //checks to see if damagable
        if (damagable != null)
        {
            damagable.TakeDamage(bulletDamage);
        }

        Destroy(gameObject);
    }
}
