using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{
    Scene selectedScene;
    Dictionary<Scene, string> scenes;
    Transmission transmission;

    private void OnEnable()
    {
        selectedScene = Scene.WallRing;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += StartButton;

        Button exitButton = root.Q<Button>("ExitButton");
        exitButton.clicked += ExitButton;
    }

    private void Start()
    {
        scenes = new Dictionary<Scene, string>
        {
            {Scene.WallRing, "KartClassic_Training_followTarget" },
            {Scene.Nordschleife, "Nordschleife Location Scene" }
        };

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        List<RadioButton> radioButtonList;

        // Map group
        RadioButtonGroup mapGroup = root.Q<RadioButtonGroup>("MapGroup");

        radioButtonList = mapGroup.Query<RadioButton>().Build().ToList();
        foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
        {
            radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                if (evt.newValue)
                {
                    (evt.target as RadioButton).AddToClassList("RadioButton--selected");
                    mapGroup.value = item.i;
                    selectedScene = (Scene)mapGroup.value;
                }
                else
                {
                    (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
                }
            });
        }

        // Transmission group
        RadioButtonGroup transmissionGroup = root.Q<RadioButtonGroup>("TransmissionGroup");

        radioButtonList = transmissionGroup.Query<RadioButton>().Build().ToList();
        foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
        {
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

        // Assists group
        VisualElement assistsGroup = root.Q<VisualElement>("AssistsGroup");

        List<Toggle> toggleList = assistsGroup.Query<Toggle>().Build().ToList();
        foreach (var item in toggleList.Select((value, i) => new { i, value }))
        {
            toggleList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                ApplicationModel.assists[(Assists)item.i] = evt.newValue;
            });
        }
    }

    //private void InitiateRadioButtons<enumType>(string groupName, out enumType variable)
    //{
    //    VisualElement root = GetComponent<UIDocument>().rootVisualElement;
    //    RadioButtonGroup buttonGroup = root.Q<RadioButtonGroup>(groupName);

    //    List<RadioButton> radioButtonList = buttonGroup.Query<RadioButton>().Build().ToList();
    //    foreach (var item in radioButtonList.Select((value, i) => new { i, value }))
    //    {
    //        radioButtonList[item.i].RegisterCallback<ChangeEvent<bool>>((evt) =>
    //        {
    //            if (evt.newValue)
    //            {
    //                (evt.target as RadioButton).AddToClassList("RadioButton--selected");
    //                buttonGroup.value = item.i;
    //                selectedScene = (Scene)buttonGroup.value;
    //            }
    //            else
    //            {
    //                (evt.target as RadioButton).RemoveFromClassList("RadioButton--selected");
    //            }
    //        });
    //    }
    //}

    private void StartButton()
    {
        SceneManager.LoadScene(scenes[selectedScene]);
    }

    private void ExitButton()
    {
        Application.Quit();
    }

    enum Scene
    {
        WallRing,
        Nordschleife
    }
}
