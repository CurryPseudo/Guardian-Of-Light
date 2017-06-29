using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SkillChargeData))]
public class SkillChargeDataEditor : Editor {
    SkillChargeData data;
    List<bool> skillFoldOuts = new List<bool>();
    public void OnEnable()
    {
        data = (SkillChargeData)target;
        for (int i = 0; i < data.skillList.Count; i++)
        {
            skillFoldOuts.Add(false);
        }
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("主动技能列表");
        for (int i = 0; i < data.skillList.Count; i++)
        {
            SkillChargeData.Skill skill = data.skillList[i];
            EditorGUI.indentLevel = 0;
            skillFoldOuts[i] = EditorGUILayout.Foldout(skillFoldOuts[i], skill.name);
            if (skillFoldOuts[i])
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("编号 " + skill.index);
                skill.name = EditorGUILayout.TextField("名称", skill.name);
                skill.useType = (SkillChargeData.Skill.UseType) EditorGUILayout.EnumPopup("使用模式", skill.useType);
                if (skill.useType == SkillChargeData.Skill.UseType.normal)
                {
                    skill.chargeTime = EditorGUILayout.FloatField("充能时间", skill.chargeTime);
                    skill.initCount = EditorGUILayout.IntField("初始充能数", skill.initCount);
                    skill.maxCount = EditorGUILayout.IntField("初始充能上限数", skill.maxCount);
                }
                skill.initEnabled = EditorGUILayout.Toggle("初始可用", skill.initEnabled);
                if (!skill.initEnabled)
                {
                    skill.skillTableRelated = EditorGUILayout.Toggle("是否关联技能升级", skill.skillTableRelated);
                    if (skill.skillTableRelated)
                    {
                        EditorGUI.indentLevel = 2;
                        skill.skillTableName = EditorGUILayout.TextField("关联技能升级名", skill.skillTableName);
                        EditorGUI.indentLevel = 1;

                    }
                }
                skill.chargeColor = EditorGUILayout.ColorField("技能充能环颜色", skill.chargeColor);
                skill.buttonName = EditorGUILayout.TextField("使用按钮", skill.buttonName);
                skill.buttonFile = EditorGUILayout.TextField("按钮图标路径", skill.buttonFile);
                EditorGUI.indentLevel = 1;
                if (GUILayout.Button("删除此技能"))
                {
                    data.skillList.Remove(skill);
                    data.refleshIndex();
                    return;
                }
                data.skillList[i] = skill;
            }
        }
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("新技能"))
        {
            for (int i = 0; i < data.skillList.Count; i++)
            {
                skillFoldOuts[i] = false;
            }
            data.skillList.Add(new SkillChargeData.Skill(data.skillList.Count));
            skillFoldOuts.Add(true);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
