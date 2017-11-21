using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Tree : NetworkBehaviour {

    /*
    private NetworkIdentity networkIdentity;

    void Awake() {
        networkIdentity = GetComponent<NetworkIdentity>();
    }

    private float nextUpdate = 0f;
    void Update() {
        if (Time.time > nextUpdate) {
            networkIdentity.RebuildObservers(false);
            nextUpdate += 1f;
        }
    }
    */

    public override void OnSetLocalVisibility(bool vis) {
        Debug.Log("Visibility: " + vis);
    }

    /*
    public override bool OnRebuildObservers(HashSet<NetworkConnection> observers, bool initialize) {
        Collider[] hits = Physics.OverlapSphere(transform.position, 50f);
        foreach (Collider hit in hits) {
            NetworkIdentity identity = hit.GetComponent<NetworkIdentity>();
            if (identity != null && identity.connectionToClient != null) {
                observers.Add(identity.connectionToClient);
            }
        }
        return true;
    }
    */

}
