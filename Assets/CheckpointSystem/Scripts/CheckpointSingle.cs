using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehiclePhysics;

public class CheckpointSingle : MonoBehaviour {

    [HideInInspector] public TrackCheckpoints trackCheckpoints;
    //private MeshRenderer meshRenderer;

    private void Awake() {
        //meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        //Hide();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "Sport Coupe Collider Base") {
            trackCheckpoints.CarThroughCheckpoint(this, other.transform.parent.parent);
        }
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints) {
        this.trackCheckpoints = trackCheckpoints;
    }

    //public void Show() {
    //    meshRenderer.enabled = true;
    //}

    //public void Hide() {
    //    meshRenderer.enabled = false;
    //}

}
