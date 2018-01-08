using UnityEngine;
using UnityEngine.Networking;

[DisallowMultipleComponent]
public class CustomNetworkManager : NetworkManager {

    // size and seed that is stored on the server and sent to clients
    public int worldSize = 5;
    public int worldSeed = 0;

    private static CustomNetworkManager instance;
    public static CustomNetworkManager Instance {
        get { return instance; }
    }

    void Awake() {
        instance = this;
    }

    public override void OnServerConnect(NetworkConnection conn) {
        WorldGenMessage message = new WorldGenMessage();
        Debug.Log("Client connected: " + conn.connectionId);
        message.size = worldSize;
        message.seed = worldSeed;
        conn.Send(100, message);
        Debug.Log("WorldGen message sent to client " + conn.connectionId);
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        NetworkServer.DestroyPlayersForConnection(conn);
        Debug.Log("Client disconnected: " + conn.connectionId);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId) {
        Vector3 spawnPoint = new Vector3(0, 100, 0);
        Debug.Log("Spawning player at " + spawnPoint.ToString());
        GameObject player = (GameObject)Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player) {
        base.OnServerRemovePlayer(conn, player);
        Debug.Log("Removing player " + conn.address);
    }

#if CLIENT
    public override void OnClientDisconnect(NetworkConnection conn) {
        // TODO: Improve this method
        LoadingScreen.SetSubtitle("Failed to connect...");
    }
#endif

    public void SpawnPlayer() {
        if (ClientScene.ready)
            ClientScene.AddPlayer(client.connection, 0);
    }

    public void KillPlayer() {
        if (ClientScene.ready)
            ClientScene.RemovePlayer(0);
    }

}

public class WorldGenMessage : MessageBase {
    public int size;
    public int seed;
}
