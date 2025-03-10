using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DialogueSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        So_Dialogue _selectedDialogue;


        #region Window

        //* To popup a Dialogue Editor window in Unity Editor
        [MenuItem("Window/Dialogue Editor/Dialogue Editor", false, 100)]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditor>("Dialogue Editor");
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
            return false;
        }

        #endregion

        #region GUI
        void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        void OnSelectionChanged()
        {
            var currentDialogue = Selection.activeObject as So_Dialogue;
            if (currentDialogue != null)
            {
                _selectedDialogue = currentDialogue;
                Repaint();
            }
        }

        void OnGUI()
        {
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected");
                return;
            }
            else
            {
                foreach (var item in _selectedDialogue.GetNodes())
                {
                    EditorGUI.BeginChangeCheck();
                    //EditorGUILayout.LabelField(item.Text, EditorStyles.boldLabel);
                    string newText = EditorGUILayout.TextField(item.Text);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_selectedDialogue, "Dialogue Text Change");
                        item.Text = newText;
                        //EditorUtility.SetDirty(_selectedDialogue);
                    }



                }
            }

        }

        #endregion
    }
}
