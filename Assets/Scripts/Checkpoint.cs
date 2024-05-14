using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Event triggered when a car passes through this checkpoint
    public delegate void CheckpointPassed(Checkpoint checkpoint);
    public static event CheckpointPassed OnCheckpointPassed;

    // Method called when a car passes through this checkpoint
    private void OnTriggerEnter(Collider other)
    {
        // Notify that this checkpoint has been passed
        OnCheckpointPassed?.Invoke(this);
    }
}
