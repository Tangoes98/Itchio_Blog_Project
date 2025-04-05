using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(So_InspectorUI))]
public class So_InspectorUI_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        So_InspectorUI so = (So_InspectorUI)target;

        var btn_CheckOut = GUILayout.Button("Check Out");
        var btn_ClearOutput = GUILayout.Button("Clear Result");

        OnButtonClick(btn_CheckOut, so.CheckOut);
        OnButtonClick(btn_ClearOutput, so.ClearOutput);
    }

    void OnButtonClick(bool btn, Action action)
    {
        if (btn)
            action.Invoke();
    }
}
