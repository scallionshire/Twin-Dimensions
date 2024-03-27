using UnityEngine;

public class RotateDetectionZone : MonoBehaviour
{
    public float baseRotationSpeed = 20.0f;
    private float currentRotationSpeed;
    public float rotationSpeedIncrease = 5.0f;

    void Start()
    {
        currentRotationSpeed = baseRotationSpeed;
    }

    void Update()
    {
        if (!GameManager.instance.isFrozen)
        {
            transform.Rotate(Vector3.up, currentRotationSpeed * Time.deltaTime);
        }
    }

    public void IncreaseRotationSpeed()
    {
        currentRotationSpeed += rotationSpeedIncrease;
        Debug.Log("Rotation Speed: " + currentRotationSpeed);
    }
}