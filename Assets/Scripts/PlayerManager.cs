using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{    
    // Start is called before the first frame update
    void Start()
    {   
        transform.position = GameManager.instance.gameState.PlayerPosition3D;
        transform.rotation = Quaternion.Euler(GameManager.instance.gameState.PlayerRotation3D);
        InputSystem.onActionChange += InputActionChangeCallback;
    }

    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction receivedInputAction = (InputAction) obj;
            InputDevice lastDevice = receivedInputAction.activeControl.device;

            bool isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
            GameManager.instance.isUsingController = !isKeyboardAndMouse;
            //If needed we could check for "XInputControllerWindows" or "DualShock4GamepadHID"
            //Maybe if it Contains "controller" could be xbox layout and "gamepad" sony? More investigation needed
        }
    }
}
