
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseMenuUI : MonoBehaviour
{
    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button continueButton = root.Q<Button>("ContinueButton");
        continueButton.clicked += ContinueButton;

        Button exitButton = root.Q<Button>("ExitButton");
        exitButton.clicked += ExitToMainMenuButton;

        // Assists group
        VisualElement assistsGroup = root.Q<VisualElement>("AssistsGroup");

        List<Toggle> toggleList = assistsGroup.Query<Toggle>().Build().ToList();
        foreach (var item in toggleList.Select((value, i) => new { i, value }))
        {
            toggleList[item.i].value = ApplicationModel.assists[(Assists)item.i];
            toggleList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                ApplicationModel.assists[(Assists)item.i] = evt.newValue;
            });
        }

        // Transmission group
        RadioButtonGroup transmissionGroup = root.Q<RadioButtonGroup>("TransmissionGroup");

        List<RadioButton> radioButtonList = transmissionGroup.Query<RadioButton>().Build().ToList();

        foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
        {
            if (item.i == (int)ApplicationModel.transmission)
            {
                radioButtonList[item.i].value = true;
                radioButtonList[item.i].AddToClassList("RadioButton--selected");
                transmissionGroup.value = item.i;
            }
            else
            {
                radioButtonList[item.i].value = false;
                radioButtonList[item.i].RemoveFromClassList("RadioButton--selected");
            }
            radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.newValue)
                {
                    (evt.target as RadioButton).AddToClassList("RadioButton--selected");
                    transmissionGroup.value = item.i;
                    ApplicationModel.transmission = (Transmission)transmissionGroup.value;
                }
                else
                {
                    (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
                }
            });
        }
    }

    public void ContinueButton()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ExitToMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }

    public void ExitToDesktopButton()
    {
        Application.Quit();
    }
}
