using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D hitbox;
    [SerializeField] Player player;
    [SerializeField] TrailRenderer playerTrail;

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
            StartCoroutine(SpeedUpAndDestroy());
            hitbox.enabled = false;
            spriteRenderer.enabled = false;
        }
    }

    IEnumerator SpeedUpAndDestroy()
    {
        player.speed = 20f;
        playerTrail.enabled = true;

        yield return new WaitForSeconds(duration);

        playerTrail.enabled = false;
        player.speed = 10f;

        Destroy(gameObject);
    }
}
