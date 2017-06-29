using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="InitInfo/Create MonsterData")]
public class MonsterData : ScriptableObject
{
    public List<MutationInfo> mutationInfos = new List<MutationInfo>();
    public MutationInfo getMutationInfo(string name)
    {
        for(int i = 0; i < mutationInfos.Count; i++)
        {
            if(mutationInfos[i].name == name)
            {
                return mutationInfos[i];
            }
        }
        return null;
    }
    public MutationInfo getMutationInfo(int index)
    {
        return mutationInfos[index];
    }
    public void addMutationInfo()
    {
        MutationInfo info = new MutationInfo();
        mutationInfos.Add(info);
    }
    public void removeMutationInfo(int index)
    {
        MutationInfo info = mutationInfos[index];
        removeMutationInfo(info);
    }
    public void removeMutationInfo(string name)
    {
        MutationInfo info = getMutationInfo(name);
        removeMutationInfo(info);

    }
    public void removeMutationInfo(MutationInfo info)
    {
        mutationInfos.Remove(info);
    }
    public void changeMutationName(MutationInfo info,string name)
    {
        int index = mutationInfos.IndexOf(info);
        info.name = name;
    }
    public List<MonsterInfo> monsterInfoEachNight = new List<MonsterInfo>();
    public MonsterInfo getMonsterInfo(int nightNum)
    {
        return monsterInfoEachNight[nightNum];
    }
    public void addMonsterInfo()
    {
        monsterInfoEachNight.Add(new MonsterInfo(monsterInfoEachNight.Count));
    }
    public AnimationCurve speedChangeCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(0.5f,0.5f),new Keyframe(1,1));
    public AnimationCurve escapeSpeedChangeCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(0.5f,0.5f),new Keyframe(1,1));
    public AnimationCurve attackChangeCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(0.5f,0.5f),new Keyframe(1,1));
    public MonsterInfo getNowMonsterInfo(int nightNum,float nightDegree)
    {
        MonsterInfo result = new MonsterInfo(0);
        MonsterInfo infoThisNight = getMonsterInfo(nightNum);
        result.originalSpeed = Mathf.Lerp(infoThisNight.originalSpeed,infoThisNight.finalSpeed, speedChangeCurve.Evaluate(nightDegree));
        result.originalEscapeSpeed = Mathf.Lerp(infoThisNight.originalEscapeSpeed, infoThisNight.finalEscapeSpeed, escapeSpeedChangeCurve.Evaluate(nightDegree));
        result.originalAttack = Mathf.Lerp(infoThisNight.originalAttack, infoThisNight.finalAttack, attackChangeCurve.Evaluate(nightDegree));
        result.mutationProbability = infoThisNight.mutationProbability;
        result.monsterNum = infoThisNight.monsterNum;
        return result;
    }
}
