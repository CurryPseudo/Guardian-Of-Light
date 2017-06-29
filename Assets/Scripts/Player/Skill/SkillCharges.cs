using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCharges {
    List<SkillCharge> skillList;
    int activeSkillCount;
   public int ActiveSkillCount
    {
        get
        {
            return activeSkillCount;
        }
    }
    public void update(float time)
    {
        refleshVerticalIndex();
        foreach (SkillCharge skillCharge in skillList)
        {
            skillCharge.chargeUpdate(time);
        }
    }
    public SkillCharges()
    {
        skillList = new List<SkillCharge>();
        SkillChargeData skillChargeData = Singleton<Datas>.Instance.SkillChargeData;
        foreach (SkillChargeData.Skill skill in skillChargeData.skillList)
        {
            SkillCharge skillCharge = null;
            if (skill.useType == SkillChargeData.Skill.UseType.normal)
            {
                skillCharge = new SkillChargeNormal(skill, skillList.Count);
            } else if (skill.useType == SkillChargeData.Skill.UseType.one_time)
            {
                skillCharge = new SkillChargeOne_Time(skill, skillList.Count);
            }else if (skill.useType == SkillChargeData.Skill.UseType.one_timeEachNight)
            {
                skillCharge = new SkillChargeOne_TimeEachNight(skill, skillList.Count);
            }
            addSkillCharge(skillCharge);
        }
        refleshVerticalIndex();
    }
    public void addSkillCharge(SkillCharge skillCharge)
    {
        skillList.Add(skillCharge);
    }
    public void removeSkillCharge(SkillCharge skillCharge)
    {
        skillList.Remove(skillCharge);
        refleshVerticalIndex();
    }
    void refleshVerticalIndex()
    {
        activeSkillCount = 0;
        int i = 0;
        foreach(SkillCharge skillCharge in skillList)
        {
            if (skillCharge.skill.Enabled)
            {
                skillCharge.VerticalIndex = i;
                activeSkillCount++;
                i++;
            }else
            {
                skillCharge.VerticalIndex = -1;
            }
        }

        return;
    }
    public SkillCharge findSkillCharge(string name)
    {
        foreach(SkillCharge skillCharge in skillList)
        {
            if(skillCharge.skill.name == name)
            {
                return skillCharge;
            }
        }
        return null;
    }
}
