#if SERVER
using System.Collections.Generic;
using UnityEngine;

public class Server {

    private ServerConsole console;

    public delegate void Command();
    private Dictionary<string, Command> commands = new Dictionary<string, Command>();

    public Server() {
        console = new ServerConsole();
        console.OnCommand += ExecuteCommand;
        console.SetTitle("Server");
    }

    public void Start() {
        console.Start();
        Debug.Log("Server started");
    }

    public void Stop() {
        Debug.Log("Shutting down...");
        console.Shutdown();
    }

    public void Refresh() {
        console.Refresh();
    }

    public void ExecuteCommand(string command) {
        Command action;
        command = command.Trim().ToLower();
        if (!commands.TryGetValue(command, out action)) {
            Debug.Log("Unknown command");
            return;
        }
        action();
    }

    public void AddCommand(string command, Command action) {
        command = command.Trim().ToLower();
        commands.Add(command, action);
    }

}
#endif