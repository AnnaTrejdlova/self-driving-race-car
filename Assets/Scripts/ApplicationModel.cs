using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationModel : MonoBehaviour
{
    static public Transmission transmission = Transmission.Manual;
    static public Dictionary<Assists, bool> assists = new Dictionary<Assists, bool>()
    {
        {Assists.ABS, true },
        {Assists.TSC, true },
        {Assists.ESC, true },
        {Assists.ASR, true },
    };
    static public string externalBrainsPath = "C:\\Users\\atrej\\Documents\\Programming\\ml-agents\\results";
}

public enum Transmission
{
    Manual,
    Automatic
}

public enum Assists
{
    ABS,
    TSC,
    ESC,
    ASR
}