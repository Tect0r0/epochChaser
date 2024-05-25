using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Reference to the player's transform
    [SerializeField] private Transform target;
    // Safe area boundaries
    [SerializeField] private Vector3 offset;
    [SerializeField] private float minX, maxX, minY, maxY; // Boundaries


    private void FixedUpdate()
    {
        Follow();
    }
    private void Follow()
    {
        Vector3 cameraPosition = target.position + offset;

        // Clamp the camera's position
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, minY, maxY);

        transform.position = cameraPosition;
    }
}