using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Singleton<T>
    where T : new()
{
    public static T Instance
    {
        get { return SingletonCreator.instance; }
    }
    class SingletonCreator
    {
        internal static readonly T instance = new T();
    }
}
public class Datas : Singleton<Datas>
{
    PlayerData playerData;
    SkillTable skillTable;
    LittleManData littleManData;
    UpgradeList upgradeList;
    SkillChargeData skillChargeData;
    MonsterData monsterData;
    bool night;
    public Datas()
    {
        playerData = Resources.Load("InitialInfo/PlayerData") as PlayerData;
        skillTable = Resources.Load("InitialInfo/SkillTable") as SkillTable;
        skillChargeData = Resources.Load("InitialInfo/SkillChargeData") as SkillChargeData;
        littleManData = Resources.Load("InitialInfo/LittleManData") as LittleManData;
        monsterData = Resources.Load("InitialInfo/MonsterData") as MonsterData;
        night = true;
    }
    public void dataReload()
    {
        upgradeList = null;
        night = true;
    }
    public bool isNight
    {
        get
        {
            return night;
        }
        set
        {
            night = value;
        }
    }

    public MonsterData MonsterData
    {
        get
        {
            return monsterData;
        }
    }
    public UpgradeList UpgradeList
    {
        get
        {
            if(upgradeList==null) upgradeList = new UpgradeList(skillTable.skillList.Count);
            return upgradeList;
        }
    }
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
    }

    public SkillTable SkillTable
    {
        get
        {
            return skillTable;
        }
    }
    public SkillChargeData SkillChargeData
    {
        get
        {
            return skillChargeData;
        }
    }

    public LittleManData LittleManData
    {
        get
        {
            return littleManData;
        }
    }
}

