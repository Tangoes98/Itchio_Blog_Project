using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class So_Dialogue : ScriptableObject
    {
        [SerializeField] List<DialogueNode> _nodes = new();

        //* Preprocessing directive to only execute the code if the script is being compiled in the Unity Editor
#if UNITY_EDITOR
        void Awake()
        {
            if (_nodes.Count < 1)
                _nodes.Add(new DialogueNode());
        }
#endif

        public IEnumerable<DialogueNode> GetNodes()
            => _nodes;

    }
}
