using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Checkpoint;

public class CheckpointController : MonoBehaviour
{
    // Reference to the checkpoints passed in this lap
    private List<Checkpoint> checkpointsPassed;

    private void Start()
    {
        checkpointsPassed = new();

        // Subscribe to the OnCheckpointPassed event when the car starts
        Checkpoint.OnCheckpointPassed += HandleCheckpointPassed;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when the car is destroyed to avoid memory leaks
        Checkpoint.OnCheckpointPassed -= HandleCheckpointPassed;
    }

    // A method to be called when the car passes through a checkpoint
    private void HandleCheckpointPassed(Checkpoint checkpoint)
    {
        checkpointsPassed.Add(checkpoint);

        // Do something when the car passes through a checkpoint
        Debug.Log("Checkpoint passed: " + checkpoint.name);
    }
}
