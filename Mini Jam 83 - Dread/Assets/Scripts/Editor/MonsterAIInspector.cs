using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterAI))]
public class MonsterAIInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MonsterAI monster = (MonsterAI)target;
    }
}
