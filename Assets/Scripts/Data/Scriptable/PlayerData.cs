using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName ="InitInfo/Create PlayerData")]
public class PlayerData : ScriptableObject {
    
    #region putLightInfo
    [System.Serializable]
    public struct PutLight
    {
        public float startShrinkTime;
        public float StartShrinkTime
        {
            get
            {
                float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("光球时间");
                float initValue = startShrinkTime;
                return skillValue + initValue;
            }
        }
        public float keepTime;
        public float KeepTime
        {
            get
            {
                float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("光球时间");
                float initValue = keepTime;
                return skillValue + initValue;
            }
        }
        public float viewRadius;
        public float ViewRadius
        {
            get
            {
                float skillValue = 1;
                float initValue = viewRadius;
                return skillValue * initValue;
            }
        }
    }
    public PutLight putLight;

    #endregion
    #region lightInfo
    [System.Serializable]
    public struct Light
    {
        public float radius;
        public float ViewRadius
        {
            get
            {
                float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("光圈增大");
                float initValue = radius;
                return skillValue * initValue;
            }
        }
        public AnimationCurve expandCurve;
        public float ExpandValueDuringTime(float degree)
        {
            return expandCurve.Evaluate(degree);
        }
        public float expandDegree;
        public float ExpandDegree
        {
            get
            {

                float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("光圈变化");
                float initValue = expandDegree;
                return skillValue * initValue;
            }
        }
        public float expandTime;
        public float ExpandTime
        {
            get
            {
                return expandTime;
            }
        }
    }
    public Light light;
    #endregion
    #region leapInfo
    [System.Serializable]
    public struct Leap
    {
        public float viewRadius;
        public float ViewRadius
        {
            get
            {
                return viewRadius * Singleton<Datas>.Instance.SkillTable.findValue("光圈增大");
            }
        }
        public float moveSpeed;
        public float MoveSpeed
        {
            get
            {
                return moveSpeed * Singleton<Datas>.Instance.SkillTable.findValue("飞跃速度");
            }
        }
        public float keepTime;
        public float KeepTime
        {
            get
            {
                return keepTime;
            }
        }
        public float animateDelayTime;
        public float AnimateDelayTime
        {
            get
            {
                return animateDelayTime;
            }
        }
        public bool LeapHighObstacles
        {
            get
            {
                float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("飞跃高墙");
                return skillValue == 1;
            }
        }
    }
    public Leap leap;
    #endregion
    #region skillInfo
    public List<int> skillLevels = new List<int>();
    public int activeSkillMaxCount;
    [System.Serializable]
    public struct SummonTheLight
    {
        public float keepTime;
        public float KeepTime
        {
            get
            {
                return keepTime;
            }
        }
        public float startShrinkTime;
        public float StartShrinkTime
        {
            get
            {
                return startShrinkTime;
            }
        }
        public float viewRadius;
        public float ViewRadius
        {
            get
            {
                return viewRadius;
            }
        }
    }
    public SummonTheLight summonTheLight;
    [System.Serializable]
    public struct MonsterInduction
    {
        public float keepTime;
        public float KeepTime
        {
            get
            {
                return keepTime;
            }
        }
    }
    public MonsterInduction monsterInduction;
    #endregion
    #region physicalInfo
    [System.Serializable]
    public struct Physic
    {
        public float moveSpeed;
        public float MoveSpeed
        {
            get
            {
                float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("移动速度");
                float initValue = moveSpeed;
                return skillValue * initValue;
            }
        }
    }
    public Physic physic;
    #endregion
}
