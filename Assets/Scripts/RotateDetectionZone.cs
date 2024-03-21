using UnityEngine;

public class RotateDetectionZone : MonoBehaviour
{
    public float baseRotationSpeed = 20.0f;
    private float currentRotationSpeed;
    public float rotationSpeedIncrease = 5.0f;
    private DialogueManager dialogueManager; 

    void Start()
    {
        currentRotationSpeed = baseRotationSpeed;
        GameObject gameManagerObject = GameObject.Find("GameManager");
        if (gameManagerObject != null)
        {
            dialogueManager = gameManagerObject.GetComponent<DialogueManager>();
        }
    }

    void Update()
    {
        if (dialogueManager != null && !dialogueManager.dialogueActive)
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