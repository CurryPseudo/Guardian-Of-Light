using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillChargeAnnulusController : MonoBehaviour {
    private int chargeMaxCount = 0;
    public int chargeCount;
    [Range(0,1)]
    public float nowDegree;
    public float chargeTime;
    List<SkillChargeAnnulus> annuluses = new List<SkillChargeAnnulus>();
    public int ChargeMaxCount
    {
        get { return chargeMaxCount; }
        set {
            var temp = chargeMaxCount;
            chargeMaxCount = value;
            if(temp != value)setMaxCount();
        }
    }
    public void setButtonName(string name)
    {
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + name);
    }
    void setMaxCount()
    {
        foreach (var annulus in annuluses)
        {
            annulus.destroy();
        }
        annuluses.Clear();
        for (int i = 0; i < chargeMaxCount; i++)
        {
            annuluses.Add(new SkillChargeAnnulus(i ,chargeMaxCount, this));
        }
    }
    private void Start()
    {
        setMaxCount();
    }
    private void Update()
    {
        setCountAndDegree();
    }
    void setCountAndDegree()
    {
        for(int i = 0; i < chargeMaxCount; i++)
        {
            if (i < chargeCount)
            {
                annuluses[i].chargeValue = 1;
            }else if(i == chargeCount)
            {
                annuluses[i].chargeValue = nowDegree;
            }
            else
            {
                annuluses[i].chargeValue = 0;
            }
        }
    }
}
