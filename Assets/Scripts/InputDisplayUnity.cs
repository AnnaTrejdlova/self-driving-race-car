using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VehiclePhysics;

public class InputDisplayUnity : MonoBehaviour
{
    public GameObject Car;
    public GameObject upArrow;
    public GameObject downArrow;
    public GameObject leftArrow;
    public GameObject rightArrow;

    private void Update()
    {
        // Check the state of arrow keys
        bool upPressed = Input.GetAxis("Vertical") > 0f;
        bool downPressed = Input.GetAxis("Vertical") < 0f;
        bool leftPressed = Input.GetAxis("Horizontal") < 0f;
        bool rightPressed = Input.GetAxis("Horizontal") > 0f;

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
