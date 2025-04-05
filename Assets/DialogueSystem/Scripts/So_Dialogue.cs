using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
    public class So_Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> _nodes = new();
        Dictionary<string, DialogueNode> _nodeLookup = new();

        //* Preprocessing directive to only execute the code if the script is being compiled in the Unity Editor
        void Awake()
        {
#if UNITY_EDITOR
            if (_nodes.Count == 0)
            {
                DialogueNode node = new();
                node.ID = System.Guid.NewGuid().ToString();
                _nodes.Add(node);

                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif

            OnValidate();
        }

        void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (var node in _nodes)
            {
                _nodeLookup[node.ID] = node;
            }
        }

        public IEnumerable<DialogueNode> GetAllNodes()
            => _nodes;
        public DialogueNode GetRootNode()
            => _nodes[0];
        public IEnumerable<DialogueNode> GetAllChildNodes(DialogueNode parentNode)
        {
            if (parentNode.Children == null)
                yield break;

            foreach (var childID in parentNode.Children)
            {
                if (_nodeLookup.ContainsKey(childID))
                    yield return _nodeLookup[childID];
            }
        }

    }
}
