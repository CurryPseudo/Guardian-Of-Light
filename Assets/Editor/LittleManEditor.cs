using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(LittleMan))]
public class LittleManEditor : Editor {
    LittleMan littleMan;
    private void OnEnable()
    {
        littleMan = (LittleMan)target;
    }
    public override void OnInspectorGUI()
    {
        //Monster
        EditorGUI.indentLevel = 0;
        littleMan.Health = EditorGUILayout.FloatField("血量", littleMan.OriginalHealth);
        EditorGUILayout.LabelField("速度");
        EditorGUI.indentLevel = 1;
        littleMan.wanderMaxSpeed = EditorGUILayout.FloatField("平常的最大速度", littleMan.wanderMaxSpeed);
        littleMan.escapeMonsterMaxSpeed = EditorGUILayout.FloatField("逃离怪物的最大速度", littleMan.escapeMonsterMaxSpeed);
        littleMan.haveBeenCatchedMaxSpeed = EditorGUILayout.FloatField("被抓住时的最大速度", littleMan.haveBeenCatchedMaxSpeed);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("逃离怪物");
        EditorGUI.indentLevel = 1;
        littleMan.escapeMonsterForcePower = EditorGUILayout.FloatField("力", littleMan.escapeMonsterForcePower);
        littleMan.escapeMonsterRadius = EditorGUILayout.FloatField("感应怪物半径", littleMan.escapeMonsterRadius);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("接近光圈");
        EditorGUI.indentLevel = 1;
        littleMan.closerLightForcePower = EditorGUILayout.FloatField("力", littleMan.closerLightForcePower);
        littleMan.closerLightDistance = EditorGUILayout.FloatField("内圈半径（不再向光源移动）", littleMan.closerLightDistance);
        littleMan.closerLightKeepTime = EditorGUILayout.FloatField("该行动的持续时间", littleMan.closerLightKeepTime);
        littleMan.closerLightCoolTime = EditorGUILayout.FloatField("该行动的冷却时间", littleMan.closerLightCoolTime);
        //Agent
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("随机移动");
        EditorGUI.indentLevel = 1;
        littleMan.wanderForcePower = EditorGUILayout.FloatField("力", littleMan.wanderForcePower);
        littleMan.wanderDistance = EditorGUILayout.FloatField("随机移动圆离质心的距离", littleMan.wanderDistance);
        littleMan.wanderRadius = EditorGUILayout.FloatField("随机移动圆的半径", littleMan.wanderRadius);
        littleMan.wanderRandomMoveDistance = EditorGUILayout.FloatField("随机移动在圆上的跨步", littleMan.wanderRandomMoveDistance);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("躲避障碍物");
        EditorGUI.indentLevel = 1;
        littleMan.escapeObstacleForcePower = EditorGUILayout.FloatField("力", littleMan.escapeObstacleForcePower);
        littleMan.escapeObstacleDistance = EditorGUILayout.FloatField("开始躲避的范围", littleMan.escapeObstacleDistance);

    }
    private void OnSceneGUI()
    {
        Handles.color = Color.black;
        Vector3 WanderCenter = littleMan.RotateVector(littleMan.wanderDistance * Vector3.right);
        Handles.DrawLine(littleMan.transform.position, littleMan.transform.position + WanderCenter);
        Handles.DrawWireDisc(littleMan.transform.position + WanderCenter, Vector3.forward, littleMan.wanderRadius);
        Handles.DrawLine(littleMan.transform.position, littleMan.transform.position + Agent.V2toV3(littleMan.wanderTarget2D));
    }
}
