using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D hitbox;
    [SerializeField] Player player;

    [SerializeField] ParticleSystem jumpUpParticles;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            StartCoroutine(JumpUpAndDestroy());
            hitbox.enabled = false;
            spriteRenderer.enabled = false;
        }
    }

    IEnumerator JumpUpAndDestroy()
    {
        player.jumpForce = 12;
        jumpUpParticles.Play();

        yield return new WaitForSeconds(duration);

        jumpUpParticles.Stop();
        player.jumpForce = 8;

        Destroy(gameObject);
    }
}
