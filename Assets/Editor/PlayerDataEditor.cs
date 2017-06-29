using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerData))]
public class PlayerDataEditor : Editor {
    bool physicalInfo = true;
    bool lightInfo = true;
    bool leapInfo = true;
    bool skillInfo = true;
    bool putLightInfo = true;
    PlayerData data;
    SkillTable skillTable;
    public void OnEnable()
    {
        data = (PlayerData)target;
        skillTable = Singleton<Datas>.Instance.SkillTable;
        if (data.skillLevels.Count == 0)
        {
            for(int i = 0; i < skillTable.skillList.Count; i++)
            {
                data.skillLevels.Add(0);
            }
        }
    }
    public override void OnInspectorGUI()
    {
        
        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("玩家初始数值");
        physicalInfo = EditorGUILayout.Foldout(physicalInfo, "物理属性");
        if (physicalInfo)
        {
            EditorGUI.indentLevel = 1;
            data.physic.moveSpeed = EditorGUILayout.FloatField("基本移动速度", data.physic.moveSpeed);
        }
        EditorGUI.indentLevel = 0;
        lightInfo = EditorGUILayout.Foldout(lightInfo, "光圈属性");
        if (lightInfo)
        {
            EditorGUI.indentLevel = 1;
            data.light.radius = EditorGUILayout.FloatField("光圈半径", data.light.radius);
            data.light.expandDegree = EditorGUILayout.FloatField("光圈变化最大系数(>=1)", data.light.expandDegree);
            data.light.expandCurve = EditorGUILayout.CurveField("光圈变化曲线)", data.light.expandCurve);
            data.light.expandTime = EditorGUILayout.FloatField("光圈变化单位时间)", data.light.expandTime);
        }
        EditorGUI.indentLevel = 0;
        leapInfo = EditorGUILayout.Foldout(leapInfo, "飞跃属性");
        if (leapInfo)
        {
            EditorGUI.indentLevel = 1;
            data.leap.viewRadius = EditorGUILayout.FloatField("飞跃光圈半径", data.leap.viewRadius);
            data.leap.moveSpeed = EditorGUILayout.FloatField("飞跃移动速度", data.leap.moveSpeed);
            data.leap.keepTime = EditorGUILayout.FloatField("飞跃持续时间", data.leap.keepTime);
            data.leap.animateDelayTime = EditorGUILayout.FloatField("飞跃动画延迟时间", data.leap.animateDelayTime);
        }
        EditorGUI.indentLevel = 0;
        putLightInfo = EditorGUILayout.Foldout(putLightInfo, "光球属性");
        if (putLightInfo)
        {
            EditorGUI.indentLevel = 1;
            data.putLight.viewRadius = EditorGUILayout.FloatField("光球光圈半径", data.putLight.viewRadius);
            data.putLight.keepTime = EditorGUILayout.FloatField("光球持续时间", data.putLight.keepTime);
            data.putLight.startShrinkTime = EditorGUILayout.FloatField("光球开始缩小时间", data.putLight.startShrinkTime);
        }
        
        EditorGUI.indentLevel = 0;
        skillInfo = EditorGUILayout.Foldout(skillInfo, "技能属性");
        if (skillInfo)
        {
            EditorGUI.indentLevel = 0;
            data.activeSkillMaxCount = EditorGUILayout.IntField("主动技能数上限", data.activeSkillMaxCount);
            EditorGUILayout.LabelField("召唤圣光属性");
            EditorGUI.indentLevel = 1;
            data.summonTheLight.viewRadius = EditorGUILayout.FloatField("圣光半径", data.summonTheLight.viewRadius);
            data.summonTheLight.keepTime = EditorGUILayout.FloatField("圣光持续时间", data.summonTheLight.keepTime);
            data.summonTheLight.startShrinkTime = EditorGUILayout.FloatField("圣光开始变小时间", data.summonTheLight.startShrinkTime);
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("怪物感应属性");
            EditorGUI.indentLevel = 1;
            data.monsterInduction.keepTime = EditorGUILayout.FloatField("技能持续时间", data.monsterInduction.keepTime);

            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("初始技能");
            EditorGUI.indentLevel = 1;
            for (int i= 0;i < skillTable.skillList.Count;i++)
            {
                data.skillLevels[i] = EditorGUILayout.IntField(skillTable.skillList[i].name + "（最高等级" + (skillTable.skillList[i].eachLevelValues.Count-1).ToString() + "）", data.skillLevels[i]);
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
