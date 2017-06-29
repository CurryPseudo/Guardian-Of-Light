using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "InitInfo/Create LittleManData")]
public class LittleManData : ScriptableObject {
    public float Health(float originalHealth)
    {
        float skillValue = Singleton<Datas>.Instance.SkillTable.findValue("村民血量");
        return originalHealth * skillValue;
    }
    
    [System.Serializable]
    public struct CounterAttack
    {
        public float Probability
        {
            get
            {
                return Singleton<Datas>.Instance.SkillTable.findValue("村民反击");
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
        public float keepTime;
        public float KeepTime
        {
            get
            {
                return keepTime;
            }
        }

    }

    public CounterAttack counterAttack;
}
