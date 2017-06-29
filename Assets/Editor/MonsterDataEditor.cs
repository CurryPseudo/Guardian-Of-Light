using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MonsterData))]
public class MonsterDataEditor : Editor {
    List<bool> monsterInfoFoldOuts;
    List<bool> mutationInfoFoldOuts;
    MonsterData data;
    public void OnEnable()
    {
        data = (MonsterData)target;
        monsterInfoFoldOuts = new List<bool>();
        mutationInfoFoldOuts = new List<bool>();
        for (int i = 0; i < data.monsterInfoEachNight.Count; i++)
        {
            monsterInfoFoldOuts.Add(false);
        }
        for (int i = 0; i < data.mutationInfos.Count; i++)
        {
            mutationInfoFoldOuts.Add(false);
        }
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("每个晚上的怪物共同属性");
        data.speedChangeCurve = EditorGUILayout.CurveField("速度变化曲线", data.speedChangeCurve);
        data.escapeSpeedChangeCurve = EditorGUILayout.CurveField("避光速度变化曲线", data.escapeSpeedChangeCurve);
        data.attackChangeCurve = EditorGUILayout.CurveField("攻击变化曲线", data.attackChangeCurve);
        for (int i = 0; i < data.monsterInfoEachNight.Count; i++)
        {
            MonsterInfo monsterInfo = data.monsterInfoEachNight[i];
            EditorGUI.indentLevel = 0;
            monsterInfoFoldOuts[i] = EditorGUILayout.Foldout(monsterInfoFoldOuts[i], "第"+(i+1)+"夜");
            if (monsterInfoFoldOuts[i])
            {
                EditorGUI.indentLevel = 1;
                monsterInfo.originalSpeed = EditorGUILayout.FloatField("初始速度", monsterInfo.originalSpeed);
                monsterInfo.originalEscapeSpeed = EditorGUILayout.FloatField("初始避光速度", monsterInfo.originalEscapeSpeed);
                monsterInfo.originalAttack = EditorGUILayout.FloatField("初始攻击", monsterInfo.originalAttack);
                monsterInfo.finalSpeed = EditorGUILayout.FloatField("最终速度", monsterInfo.finalSpeed);
                monsterInfo.finalEscapeSpeed = EditorGUILayout.FloatField("最终避光速度", monsterInfo.finalEscapeSpeed);
                monsterInfo.finalAttack = EditorGUILayout.FloatField("最终攻击", monsterInfo.finalAttack);
                monsterInfo.mutationProbability = EditorGUILayout.FloatField("突变几率", monsterInfo.mutationProbability);
                monsterInfo.monsterNum = EditorGUILayout.IntField("增加怪物数", monsterInfo.monsterNum);
                data.monsterInfoEachNight[i] = monsterInfo;
                if (GUILayout.Button("删除此夜晚（往后的夜晚会顺延）"))
                {
                    data.monsterInfoEachNight.Remove(monsterInfo);
                    return;
                }
            }
        }
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("新夜晚"))
        {
            for (int i = 0; i < data.monsterInfoEachNight.Count; i++)
            {
                monsterInfoFoldOuts[i] = false;
            }
            data.addMonsterInfo();
            monsterInfoFoldOuts.Add(true);
        }
        EditorGUILayout.LabelField("突变一览");
        for (int i = 0; i < data.mutationInfos.Count; i++)
        {
            MutationInfo info = data.mutationInfos[i];
            EditorGUI.indentLevel = 0;
            mutationInfoFoldOuts[i] = EditorGUILayout.Foldout(mutationInfoFoldOuts[i], info.name);
            if (mutationInfoFoldOuts[i])
            {
                EditorGUI.indentLevel = 1;
                data.changeMutationName(info, EditorGUILayout.TextField("名称", info.name));
                info.instruction = EditorGUILayout.TextField("说明", info.instruction);
                info.randWeight = EditorGUILayout.FloatField("随机权值", info.randWeight);
                EditorGUILayout.LabelField("各等级数值");
                info.ValuesCount = EditorGUILayout.IntField("等级数", info.valueEachLevel.Count);
                EditorGUI.indentLevel = 2;
                for (int j = 0; j < info.valueEachLevel.Count; j++)
                {
                    info.valueEachLevel[j] = EditorGUILayout.FloatField("等级" + j + "的值为", info.valueEachLevel[j]);
                }
                if (GUILayout.Button("删除此突变"))
                {
                    data.removeMutationInfo(info);
                    return;
                }
            }

        }
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("新突变"))
        {
            for (int i = 0; i < data.mutationInfos.Count; i++)
            {
                mutationInfoFoldOuts[i] = false;
            }
            data.addMutationInfo();
            mutationInfoFoldOuts.Add(true);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

}
