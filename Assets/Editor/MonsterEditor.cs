using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Monster))]
[CanEditMultipleObjects]
public class MonsterEditor : Editor
{
    Monster monster;
    private void OnEnable()
    {
        monster = (Monster)target;
    }
    public override void OnInspectorGUI()
    {
        //Monster
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("速度");
        EditorGUI.indentLevel = 1;
        monster.wanderMaxSpeed = EditorGUILayout.FloatField("平常的最大速度", monster.wanderMaxSpeed);
        monster.escapeMaxSpeed = EditorGUILayout.FloatField("逃跑时候的最大速度", monster.escapeMaxSpeed);
        monster.catchMaxSpeed = EditorGUILayout.FloatField("抓平民时候的最大速度", monster.catchMaxSpeed);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("抓村民");
        EditorGUI.indentLevel = 1;
        monster.catchLittleManPower = EditorGUILayout.FloatField("力", monster.catchLittleManPower);
        monster.catchLittleManRadius = EditorGUILayout.FloatField("感应半径", monster.catchLittleManRadius);
        monster.catchedDistance = EditorGUILayout.FloatField("已抓住距离", monster.catchedDistance);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("逃离光圈");
        EditorGUI.indentLevel = 1;
        monster.escapeLightPower = EditorGUILayout.FloatField("力", monster.escapeLightPower);
        //Agent
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("随机移动");
        EditorGUI.indentLevel = 1;
        monster.wanderForcePower = EditorGUILayout.FloatField("力", monster.wanderForcePower);
        monster.wanderDistance = EditorGUILayout.FloatField("随机移动圆离质心的距离", monster.wanderDistance);
        monster.wanderRadius = EditorGUILayout.FloatField("随机移动圆的半径", monster.wanderRadius);
        monster.wanderRandomMoveDistance = EditorGUILayout.FloatField("随机移动在圆上的跨步", monster.wanderRandomMoveDistance);
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("躲避障碍物");
        EditorGUI.indentLevel = 1;
        monster.escapeObstacleForcePower = EditorGUILayout.FloatField("力", monster.escapeObstacleForcePower);
        monster.escapeObstacleDistance = EditorGUILayout.FloatField("开始躲避的范围", monster.escapeObstacleDistance);

        //Debug
        if (monster.MutationStatus != null)
        {
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField(monster.MutationStatus.ToString());
        }
    }
    private void OnSceneGUI()
    {
        Handles.color = Color.black;
        Vector3 WanderCenter = monster.RotateVector(monster.wanderDistance * Vector3.right);
        Handles.DrawLine(monster.transform.position, monster.transform.position + WanderCenter);
        Handles.DrawWireDisc(monster.transform.position + WanderCenter, Vector3.forward, monster.wanderRadius);
        Handles.DrawLine(monster.transform.position, monster.transform.position + Agent.V2toV3(monster.wanderTarget2D));
    }
}