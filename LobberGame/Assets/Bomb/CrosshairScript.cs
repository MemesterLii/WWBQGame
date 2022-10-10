using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairScript : MonoBehaviour
{
    float crosshairZ;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = crosshairZ - Camera.main.transform.position.z - 1;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.position = mousePosition;
    }
}
