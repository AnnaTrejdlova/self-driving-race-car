using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using VehiclePhysics;

public class HandlePauseMenu : MonoBehaviour
{
    GameObject canvas;
    VPVehicleController vehicle;

    private void Start()
    {
        vehicle = FindObjectOfType<VPVehicleController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetSceneByName("PauseMenu").name == null)
            {
                SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
            }
            else
            {
                if (!canvas)
                {
                    canvas = FindObjectOfType<UIDocument>(true).gameObject;
                }

                if (!canvas.activeInHierarchy)
                {
                    canvas.SetActive(true);
                    vehicle.paused = true;
                    Time.timeScale = 0f;
                }
                else
                {
                    canvas.SetActive(false);
                    vehicle.paused = false;
                    Time.timeScale = 1f;
                    FindObjectOfType<GameManager>().Start();
                }
            }
        }
    }
}
