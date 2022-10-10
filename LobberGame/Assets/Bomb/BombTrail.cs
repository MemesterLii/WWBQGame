using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrail : MonoBehaviour
{
    [SerializeField] TrailRenderer bombTrail;

    public void Enable()
    {
        bombTrail.enabled = true;
    }

    public void Disable()
    {
        bombTrail.enabled = false;
    }
}
