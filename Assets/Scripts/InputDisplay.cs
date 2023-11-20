using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VehiclePhysics;

public class InputDisplay : MonoBehaviour
{
    public GameObject Car;
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject leftArrow;
    public GameObject rightArrow;

    private void Update()
    {
        var _input = Car.GetComponent<VPStandardInput>();
        // Check the state of arrow keys
        bool upPressed = Input.GetKey(KeyCode.W) || _input.externalThrottle >= 1f;
        bool downPressed = Input.GetKey(KeyCode.S) || _input.externalBrake >= 1f;
        bool leftPressed = Input.GetKey(KeyCode.A) || _input.externalSteer <= -1f;
        bool rightPressed = Input.GetKey(KeyCode.D) || _input.externalSteer >= 1f;

        // Update UI squares based on key states
        UpdateSquareColor(upArrow, upPressed);
        UpdateSquareColor(downArrow, downPressed);
        UpdateSquareColor(leftArrow, leftPressed);
        UpdateSquareColor(rightArrow, rightPressed);
    }

    private void UpdateSquareColor(GameObject square, bool isPressed)
    {
        // Change color based on key state
        Image image = square.GetComponent<Image>();
        if (image != null)
        {
            image.color = isPressed ? Color.green : Color.red;
        }
    }
}
