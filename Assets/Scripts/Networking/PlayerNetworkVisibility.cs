using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkVisibility : NetworkBehaviour {

    public float radius = 100f;
    public float updateInterval = 1;
    private float nextUpdateTime = 0;

    private HashSet<NetworkVisibility> objectsToUpdate = new HashSet<NetworkVisibility>();

    void Awake() {
        SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = radius;
    }

    void Update() {
        if (!NetworkServer.active) // Return if the server has not been started
            return;

        if (Time.time > nextUpdateTime) {
            UpdateObjects();
            nextUpdateTime += updateInterval;
        }

    }

    private void UpdateObjects() {
        foreach (NetworkVisibility visibility in objectsToUpdate) {
            visibility.RebuildObservers();
        }
        objectsToUpdate.Clear();
    }

    void OnTriggerEnter(Collider other) {
        NetworkVisibility visibility = other.GetComponent<NetworkVisibility>();
        if (visibility != null && connectionToClient != null) {
            visibility.AddObserver(connectionToClient);
            objectsToUpdate.Add(visibility);
        }
    }

    void OnTriggerExit(Collider other) {
        NetworkVisibility visibility = other.GetComponent<NetworkVisibility>();
        if (visibility != null && connectionToClient != null) {
            visibility.RemoveObserver(connectionToClient);
            objectsToUpdate.Add(visibility);
        }
    }

}
