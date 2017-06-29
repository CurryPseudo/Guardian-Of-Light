using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class SkillCharge
{
    public SkillChargeData.Skill skill;
    protected SkillChargeAnnulusController annuluses;
    protected int verticalIndex = -1;
    protected GameObject skillUIPrefab;
    public Action skillEvent;
    protected Vector3 originalScale;
    public virtual int VerticalIndex
    {
        get
        {
            return verticalIndex;
        }
        set
        {
            if (verticalIndex == value) return;
            verticalIndex = value;
            GameObject ui = annuluses.gameObject;

            if (ui != null)
            {
                if (verticalIndex != -1)
                {
                    ui.transform.localPosition = skillUIPrefab.transform.localPosition + Vector3.right * 100 * verticalIndex;
                    ui.transform.localScale = skillUIPrefab.transform.localScale;
                }
                else
                {
                    ui.transform.localScale = Vector3.zero;
                }
            }
        }
    }
    protected virtual Upgrade RelatedUpdate
    {
       get{return Singleton<Datas>.Instance.UpgradeList.getActiveSkillUpdate(skill); }
    }
    protected virtual void commonStructure(SkillChargeData.Skill _skill, int _verticalIndex)
    {
        skillUIPrefab = Resources.Load<GameObject>("SceneObjects/SkillChargeUI");
        annuluses = GameObject.Instantiate<GameObject>(skillUIPrefab).GetComponent<SkillChargeAnnulusController>();
        annuluses.setButtonName(_skill.buttonFileAndFile);
        annuluses.GetComponent<Button>().onClick.AddListener(tryUseCharge);
        annuluses.transform.SetParent(GameObject.Find("Canvas").transform, false);
        originalScale = new Vector3(1, 1, 1);
        skill = _skill;

    }
    abstract public void fullCharge();
    abstract public void revokeUse();
    abstract public bool useCharge();
    abstract public bool CouldUse
    {
        get;
    }
    abstract public void normalUpdate(float time);
    public virtual void chargeUpdate(float time)
    {
        if (!skill.Enabled) return;
        //VerticalIndex = verticalIndex;
        
        normalUpdate(time);
        drawCharge();
    }
    void tryUseCharge()
    {
        if (CouldUse)
        {
            useCharge();
        }
    }
    public virtual void ifButtonPushedUseCharge()
    {
        if (Input.GetButton(skill.buttonName))
        {
            tryUseCharge();
        }
    }
    protected virtual void drawCharge()
    {
    }

}
public class SkillChargeNormal : SkillCharge {
    int maxChargeCount=0;
    float chargeDegree = 0;
    int chargeCount = 0;
    float timeCount = -1;
    
    int ChargeCount
    {
        get { return chargeCount; }
        set
        {
            chargeCount = value;
            chargeDegree = 0;
        }
    }
    public override void fullCharge()
    {
        ChargeCount = maxChargeCount;
    }
    public override void revokeUse()
    {
        chargeCount++;
    }
    int MaxChargeCount
    {
        get { return maxChargeCount; }
        set
        {
            maxChargeCount = value;
            annuluses.ChargeMaxCount = maxChargeCount;
        }
    }
    void increaseCount()
    {
        maxChargeCount++;
    }
    void declineCount()
    {
        maxChargeCount--;
    }
    public override bool useCharge()
    {
        if (chargeCount == 0) return false;
        skillEvent();
        chargeCount--;
        return true;
    }
    public override bool CouldUse
    {
        get
        {
            return skill.Enabled && chargeCount != 0 && Singleton<Datas>.Instance.isNight;

        }
    }
    protected override void drawCharge()
    {
        annuluses.chargeCount = chargeCount;
        annuluses.nowDegree = chargeDegree;
    }



    public SkillChargeNormal(SkillChargeData.Skill _skill,int _verticalIndex)
    {
        commonStructure(_skill, _verticalIndex);
        chargeCount = _skill.initCount;
        MaxChargeCount = _skill.MaxCount;
        VerticalIndex = _verticalIndex;
    }
    public override void normalUpdate(float time)
    {
        MaxChargeCount = skill.MaxCount;
        if(timeCount == -1)
        {
            if (chargeCount < maxChargeCount)
            {
                timeCount = 0;
            }
        }else 
        if(timeCount < skill.chargeTime)
        {
            timeCount += time;
            chargeDegree = timeCount / skill.chargeTime;
        }else
        {
            chargeCount++;
            timeCount = -1;
            chargeDegree = 0;
        }
    }
    
    
}
public class SkillChargeOne_Time : SkillCharge
{
    public SkillChargeOne_Time(SkillChargeData.Skill _skill, int _verticalIndex)
    {
        commonStructure(_skill,_verticalIndex);
        annuluses.ChargeMaxCount = 1;
        VerticalIndex = _verticalIndex;
    }

    public override bool CouldUse
    {
        get
        {
            return skill.Enabled && Singleton<Datas>.Instance.isNight;
        }
    }

    public override void fullCharge()
    {
        RelatedUpdate.Level = 1;

    }

    public override void normalUpdate(float time)
    {
    }

    public override void revokeUse()
    {
        fullCharge();

    }

    public override bool useCharge()
    {
        if (!CouldUse) return false;
        RelatedUpdate.Level = 0;
        skillEvent();
        return true;
    }
    protected override void drawCharge()
    {
        annuluses.chargeCount = 1;
    }
}
public class SkillChargeOne_TimeEachNight : SkillCharge
{
    bool useLeft;
    public SkillChargeOne_TimeEachNight(SkillChargeData.Skill _skill, int _verticalIndex)
    {
        commonStructure(_skill, _verticalIndex);
        annuluses.ChargeMaxCount = 1;
        VerticalIndex = _verticalIndex;
        useLeft = true;
    }

    public override bool CouldUse
    {
        get
        {
            return skill.Enabled && useLeft && Singleton<Datas>.Instance.isNight;
        }
    }

    public override void fullCharge()
    {
        useLeft = true;
    }



    public override void normalUpdate(float time)
    {
        if(!Singleton<Datas>.Instance.isNight && !useLeft)
        {
            fullCharge();
        }
    }

    public override void revokeUse()
    {
        useLeft = true;
    }

    public override bool useCharge()
    {
        if (!CouldUse) return false;
        skillEvent();
        useLeft = false;
        return true;
    }
    protected override void drawCharge()
    {
        annuluses.chargeCount = CouldUse ? 1 : 0;
    }
}
