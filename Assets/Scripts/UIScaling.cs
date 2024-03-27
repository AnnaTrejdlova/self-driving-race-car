using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Rendering;

public class UIScaling : MonoBehaviour
{
    private Rect currentResolution;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Window resolution: " + gameObject.GetComponent<Canvas>().pixelRect.width + "x" + gameObject.GetComponent<Canvas>().pixelRect.height);
        currentResolution = gameObject.GetComponent<Canvas>().pixelRect;

        float windowResVertical = gameObject.GetComponent<Canvas>().pixelRect.height; // Screen.currentResolution.height is native screen
        float scaleFactor = windowResVertical / 1080;
        Debug.Log("Resolution scale factor: " + scaleFactor);

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).name == "Markers")
            {
                gameObject.transform.GetChild(i).localScale = Vector3.one * scaleFactor * 0.7f + Vector3.one * 0.3f;
            } else
            {
                gameObject.transform.GetChild(i).localScale = Vector3.one * scaleFactor;
            }
        }
    }

    // Update scaling when resolution is changed
    void Update()
    {
        if (currentResolution.height != gameObject.GetComponent<Canvas>().pixelRect.height)
        {
            Debug.Log("Window resolution: " + gameObject.GetComponent<Canvas>().pixelRect.width + "x" + gameObject.GetComponent<Canvas>().pixelRect.height);

            float windowResVertical = gameObject.GetComponent<Canvas>().pixelRect.height; // Screen.currentResolution.height is native screen
            float scaleFactor = windowResVertical / 1080;
            Debug.Log("Resolution scale factor: " + scaleFactor);

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                if (gameObject.transform.GetChild(i).name == "Markers")
                {
                    gameObject.transform.GetChild(i).localScale = Vector3.one * scaleFactor * 0.7f + Vector3.one * 0.3f;
                }
                else
                {
                    gameObject.transform.GetChild(i).localScale = Vector3.one * scaleFactor;
                }
            }

            currentResolution = gameObject.GetComponent<Canvas>().pixelRect;
        }
    }
}
