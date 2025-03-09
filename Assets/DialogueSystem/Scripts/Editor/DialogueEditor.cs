using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DialogueSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        //* To popup a Dialogue Editor window in Unity Editor
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

        //* To open a Dialogue Editor window when a Dialogue asset is double clicked
        [OnOpenAsset(1)]
        public static bool OnOpenDialogueSoAsset(int instanceID, int line)
        {
            var dialogueSo = EditorUtility.InstanceIDToObject(instanceID) as So_Dialogue;
            if (dialogueSo != null)
            {
                ShowWindow();
                Debug.Log("Opening dialogue: " + dialogueSo.name);
                return true;
            }
            // if (Selection.activeObject.GetType() == typeof(So_Dialogue))
            // {
            //     So_Dialogue dialogue = Selection.activeObject as So_Dialogue;
            //     Debug.Log("Opening dialogue: " + dialogue.name);
            //     return true;
            // }
            return false;
        }
    }
}
