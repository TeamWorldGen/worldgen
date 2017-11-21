using UnityEngine;
using UnityEditor;

public class MenuItems : MonoBehaviour {

    private static string buildPath = "D:/Libraries/Development/Unity Projects/Unity builds/WorldGen";

    [MenuItem("Build/Build Client and Server &b", false, 0)]
    private static void BuildClientAndServer() {
        BuildClient();
        BuildServer();
    }

    [MenuItem("Build/Build Client &c", false, 1)]
    private static void BuildClient() {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

        SetClient();
        Debug.Log("Building client...");

        string path = buildPath + "/client/WorldGen.exe";
        string[] levels = { "Assets/_Scenes/main.unity", "Assets/_Scenes/game.unity" };

        BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows, BuildOptions.None);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
    }

    [MenuItem("Build/Build Server &s", false, 2)]
    private static void BuildServer() {
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);

        SetServer();
        Debug.Log("Building server...");

        string path = buildPath + "/server/WorldGenServer.exe";
        string[] levels = { "Assets/_Scenes/game.unity" };

        BuildPipeline.BuildPlayer(levels, path, BuildTarget.StandaloneWindows, BuildOptions.None);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
    }

    [MenuItem("Set Defines/CLIENT + SERVER", false, 20)]
    private static void SetClientAndServer() {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CLIENT;SERVER");
    }

    [MenuItem("Set Defines/CLIENT", false, 21)]
    private static void SetClient() {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CLIENT");
    }

    [MenuItem("Set Defines/SERVER", false, 22)]
    private static void SetServer() {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "SERVER");
    }

    [MenuItem("Set Defines/TESTING", false, 23)]
    private static void Testing() {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "CLIENT;SERVER;TESTING");
    }

}
