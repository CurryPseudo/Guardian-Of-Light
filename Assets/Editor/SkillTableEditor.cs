using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SkillTable))]
public class SkillTableEditor : Editor {
    List<bool> skillFoldOuts;
    List<bool> levelFoldOuts;
    SkillTable data;
    PlayerData playerData;
    public void OnEnable()
    {
        data = (SkillTable)target;
        playerData = Singleton<Datas>.Instance.PlayerData;
        skillFoldOuts = new List<bool>();
        levelFoldOuts = new List<bool>();
        for(int i = 0; i < data.skillList.Count; i++)
        {
            skillFoldOuts.Add(false);
            levelFoldOuts.Add(true);
        }
    }
    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("技能升级列表");
        EditorGUI.indentLevel = 0;
        data.skillDirectory = EditorGUILayout.TextField("技能图标目录", data.skillDirectory);

        for (int i = 0; i < data.skillList.Count; i++)
        {
            SkillTable.Skill skill = data.skillList[i];
            EditorGUI.indentLevel = 0;
            skillFoldOuts[i] = EditorGUILayout.Foldout(skillFoldOuts[i], skill.name);
            if (skillFoldOuts[i])
            {
                EditorGUI.indentLevel = 1;
                EditorGUILayout.LabelField("编号 " + skill.index);
                skill.name = EditorGUILayout.TextField("名称", skill.name);
                skill.discription = EditorGUILayout.TextField("技能描述", skill.discription);
                skill.buttonFile = EditorGUILayout.TextField("按钮文件名", skill.buttonFile);
                skill.randWeight = EditorGUILayout.FloatField("随机权值", skill.randWeight);
                levelFoldOuts[i] = EditorGUILayout.Foldout(levelFoldOuts[i],"各等级数值");
                if (levelFoldOuts[i]) {
                    EditorGUI.indentLevel = 2;
                    if (skill.eachLevelValues.Count > 0)
                    {
                        for (int j = 0; j < skill.eachLevelValues.Count; j++)
                        {

                            skill.eachLevelValues[j] = EditorGUILayout.FloatField("等级 " + j, skill.eachLevelValues[j]);
                        }
                    }else
                    {
                        EditorGUILayout.LabelField("没有数值，请点击新等级");
                    }
                    if (GUILayout.Button("新等级"))
                    {
                        skill.eachLevelValues.Add(0);
                    }
                    if (GUILayout.Button("删除一个等级"))
                    {
                        skill.eachLevelValues.RemoveAt(skill.eachLevelValues.Count - 1);
                    }
                }
                EditorGUI.indentLevel = 1;
                if (GUILayout.Button("删除此技能"))
                {
                    data.skillList.Remove(skill);
                    playerData.skillLevels.RemoveAt(i);
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
            data.skillList.Add(new SkillTable.Skill(data.skillList.Count,"未命名", "未描述","未设置文件名", 0));
            playerData.skillLevels.Add(0);
            skillFoldOuts.Add(true);
            levelFoldOuts.Add(true);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
