using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using Container;
using log4net.Util;

// デバッグ用ウィンドウ
public class DebugWindow : EditorWindow
{
    private Vector3 from;
    private Vector3 to;
    private int turnNumber;

    [MenuItem("Window/Debug Window")]
    static void Open()
    {
        EditorWindow window = GetWindow<DebugWindow>();
        window.titleContent = new GUIContent("Debug Window");
    }

    private void OnGUI()
    {
        from = EditorGUILayout.Vector3Field("From", from);
        to = EditorGUILayout.Vector3Field("to", to);
        turnNumber = EditorGUILayout.IntField("turnNumber", turnNumber);

        Setting.language = (Language)EditorGUILayout.EnumPopup("Language", Setting.language);

        if (GUILayout.Button("Generate Mesh and Object"))
        {
            MeshGenerater meshGenerater = new MeshGenerater();
            meshGenerater.CreateHall(from, to, 1);

            GameObject go = new GameObject("test");
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            meshFilter.mesh = meshGenerater.GenerateMesh();

            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        }

        if (GUILayout.Button("Test"))
        {
            for (int i = 1; i <= turnNumber; i++)
            {
                Debug.Log((float)i / turnNumber + 1);
            }
        }
    }
}
