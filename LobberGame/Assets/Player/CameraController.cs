using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Player player;
    [SerializeField] GameObject background;

    float xSmoothing = 0.6f; //horizontal camera speed (speed percentage of approach)
    float ySmoothing = 0.5f; //vertical camera speed (speed percentage of approach)

    float lookMultiplier = 2; //modifies desired camera position based on cursor position
    float speedMultiplier = 0; //takes player velocity and moves based on that

    float cameraZ = -15;

    void FixedUpdate()
    {
        Vector3 desiredPosition = GetDesiredPosition();

        float midPointX = Mathf.Lerp(transform.position.x, desiredPosition.x, xSmoothing);
        float midPointY = Mathf.Lerp(transform.position.y, desiredPosition.y, ySmoothing);

        transform.position = new Vector3(midPointX, midPointY, cameraZ);
    }

    Vector3 GetDesiredPosition()
    {
        Vector2 target = player.transform.position;
        target += player.rb.velocity * speedMultiplier;

        Vector3 lookPosition = Input.mousePosition;
        lookPosition.z = player.transform.position.z - cameraZ;

        lookPosition = mainCamera.ScreenToWorldPoint(lookPosition); //gets mouse position in world

        Vector2 lookDirection = lookPosition - player.transform.position;

        target += lookDirection.normalized * lookMultiplier;

        return new Vector3(target.x, target.y, cameraZ);
    }
}
