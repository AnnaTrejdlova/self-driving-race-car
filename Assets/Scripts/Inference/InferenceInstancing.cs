using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using Unity.Sentis;
using UnityEngine;

public class InferenceInstancing : MonoBehaviour
{
    public GameObject Dropdown;
    public GameObject Car;

    string m_SelectedBrainPath;
    List<GameObject> m_CarPool;

    // Start is called before the first frame update
    void Start()
    {
        m_SelectedBrainPath = Dropdown.GetComponent<LoadFile>().selectedBrainPath;

        for (int i = 0; i < 4; i++)
        {
            m_CarPool.Add(Instantiate(Car)); // USE SOME TRICK INSTEAD
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
