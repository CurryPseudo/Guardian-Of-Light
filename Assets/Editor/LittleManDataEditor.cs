using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LittleManData))]
public class LittleManDataEditor : Editor {
    LittleManData data;
    private void OnEnable()
    {
        data = (LittleManData)target;
    }
    public override void OnInspectorGUI()
    {
        //CounterAttack
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("村民属性");
        EditorGUILayout.LabelField("反击");
        EditorGUI.indentLevel = 1;
        data.counterAttack.viewRadius = EditorGUILayout.FloatField("反击光圈半径", data.counterAttack.viewRadius);
        data.counterAttack.keepTime = EditorGUILayout.FloatField("反击光圈持续时间", data.counterAttack.keepTime);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
