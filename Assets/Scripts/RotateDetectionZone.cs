using UnityEngine;

public class RotateDetectionZone : MonoBehaviour
{
    public float rotationSpeed = 50.0f; // Rotation speed in degrees per second

    void Update()
    {
        // Rotate around the up axis of the gameObject
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}