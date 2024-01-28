using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;

    public Transform playerBody; // Reference to the player body
    
    float xRotation = 0f; // The rotation around the x-axis

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime; // Time.deltaTime is the time between each frame
        float mouseY= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY; // Subtract the mouse y-axis movement from the xRotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp the xRotation between -90 and 90 degrees


        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // Rotate the camera around the x-axis
        playerBody.Rotate(Vector3.up * mouseX); // Rotate the player body around the y-axis
        
    }
}
