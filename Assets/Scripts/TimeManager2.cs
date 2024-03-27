﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TimeManager2 : MonoBehaviour
{ 
    public float TotalTime { get; private set; }
    public float accTime { get; private set; }
    public bool IsOver { get; private set; }

    public TextMeshProUGUI timerText;

    private bool raceStarted;

    private List<float> lapTimes;
    private int lapCount;
    private float bestSessionTime;
    private float bestAllTime;

    private TextMeshProUGUI BestSessionTimeText;
    private TextMeshProUGUI BestAllTimeText;
    private TextMeshProUGUI LastLapTimeText;

    private static string filePath;

    private async void Awake()
    {
        // Setup
        accTime = 0f;
        lapTimes = new List<float>();

        BestSessionTimeText = GameObject.Find("BestSessionTime").GetComponent<TextMeshProUGUI>();
        BestAllTimeText = GameObject.Find("BestAllTime").GetComponent<TextMeshProUGUI>();
        LastLapTimeText = GameObject.Find("LastLapTime").GetComponent<TextMeshProUGUI>();

        // Read best time
        filePath = Application.persistentDataPath + "/lapTimes.txt"; // Application.persistentDataPath = %userprofile%\AppData\LocalLow\<companyname>\
        var items = await ReadFromFileAsync();

        bestAllTime = (items.Length == 0) ? int.MaxValue : items.Min();
        Debug.Log("bestAllTime: " + bestAllTime);
        BestAllTimeText.text = "Total Best: " + string.Format("{0:0}:{1:00.000}", Math.Floor(bestAllTime / 60), bestAllTime % 60);
        bestSessionTime = float.MaxValue;
    }

    void Update()
    {
        if (!raceStarted) return;
        
        // after race started
        accTime += Time.deltaTime;

        timerText.gameObject.SetActive(true);
        timerText.text = string.Format("{0:0}:{1:00.000}", Math.Floor(accTime / 60), accTime % 60);
    }

    public void StartRace()
    {
        raceStarted = true;
    }

    public void StopRace() {
        raceStarted = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (!raceStarted && other.CompareTag("Player"))
        {
            // Start race
            accTime = 0f;
            StartRace();
        }
        if (raceStarted && other.CompareTag("Player") && accTime > 5)
        {
            // New lap
            lapCount++;
            float lapTime = accTime;
            lapTimes.Append(lapTime);
            LastLapTimeText.text = "Last Lap: " + string.Format("{0:0}:{1:00.000}", Math.Floor(lapTime / 60), lapTime % 60);

            if (lapTime < bestSessionTime)
            { // New best time
                bestSessionTime = lapTime;
                WriteToFile(bestSessionTime.ToString());
                BestSessionTimeText.text = "Current Best: " + string.Format("{0:0}:{1:00.000}", Math.Floor(bestSessionTime / 60), bestSessionTime % 60);
            }

            if (bestSessionTime < bestAllTime)
            {
                
            }

            accTime = 0f;
            StartRace();
        }
    }

    static void WriteToFile(string str, bool appendFile = true)
    {
        using (var writer = new StreamWriter(filePath, appendFile))
        {
            writer.Write(str + "\n");
        }
    }

    static void WriteToFile(string[] str, bool appendFile = true)
    {
        using (var writer = new StreamWriter(filePath, appendFile))
        {
            writer.Write(string.Join("\n", str));
            writer.Write("\n");
        }
    }

    static async Task<float[]> ReadFromFileAsync()
    {
        using var reader = new StreamReader(filePath);
        string str = await reader.ReadToEndAsync();
        return str.SplitLines().Select(line => float.Parse(line)).ToArray();
    }
}

