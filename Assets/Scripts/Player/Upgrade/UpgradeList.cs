using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeList {
    Upgrade[] upgradeList;
    public delegate bool CheckEnable();
    Dictionary<int, CheckEnable> checkEnableRule = new Dictionary<int, CheckEnable>();
    public float findValue(string skillName)
    {
        SkillTable.Skill skill = Singleton<SkillTable>.Instance.findSkill(skillName);
        if (skill != null)
        {
            int level = originalList[skill.index].Level;
            if (level >= skill.eachLevelValues.Count)
            {
                Debug.LogErrorFormat("skill[" + skill.name + "] getting level[" + level + "] wrong");
            }
            return skill.eachLevelValues[level];
        }
        return -1;
    }
    public UpgradeList(int length)
    {
        upgradeList = new Upgrade[length];
        for(int i = 0; i < length; i++)
        {
            upgradeList[i] = new Upgrade(i,this);
            
        }
        for(int i = 0; i < length; i++)
        {
            string name = Singleton<Datas>.Instance.SkillTable.skillList[i].name;
            if (isActiveSkill(name))
            {
                checkEnableRule[i] = () =>
                {
                    return GameObject.Find("Player").GetComponent<SkillCharges>().ActiveSkillCount < Singleton<Datas>.Instance.PlayerData.activeSkillMaxCount;
                };
            }
        }
    }
    public CheckEnable getCheckEnable(int i)
    {
        CheckEnable result;
        if(checkEnableRule.TryGetValue(i,out result))
        {
            return result;
        }
        return null;
    }
    public Upgrade getActiveSkillUpdate(SkillChargeData.Skill skill)
    {
        if (!skill.skillTableRelated) return null;
        return upgradeList[Singleton<Datas>.Instance.SkillTable.findSkill(skill.skillTableName).index];
    }
    public bool isActiveSkill(string name)
    {
        return Singleton<Datas>.Instance.SkillChargeData.findSkill(name) != null;
    }
   public Upgrade[] originalList
    {
        get
        {
            return upgradeList;
        }
    }

    public List<Upgrade> getRandUpgrades(int num)
    {
        List<Upgrade> candidateList = new List<Upgrade>();
        List<Upgrade> resultList = new List<Upgrade>();
        foreach (Upgrade upgrade in upgradeList)
        {
            if (upgrade.IsEnable())
            {
                candidateList.Add(upgrade);
            }
        }
        for(int i = 0; i < num && candidateList.Count >0; i++)
        {
            Upgrade one = straightGetRandUpgrade(candidateList);
            candidateList.Remove(one);
            resultList.Add(one);
        }
        return resultList;

    }
    Upgrade straightGetRandUpgrade(List<Upgrade> list)
    {
        if (list.Count == 0) return null;
        int count = list.Count;
        float[] randWeightSum = new float[count];
        randWeightSum[0] =0;
        for (int i = 1; i < count; i++)
        {
            randWeightSum[i] = randWeightSum[i - 1] + list[i-1].Skill.randWeight;
        }
        float randValue = Random.Range(0, randWeightSum[count - 1]);
        for(int i = 1; i < count; i++)
        {
            if (randValue <= randWeightSum[i]) return list[i - 1];
        }
        return list[count-1];
    }
	
}
