using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkVisibility : NetworkBehaviour {

    private HashSet<NetworkConnection> observers = new HashSet<NetworkConnection>();
    private NetworkIdentity networkIdentity;

    void Awake() {
        networkIdentity = GetComponent<NetworkIdentity>();
    }

    // Called when new players enters the game
    public override bool OnCheckObserver(NetworkConnection conn) {
        return false;
    }

    // Called when RebuildObservers is invoked
    public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize) {
        foreach (NetworkConnection connection in this.observers) {
            if (connection.isReady)
                observers.Add(connection);
        }
        return true;
    }

    public bool AddObserver(NetworkConnection connection) {
        return observers.Add(connection);
    }

    public bool RemoveObserver(NetworkConnection connection) {
        return observers.Remove(connection);
    }

    public void RebuildObservers() {
        networkIdentity.RebuildObservers(false);
    }

    public override void OnSetLocalVisibility(bool vis) {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
            renderer.enabled = vis;
        }
    }

}
