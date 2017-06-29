using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "InitInfo/Create SkillChargeData")]
public class SkillChargeData : ScriptableObject{
    [System.Serializable]
    public struct Skill
    {
        public int index;
        public string name;
        public float chargeTime;
        public int initCount;
        public int maxCount;
        [System.Serializable]
        public enum UseType
        {
            normal = 0,
            one_time = 1,
            one_timeEachNight = 2,
        }
        public UseType useType;
        public bool initEnabled;
        public bool skillTableRelated;
        public string skillTableName;
        public Color chargeColor;
        public string buttonName;
        public string buttonFile;
        public bool Enabled
        {
            get
            {
                if (initEnabled) return true;
                return Singleton<Datas>.Instance.SkillTable.findValue(skillTableName) != 0;
            }
        }
        public int MaxCount
        {
            get
            {
                if (!skillTableRelated) return maxCount;
                return maxCount + (int)Singleton<Datas>.Instance.SkillTable.findValue(skillTableName);
            }
        }

        public string buttonFileAndFile
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

        public Skill(int _index)
        {
            index = _index;
            name = "未命名";
            chargeTime = 0;
            useType = UseType.normal;
            initCount = 0;
            skillTableRelated = false;
            maxCount = 0;
            buttonName = "";
            buttonFile = "";
            initEnabled = true;
            skillTableName = "";
            chargeColor = Color.white;
        }
    }
    public List<Skill> skillList = new List<Skill>();
    public void refleshIndex()
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            Skill skill = (Skill)skillList[i];
            skill.index = i;
            skillList[i] = skill;
        }
    }
    public Skill? findSkill(string skillName)
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
}
