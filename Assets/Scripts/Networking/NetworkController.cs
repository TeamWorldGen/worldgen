using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Diagnostics;

[DisallowMultipleComponent]
public class NetworkController : MonoBehaviour {

    private static NetworkController instance;
    public static NetworkController Instance {
        get { return instance; }
    }

#if SERVER
    private Server server;
#endif

    void Awake() {
        instance = this;
    }

    void Start() {
        CustomNetworkManager manager = CustomNetworkManager.Instance;

        // Start as server when in batchmode
        if (getCommandArgument("batchmode") != null) {

#if SERVER

            // Start server

            string size = getCommandArgument("size");
            string seed = getCommandArgument("seed");
            manager.worldSize = (size != null) ? int.Parse(size) : 100;
            manager.worldSeed = (seed != null) ? int.Parse(seed) : 0;

            string address = getCommandArgument("address");
            string port = getCommandArgument("port");
            manager.networkAddress = (address != null) ? address : "localhost";
            manager.networkPort = (port != null) ? int.Parse(port) : 7777;

            // Start server console
            server = new Server();
            SetupCommands();
            server.Start();

            // Start networking server
            manager.StartServer();

            // Build the world
            BuildDynamicWorld();

#elif CLIENT
            // Quit the application if the client build is started in batchmode
            Application.Quit();
#endif

        } else { // If batchmode argument is not found

#if CLIENT
            LoadingScreen.Show();
            LoadingScreen.SetSubtitle("Connecting to server...");
            string address = GlobalStorage.connectAddress;
            int port = GlobalStorage.connectPort;

#if TESTING
            // Start host (only for quick testing in editor)
            StartHost(address, port);
#else
            // Start client
            ConnectToServer(address, port);
#endif


#elif SERVER
            // Quit the application if the server build is not started in batchmode
            Application.Quit();
#endif

        }

    }

#if SERVER
    void Update() {

        if (server != null)
            server.Refresh();

    }

    private void StartServer() {
    }

    private void SetupCommands() {

        server.AddCommand("stop", () => { // stop server
            Application.Quit();
        });

        server.AddCommand("list", () => { // player list
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            string output = playerObjects.Length + " players connected\n";
            foreach (GameObject playerObject in playerObjects) {
                Player player = playerObject.GetComponent<Player>();
                output += " - " + player.PlayerName + "\n";
            }
            UnityEngine.Debug.Log(output);
        });

    }

#endif

#if TESTING
    public void StartHost(string address, int port) {
        CustomNetworkManager manager = CustomNetworkManager.Instance;
        manager.networkAddress = address;
        manager.networkPort = port;
        manager.StartHost();
        manager.client.RegisterHandler(100, OnCreateWorld);

        //BuildDynamicWorld();
    }
#endif

#if SERVER || TESTING
    public void BuildDynamicWorld() {
        UnityEngine.Debug.Log("Building dynamic world...");
        PrefabSpawner.Instance.SpawnTrees(500);
    }
#endif

#if CLIENT
    public void ConnectToServer(string address, int port) {
        CustomNetworkManager manager = CustomNetworkManager.Instance;
        manager.networkAddress = address;
        manager.networkPort = port;
        manager.StartClient();
        manager.client.RegisterHandler(100, OnCreateWorld);
    }

    void OnCreateWorld(NetworkMessage netMessage) {

        // Read net message
        WorldGenMessage message = netMessage.ReadMessage<WorldGenMessage>();

        // TODO: Use seed and size from net message

        // Create chunks
        WorldManager.Instance.CreateTerrainChunks();
    }

    public void OnChunksReady() {
        LoadingScreen.Hide();
    }

#endif

    private string getCommandArgument(string arg) {
        string[] args = System.Environment.GetCommandLineArgs();
        string input = null;
        for (int i = 0; i < args.Length; i++) {
            if (args[i] == ("-" + arg))
                input = args[i + 1];
        }
        return input;
    }

    private void OnDestroy() {
        instance = null;
#if SERVER
        if (server != null)
            server.Stop();
#endif
    }

}
