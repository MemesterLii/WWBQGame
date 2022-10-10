using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class KillBorder : MonoBehaviour
{
    [SerializeField] Player player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 7)
        {
            player.dead = true;
        }
    }
}
