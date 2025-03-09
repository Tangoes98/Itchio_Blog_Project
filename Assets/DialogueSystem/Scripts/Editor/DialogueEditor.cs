using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        [MenuItem("Window/Dialogue Editor")]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditor>("Dialogue Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dialogue Editor", EditorStyles.boldLabel);

            if (GUILayout.Button("Create New Dialogue"))
            {
                Debug.Log("Creating new dialogue");
            }
        }
    }
}
