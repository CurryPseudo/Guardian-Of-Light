using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Upgrade
{
    UpgradeList allUpdate;
    int index;
    int level = 0;
    GameObject buttonPrefab;
    GameObject buttonInstance;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            if (level != value)
            {
                string name = Singleton<Datas>.Instance.SkillTable.skillList[index].name;
            }
            level = value;
            
        }
    }
    public string Name
    {
        get
        {
            return Singleton<Datas>.Instance.SkillTable.skillList[index].name;
        }
    }
    public GameObject ButtonInstance
    {
        get { return buttonInstance; }
    }
 
    public SkillTable.Skill Skill
    {
        get
        {
            return Singleton<Datas>.Instance.SkillTable.skillList[index];
        }
    }
    public Upgrade(int _index,UpgradeList _allUpdate)
    {
        index = _index;
        level = Singleton<Datas>.Instance.PlayerData.skillLevels[index];
        allUpdate = _allUpdate;
        buttonPrefab = Resources.Load<GameObject>("SceneObjects/UpgradeButton");
        
    }
    public virtual void DoUpgrade()
    {
        level++;
        if (allUpdate.isActiveSkill(Name))
        {
            SkillCharge charge = Singleton<Datas>.Instance.SkillCharges.findSkillCharge(Name);
            charge.fullCharge();
        }
    }
    public virtual void ShowOnScreen(Vector3 position)
    {
        buttonInstance = GameObject.Instantiate(buttonPrefab);
        
        buttonInstance.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + Skill.buttonDirectoryAndFile);
        buttonInstance.transform.Translate(position);
        buttonInstance.transform.SetParent(GameObject.Find("Canvas").transform);
        buttonInstance.transform.localScale = buttonPrefab.transform.localScale;
        buttonInstance.GetComponent<Button>().onClick.AddListener(DoUpgrade);
        
    }
    public bool IsEnable()
    {
        UpgradeList.CheckEnable checkEnable = allUpdate.getCheckEnable(index);
        if (checkEnable != null && !checkEnable())
        {
            return false;
        }
        return level < Skill.eachLevelValues.Count - 1;
    }
}
