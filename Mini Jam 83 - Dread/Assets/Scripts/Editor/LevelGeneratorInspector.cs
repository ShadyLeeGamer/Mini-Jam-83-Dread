using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))] // LINK THIS WITH LEVEL GENERATOR TYPE
public class LevelGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // DRAW DEFAULT VALUES AND PROPERTIES

        LevelGenerator levelGenerator = (LevelGenerator)target; // REFERENCING LEVEL GENERATOR
        if(GUILayout.Button("Generate"))
            levelGenerator.Generate();
    }
}