using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;
using UnityEditorInternal;
#endif
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
