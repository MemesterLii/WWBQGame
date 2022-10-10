using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D hitbox;
    [SerializeField] Bomb bomb;
    SpriteRenderer bombSpriteRenderer;

    float duration = 5f;

    [SerializeField] Rigidbody2D rb;
    bool hoverSwitchTime = true;
    int bobDirection = 1;
    float bobSpeed = 0.3f;

    void Update()
    {
        if (hoverSwitchTime)
        {
            StartCoroutine(HoverSwitch());
        }
    }

    IEnumerator HoverSwitch()
    {
        hoverSwitchTime = false;
        rb.velocity = Vector2.up * new Vector2(0, bobDirection * bobSpeed);
        yield return new WaitForSeconds(0.4f);
        bobDirection *= -1;
        hoverSwitchTime = true;
    }

    private void Awake()
    {
        bombSpriteRenderer = bomb.GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            StartCoroutine(DamageUpAndDestroy());
            hitbox.enabled = false;
            spriteRenderer.enabled = false;
        }
    }

    IEnumerator DamageUpAndDestroy()
    {
        bomb.explosionDamage = 10;
        bombSpriteRenderer.color = Color.red;
        bomb.transform.localScale = new Vector3(1.5f, 1.5f, 1);

        yield return new WaitForSeconds(duration);

        bomb.transform.localScale = Vector3.one;
        bombSpriteRenderer.color = Color.white;
        bomb.explosionDamage = 5;

        Destroy(gameObject);
    }
}
