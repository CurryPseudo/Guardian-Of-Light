using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="InitInfo/Create SkillTable")]
public class SkillTable : ScriptableObject {
    public string skillDirectory = "";
    [System.Serializable]
    public class Skill{
        public int index;
        public string name;
        public string discription;
        public string buttonFile;
        public float randWeight;
        public List<float> eachLevelValues;

        public string buttonDirectoryAndFile
        {
            get
            {
                return Singleton<Datas>.Instance.SkillTable.skillDirectory+"/"+ buttonFile;
            }

            set
            {
                buttonFile = value;
            }
        }

        public Skill(int index,string name,string discription, string buttonFile, float randWeight)
        {
            this.index = index;
            this.name = name;
            this.discription = discription;
            this.buttonDirectoryAndFile = buttonFile;
            this.randWeight = randWeight;
            eachLevelValues = new List<float>();
        }
    }
    public void refleshIndex()
    {
        for(int i = 0; i < skillList.Count; i++)
        {
            Skill skill = (Skill)skillList[i];
            skill.index = i;
            skillList[i] = skill;
        }
    }
    public Skill findSkill(string skillName)
    {
        foreach (Skill skill in skillList)
        {
            if (skill.name == skillName)
            {
                return skill;
            }
        }
        return null;
    }
    public float findValue(string skillName)
    {
        Skill skill = findSkill(skillName);
        if (skill!=null)
        {
            int level = Singleton<Datas>.Instance.UpgradeList.originalList[skill.index].Level;
            if(level>= skill.eachLevelValues.Count)
            {
                Debug.LogErrorFormat("skill[" + skill.name + "] getting level[" + level + "] wrong");
            }
            return skill.eachLevelValues[level];
        }
        return -1;
    }
    public List<Skill> skillList = new List<Skill>();
}
