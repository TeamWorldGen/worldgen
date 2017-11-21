using System;
using UnityEngine;

public class ServerConsole {

    private IConsole console;
    private string input;

    public delegate void CommandAction(string command);
    public event CommandAction OnCommand;

    public ServerConsole() {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        console = new WindowsConsole();
#endif
    }

    public void Start() {
        console.Init();
        Application.logMessageReceived += HandleLog;
    }

    public void SetTitle(string title) {
        console.SetTitle(title);
    }

    public void Refresh() {
        if (!Console.KeyAvailable) // Return if no key is avilable
            return;

        ConsoleKeyInfo keyInfo = Console.ReadKey();

        if (keyInfo.Key == ConsoleKey.Backspace && input != null && input.Length > 0) { // backspace
            input = input.Substring(0, input.Length - 1);
            ClearInputLine();
            Console.Write(input);
        }
        else if (keyInfo.Key == ConsoleKey.Enter) { // enter
            if (input == null)
                return;

            ClearInputLine();
            Console.WriteLine("> " + input);
            OnCommand(input);
            input = "";
        }
        else if (keyInfo.Key == ConsoleKey.Escape) { // ignore escape for now
            // TODO: Should escape key do something in the console?
        }
        else if (keyInfo.Key == ConsoleKey.Tab) { // ignore tab
        }
        else if (keyInfo.KeyChar == '\u0000') { // ignore if KeyChar value is \u0000 (does not map to a unicode char)
        }
        else { // write key to input
            input += keyInfo.KeyChar;
            ClearInputLine();
            Console.Write(input);
        }
    }

    // Clear the current line of the console
    private void ClearInputLine() {
        if (input == null || input.Length == 0)
            return;

        Console.CursorLeft = 0;
        Console.Write(new String(' ', Console.WindowWidth));
        Console.CursorTop--;
        Console.CursorLeft = 0;
    }

    private void HandleLog(string logString, string stackTrace, LogType type) {

        if (type == LogType.Error)
            Console.ForegroundColor = ConsoleColor.Red;
        else if (type == LogType.Warning)
            Console.ForegroundColor = ConsoleColor.Yellow;
        else
            Console.ForegroundColor = ConsoleColor.White;

        ClearInputLine();
        Console.WriteLine(logString);
        if (!string.IsNullOrEmpty(stackTrace))
            Console.WriteLine(stackTrace);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void Shutdown() {
        Application.logMessageReceived -= HandleLog;
        console.Shutdown();
    }

}
