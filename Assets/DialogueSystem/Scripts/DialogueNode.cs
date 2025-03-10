using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DialogueSystem
{
    [Serializable]
    public class DialogueNode
    {
        public string ID;
        public string Text;
        public string[] Children;
        public Rect NodePosition;
    }
}
