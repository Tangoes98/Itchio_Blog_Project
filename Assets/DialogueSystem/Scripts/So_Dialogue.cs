using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
    public class So_Dialogue : ScriptableObject
    {
        [SerializeField] DialogueNode[] _nodes;


    }
}
