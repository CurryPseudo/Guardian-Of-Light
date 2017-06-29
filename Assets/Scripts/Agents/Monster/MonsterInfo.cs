using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MonsterInfo
{
    public int index;
    public float originalSpeed;
    public float finalSpeed;
    public float originalEscapeSpeed;
    public float finalEscapeSpeed;
    public float originalAttack;
    public float finalAttack;
    public float mutationProbability;
    public int monsterNum;
    public MonsterInfo(int _index)
    {
        index = _index;
        originalSpeed = 1;
        finalSpeed = 1;
        originalAttack = 1;
        finalAttack = 1;
        originalEscapeSpeed = 1;
        finalEscapeSpeed = 1;
        mutationProbability = 0;
        monsterNum = 0;
    }
}
[System.Serializable]
public class MutationInfo
{
    public string name;
    public string instruction;
    public float randWeight;
    public List<float> valueEachLevel;
    public MutationInfo()
    {
        name = "未命名";
        instruction = "未说明";
        randWeight = 0;
        valueEachLevel = new List<float>();
    }
    public float getValue(int level)
    {
        return valueEachLevel[level];
    }
    public float ValuesCount
    {
        set
        {
            while (valueEachLevel.Count > value)
            {
                valueEachLevel.RemoveAt(valueEachLevel.Count-1);
            }
            while (valueEachLevel.Count < value)
            {
                valueEachLevel.Add(0);
            }
        }
    }

}
public class MutationStatus
{
    List<int> mutationLevel;
    MonsterData data;
    public void mutationLevelUp(int mutationIndex)
    {
        if( mutationImproveEnable(mutationIndex)){
            mutationLevel[mutationIndex]++;
        }
    }
    public new string ToString()
    {
        string result = "";
        for(int i = 0; i < mutationLevel.Count; i++)
        {
            result += data.getMutationInfo(i).name;
            result += ": " + getMutationLevel(i) +" ";
        }
        return result;
    }
    public bool mutationImproveEnable(int mutationIndex)
    {
        return mutationLevel[mutationIndex] < data.getMutationInfo(mutationIndex).valueEachLevel.Count - 1;
    }
    public int getMutationLevel(int mutationIndex)
    {
        return mutationLevel[mutationIndex];
    }
    public MutationInfo getMutationInfo(int mutationIndex)
    {
       return data.getMutationInfo(mutationIndex);
    }
    public MutationInfo getMutationInfo(string name)
    {
        return data.getMutationInfo(name);
    }
    public int getMutationLevel(string name)
    {
        return mutationLevel[data.mutationInfos.IndexOf(data.getMutationInfo(name))];
    }
    public float getMutationValue(string name)
    {
        return data.getMutationInfo(name).valueEachLevel[getMutationLevel(name)];
    }
    public float getMutationValue(int index)
    {
        return data.getMutationInfo(index).valueEachLevel[getMutationLevel(index)];
    }
    public MutationStatus(MonsterData _data)
    {
        data = _data;
        int mutationNum = data.mutationInfos.Count;
        mutationLevel = new List<int>();
        for(int i = 0; i < mutationNum; i++)
        {
            mutationLevel.Add(0);
        }
    }
    public void randMutation()
    {
        List<int> enableMutations = new List<int>();
        for(int i = 0; i < mutationLevel.Count; i++)
        {
            if (mutationImproveEnable(i))
            {
                enableMutations.Add(i);
            }
        }
        float[] randWeightSums = new float[enableMutations.Count + 1];
        randWeightSums[0] = 0;
        for(int i = 0; i < enableMutations.Count; i++)
        {
            randWeightSums[i + 1] = randWeightSums[i] + getMutationInfo(enableMutations[i]).randWeight;
        }
        float randValue = Random.value * randWeightSums[enableMutations.Count];
        int resultMutationIndex = 0;
        for(int i = 1; i < randWeightSums.Length; i++)
        {
            if (randValue < randWeightSums[i])
            {
                resultMutationIndex = enableMutations[i-1];
                break;
            }
        }
        mutationLevelUp(resultMutationIndex);
    }
}
