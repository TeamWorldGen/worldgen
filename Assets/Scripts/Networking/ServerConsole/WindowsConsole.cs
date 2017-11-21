using System;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

public class WindowsConsole : IConsole {

    TextWriter oldOutput;

    public void Init() {

        // Attach the console to the parent process or create a new one if it fails
        if (!AttachConsole(0x0ffffffff)) {
            AllocConsole();
        }

        // Store the output before we set a new one.
        // We need to set it back to this when the console shuts down to avoid some errors
        oldOutput = Console.Out;

        try {
            IntPtr stdHandle = GetStdHandle(-11);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fStream = new FileStream(safeFileHandle, FileAccess.Write);
            StreamWriter stdOut = new StreamWriter(fStream);
            stdOut.AutoFlush = true;
            Console.SetOut(stdOut);
        } catch(Exception e) {
            Debug.LogError(e.Message);
        }

    }

    // Set the title of the console window
    public void SetTitle(String title) {
        SetConsoleTitle(title);
    }

    // Shutdown the console
    public void Shutdown() {
        Console.SetOut(oldOutput);
        FreeConsole();
    }

    // *************************************
    // Functions for the windows console API
    // *************************************
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleTitle(string lpConsoleTitle);

}
