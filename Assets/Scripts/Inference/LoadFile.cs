using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadFile : MonoBehaviour
{
    public TextMeshProUGUI text;

    [HideInInspector]
    public string selectedBrainPath;

    TMP_Dropdown m_Dropdown;

    // Start is called before the first frame update
    void Start()
    {
        GetBrainsList();

        m_Dropdown = GetComponent<TMP_Dropdown>();
        if (m_Dropdown)
            SetOptions();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetBrainsList()
    {
        text.text = string.Join('\n', LoadBrainsList());
    }

    static string[] LoadBrainsList()
    {
        return Directory.GetFiles(ApplicationModel.externalBrainsPath, "*.onnx", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(ApplicationModel.externalBrainsPath, file)).ToArray();
    }

    static string[] LoadBrainsList(string folder)
    {
        string basePath = Path.Combine(ApplicationModel.externalBrainsPath, folder);
        return Directory.GetFiles(basePath, "*.onnx", SearchOption.AllDirectories)
            .Select(file => Path.GetRelativePath(basePath, file)).ToArray();
    }

    public void OnDropdownChanged(int index)
    {
        Debug.Log("index: " + m_Dropdown.value);
        Debug.Log(m_Dropdown.options[m_Dropdown.value].text);
        string[] brains = LoadBrainsList(m_Dropdown.options[m_Dropdown.value].text);
        text.text = string.Join('\n', brains);
        selectedBrainPath = brains[0];
    }

    public void SetOptions()
    {
        m_Dropdown.ClearOptions();
        m_Dropdown.options = LoadBrainsList()
            .Where(file => file.IndexOf(Path.DirectorySeparatorChar) == file.LastIndexOf(Path.DirectorySeparatorChar))
            .Select(file => new TMP_Dropdown.OptionData(file.Substring(0, file.IndexOf(Path.DirectorySeparatorChar))))
            .ToList();
        m_Dropdown.value = m_Dropdown.options.Count - 1;
        OnDropdownChanged(0);
    }
}
