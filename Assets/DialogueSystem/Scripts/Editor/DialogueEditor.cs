using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DialogueSystem.Editor
{
    public class DialogueEditor : EditorWindow
    {
        So_Dialogue _selectedDialogue;
        GUIStyle _nodeStyle;
        DialogueNode _draggedNode;
        Vector2 _draggedNodeOffset;


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

            _nodeStyle = new();
            _nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _nodeStyle.normal.textColor = Color.white;
            _nodeStyle.padding = new(10, 10, 10, 10);
            _nodeStyle.border = new(12, 12, 12, 12);
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
                ProcessEvents(Event.current);
                foreach (var item in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(item);
                    DrawConnections(item);
                }
            }

        }

        void ProcessEvents(Event curEvt)
        {
            if (curEvt.type == EventType.MouseDown && _draggedNode == null)
            {
                //* Start to drag
                _draggedNode = GetNodeAtPoint(curEvt.mousePosition);
                if (_draggedNode != null)
                    _draggedNodeOffset = curEvt.mousePosition - _draggedNode.Rect.position;
            }
            else if (curEvt.type == EventType.MouseDrag && _draggedNode != null)
            {
                //* Dragging
                Undo.RecordObject(_selectedDialogue, "Dialogue Node Drag");
                _draggedNode.Rect.position = curEvt.mousePosition - _draggedNodeOffset;
                GUI.changed = true;
            }
            else if (curEvt.type == EventType.MouseUp && _draggedNode != null)
            {
                //* Stop dragging
                _draggedNode = null;
            }
        }

        void DrawNode(DialogueNode node)
        {
            GUILayout.BeginArea(node.Rect, _nodeStyle);
            EditorGUI.BeginChangeCheck();


            EditorGUILayout.LabelField("NodeID: " + node.ID, EditorStyles.boldLabel);
            string newText = EditorGUILayout.TextField(node.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Dialogue Text Change");
                node.Text = newText;
                //EditorUtility.SetDirty(_selectedDialogue);
            }

            if (GUILayout.Button("Add Child"))
            {
                Undo.RecordObject(_selectedDialogue, "Add Dialogue Node");
                // node.Children.Add(Guid.NewGuid().ToString());
                // EditorUtility.SetDirty(_selectedDialogue);
            }


            EditorGUILayout.LabelField("Child:", EditorStyles.boldLabel);
            foreach (var item in _selectedDialogue.GetAllChildNodes(node))
            {
                EditorGUILayout.LabelField(item.Text);
            }

            GUILayout.EndArea();
        }


        private DialogueNode GetNodeAtPoint(Vector2 mousePosition)
        {
            DialogueNode expectNode = null;
            foreach (var item in _selectedDialogue.GetAllNodes())
            {
                if (!item.Rect.Contains(mousePosition))
                    continue;
                else
                    expectNode = item;
            }

            return expectNode;
        }
        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPos = new(node.Rect.xMax, node.Rect.center.y);
            foreach (var childNode in _selectedDialogue.GetAllChildNodes(node))
            {
                //Vector3 startPos = node.Rect.center;
                Vector3 endPos = new(childNode.Rect.xMin, childNode.Rect.center.y);
                Vector3 offset = endPos - startPos;
                offset.y = 0;
                offset.x *= 0.5f;

                Handles.DrawBezier(
                    startPos, endPos,
                    startPos + offset, endPos - offset,
                    Color.white, null, 4f);
            }
        }


        #endregion
    }
}
