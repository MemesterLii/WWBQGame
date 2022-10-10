using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobUp : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D hitbox;
    [SerializeField] Player player;

    [SerializeField] BombBar goldBombBar;
    [SerializeField] BombTrail goldBombTrail;

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
            StartCoroutine(LobUpAndDestroy());
            hitbox.enabled = false;
            spriteRenderer.enabled = false;
        }
    }

    void Awake()
    {
        goldBombBar.Disable();
        goldBombTrail.Disable();
    }

    IEnumerator LobUpAndDestroy()
    {
        player.maxThrowCooldown = 0;
        goldBombBar.Enable();
        goldBombTrail.Enable();

        yield return new WaitForSeconds(duration);

        goldBombBar.Disable();
        goldBombTrail.Disable();
        player.maxThrowCooldown = 30;

        Destroy(gameObject);
    }
}
