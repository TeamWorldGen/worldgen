using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldManager))]
public class WorldGeneratorEditor : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        WorldManager worldGenerator = (WorldManager)target;
        
        if (GUILayout.Button("Generate")) {
            worldGenerator.OnEditorGenerate();
        }

        if (GUILayout.Button("Reset")) {
            worldGenerator.OnEditorReset();
        }

    }

}